using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Elasticsearch;
using SGKPortalApp.BusinessObjectLayer.Configuration;
using SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch;

namespace SGKPortalApp.BusinessLogicLayer.Services.Elasticsearch
{
    /// <summary>
    /// Elasticsearch personel arama servisi implementasyonu
    /// </summary>
    public class PersonelSearchService : IPersonelSearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly ElasticsearchSettings _settings;
        private readonly ILogger<PersonelSearchService> _logger;

        public PersonelSearchService(
            IOptions<ElasticsearchSettings> settings,
            ILogger<PersonelSearchService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            var clientSettings = new ElasticsearchClientSettings(new Uri(_settings.Uri));

            // Authentication varsa ekle
            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                clientSettings = clientSettings.Authentication(
                    new BasicAuthentication(_settings.Username, _settings.Password));
            }

            _client = new ElasticsearchClient(clientSettings);
        }

        public async Task<bool> PingAsync()
        {
            try
            {
                var response = await _client.PingAsync();
                return response.IsValidResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elasticsearch ping hatası");
                return false;
            }
        }

        public async Task<bool> IndexExistsAsync()
        {
            try
            {
                var response = await _client.Indices.ExistsAsync(_settings.IndexName);
                return response.Exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index exists kontrolü hatası");
                return false;
            }
        }

        public async Task<bool> CreateIndexAsync()
        {
            try
            {
                if (await IndexExistsAsync())
                {
                    _logger.LogInformation("Index zaten mevcut: {IndexName}", _settings.IndexName);
                    return true;
                }

                var createIndexResponse = await _client.Indices.CreateAsync(_settings.IndexName, c => c
                    .Settings(s => s
                        .Analysis(a => a
                            .TokenFilters(tf => tf
                                .AsciiFolding("tr_ascii", af => af.PreserveOriginal(true))
                                .EdgeNGram("tr_edge", en => en.MinGram(2).MaxGram(20))
                                .Synonym("tr_synonym", syn => syn.Synonyms(_settings.Synonyms))
                            )
                            .Analyzers(an => an
                                .Custom("tr_index", ca => ca
                                    .Tokenizer("standard")
                                    .Filter(new[] { "lowercase", "tr_ascii", "tr_edge" })
                                )
                                .Custom("tr_search", ca => ca
                                    .Tokenizer("standard")
                                    .Filter(new[] { "lowercase", "tr_ascii", "tr_synonym" })
                                )
                            )
                        )
                    )
                    .Mappings(m => m
                        .Properties<PersonelElasticDto>(p => p
                            .Keyword(k => k.TcKimlikNo)
                            .Text(t => t.Ad, t => t.Analyzer("tr_search"))
                            .Text(t => t.Soyad, t => t.Analyzer("tr_search"))
                            .Text(t => t.AdSoyad, t => t.Analyzer("tr_search"))
                            .Keyword(k => k.SicilNo)
                            .IntegerNumber(i => i.DepartmanId)
                            .Text(t => t.DepartmanAdi, t => t.Analyzer("tr_search"))
                            .IntegerNumber(i => i.ServisId)
                            .Text(t => t.ServisAdi, t => t.Analyzer("tr_search"))
                            .IntegerNumber(i => i.UnvanId)
                            .Text(t => t.UnvanAdi, t => t.Analyzer("tr_search"))
                            .Keyword(k => k.Resim)
                            .IntegerNumber(i => i.PersonelAktiflikDurum)
                            .Boolean(b => b.Aktif)
                            .Text(t => t.FullText, t => t
                                .Analyzer("tr_index")
                                .SearchAnalyzer("tr_search")
                            )
                            .Date(d => d.GuncellemeTarihi)
                        )
                    )
                );

                if (createIndexResponse.IsValidResponse)
                {
                    _logger.LogInformation("Index başarıyla oluşturuldu: {IndexName}", _settings.IndexName);
                    return true;
                }

                _logger.LogError("Index oluşturma hatası: {Error}", createIndexResponse.DebugInformation);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index oluşturma hatası");
                return false;
            }
        }

        public async Task<bool> RecreateIndexAsync()
        {
            try
            {
                if (await IndexExistsAsync())
                {
                    var deleteResponse = await _client.Indices.DeleteAsync(_settings.IndexName);
                    if (!deleteResponse.IsValidResponse)
                    {
                        _logger.LogError("Index silme hatası: {Error}", deleteResponse.DebugInformation);
                        return false;
                    }
                    _logger.LogInformation("Index silindi: {IndexName}", _settings.IndexName);
                }

                return await CreateIndexAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index yeniden oluşturma hatası");
                return false;
            }
        }

        public async Task<bool> IndexPersonelAsync(PersonelElasticDto personel)
        {
            try
            {
                var response = await _client.IndexAsync(personel, i => i
                    .Index(_settings.IndexName)
                    .Id(personel.TcKimlikNo)
                );

                if (response.IsValidResponse)
                {
                    _logger.LogDebug("Personel indexlendi: {TcKimlikNo}", personel.TcKimlikNo);
                    return true;
                }

                _logger.LogError("Personel indexleme hatası: {Error}", response.DebugInformation);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel indexleme hatası: {TcKimlikNo}", personel.TcKimlikNo);
                return false;
            }
        }

        public async Task<int> BulkIndexAsync(IEnumerable<PersonelElasticDto> personeller)
        {
            try
            {
                var bulkResponse = await _client.BulkAsync(b => b
                    .Index(_settings.IndexName)
                    .IndexMany(personeller, (op, doc) => op.Id(doc.TcKimlikNo))
                );

                if (bulkResponse.IsValidResponse)
                {
                    var indexedCount = bulkResponse.Items.Count(i => i.IsValid);
                    _logger.LogInformation("Toplu indexleme tamamlandı: {Count} personel", indexedCount);
                    return indexedCount;
                }

                _logger.LogError("Toplu indexleme hatası: {Error}", bulkResponse.DebugInformation);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Toplu indexleme hatası");
                return 0;
            }
        }

        public async Task<bool> DeletePersonelAsync(string tcKimlikNo)
        {
            try
            {
                var response = await _client.DeleteAsync(_settings.IndexName, tcKimlikNo);
                return response.IsValidResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Personel silme hatası: {TcKimlikNo}", tcKimlikNo);
                return false;
            }
        }

        public async Task<IEnumerable<PersonelElasticDto>> SearchAsync(
            string searchTerm,
            IEnumerable<int>? departmanIds = null,
            bool sadeceAktif = true,
            int size = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return Enumerable.Empty<PersonelElasticDto>();

                var response = await _client.SearchAsync<PersonelElasticDto>(s => s
                    .Index(_settings.IndexName)
                    .Size(size)
                    .Query(q => q
                        .Bool(b =>
                        {
                            // Filter koşulları
                            var filters = new List<Action<QueryDescriptor<PersonelElasticDto>>>();

                            if (sadeceAktif)
                            {
                                filters.Add(f => f.Term(t => t.Field(p => p.Aktif).Value(true)));
                            }

                            if (departmanIds != null && departmanIds.Any())
                            {
                                filters.Add(f => f.Terms(t => t
                                    .Field(p => p.DepartmanId)
                                    .Terms(new TermsQueryField(departmanIds.Select(id => FieldValue.Long(id)).ToArray()))
                                ));
                            }

                            // Should koşulları (en az 1 eşleşmeli)
                            b.Filter(filters.ToArray())
                             .Should(
                                // Tam eşleşme (daha yüksek skor)
                                sh => sh.Match(m => m
                                    .Field(f => f.FullText)
                                    .Query(searchTerm)
                                    .Operator(Operator.And)
                                    .Boost(2.0f)
                                ),
                                // Fuzzy arama (yanlış yazım toleransı)
                                sh => sh.Match(m => m
                                    .Field(f => f.FullText)
                                    .Query(searchTerm)
                                    .Fuzziness(new Fuzziness("AUTO"))
                                    .Operator(Operator.And)
                                ),
                                // Sicil no ile eşleşme
                                sh => sh.Prefix(p => p
                                    .Field(f => f.SicilNo)
                                    .Value(searchTerm)
                                    .Boost(3.0f)
                                ),
                                // TC ile eşleşme
                                sh => sh.Prefix(p => p
                                    .Field(f => f.TcKimlikNo)
                                    .Value(searchTerm)
                                    .Boost(3.0f)
                                )
                             )
                             .MinimumShouldMatch(1);

                            return b;
                        })
                    )
                );

                if (response.IsValidResponse)
                {
                    return response.Documents;
                }

                _logger.LogError("Arama hatası: {Error}", response.DebugInformation);
                return Enumerable.Empty<PersonelElasticDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Arama hatası: {SearchTerm}", searchTerm);
                return Enumerable.Empty<PersonelElasticDto>();
            }
        }

        public async Task<IEnumerable<PersonelElasticDto>> AutocompleteAsync(
            string prefix,
            IEnumerable<int>? departmanIds = null,
            bool sadeceAktif = true,
            int size = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prefix) || prefix.Length < 2)
                    return Enumerable.Empty<PersonelElasticDto>();

                var response = await _client.SearchAsync<PersonelElasticDto>(s => s
                    .Index(_settings.IndexName)
                    .Size(size)
                    .Query(q => q
                        .Bool(b =>
                        {
                            var filters = new List<Action<QueryDescriptor<PersonelElasticDto>>>();

                            if (sadeceAktif)
                            {
                                filters.Add(f => f.Term(t => t.Field(p => p.Aktif).Value(true)));
                            }

                            if (departmanIds != null && departmanIds.Any())
                            {
                                filters.Add(f => f.Terms(t => t
                                    .Field(p => p.DepartmanId)
                                    .Terms(new TermsQueryField(departmanIds.Select(id => FieldValue.Long(id)).ToArray()))
                                ));
                            }

                            b.Filter(filters.ToArray())
                             .Must(m => m
                                .MatchPhrasePrefix(mp => mp
                                    .Field(f => f.FullText)
                                    .Query(prefix)
                                )
                             );

                            return b;
                        })
                    )
                );

                if (response.IsValidResponse)
                {
                    return response.Documents;
                }

                _logger.LogError("Autocomplete hatası: {Error}", response.DebugInformation);
                return Enumerable.Empty<PersonelElasticDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Autocomplete hatası: {Prefix}", prefix);
                return Enumerable.Empty<PersonelElasticDto>();
            }
        }

        public async Task<long> GetDocumentCountAsync()
        {
            try
            {
                var response = await _client.CountAsync<PersonelElasticDto>(c => c
                    .Index(_settings.IndexName)
                );

                return response.IsValidResponse ? response.Count : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doküman sayısı alma hatası");
                return 0;
            }
        }
    }
}
