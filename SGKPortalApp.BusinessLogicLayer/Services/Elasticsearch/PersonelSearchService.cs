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
using System.Text.Json;

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
        private readonly List<string> _synonyms;

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

            // Synonym'ları yükle
            _synonyms = LoadSynonyms();
        }

        /// <summary>
        /// Synonym'ları dosyadan veya inline ayarlardan yükler
        /// </summary>
        private List<string> LoadSynonyms()
        {
            try
            {
                // Dosya yolu varsa dosyadan oku
                if (!string.IsNullOrEmpty(_settings.SynonymsFilePath) && File.Exists(_settings.SynonymsFilePath))
                {
                    var json = File.ReadAllText(_settings.SynonymsFilePath);
                    var doc = JsonDocument.Parse(json);
                    var synonyms = new List<string>();

                    if (doc.RootElement.TryGetProperty("synonyms", out var synonymsElement))
                    {
                        foreach (var category in synonymsElement.EnumerateObject())
                        {
                            foreach (var item in category.Value.EnumerateArray())
                            {
                                var synonym = item.GetString();
                                if (!string.IsNullOrWhiteSpace(synonym))
                                {
                                    synonyms.Add(synonym);
                                }
                            }
                        }
                    }

                    _logger.LogInformation("Synonym dosyasından {Count} synonym yüklendi: {FilePath}",
                        synonyms.Count, _settings.SynonymsFilePath);
                    return synonyms;
                }

                // Dosya yoksa inline ayarları kullan
                _logger.LogInformation("Inline {Count} synonym kullanılıyor", _settings.Synonyms.Count);
                return _settings.Synonyms;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Synonym yükleme hatası, inline ayarlar kullanılıyor");
                return _settings.Synonyms;
            }
        }

        private async Task<IEnumerable<PersonelElasticDto>> SearchExactNumericAsync(
            string numericTerm,
            IEnumerable<int>? departmanIds,
            bool sadeceAktif,
            int size)
        {
            try
            {
                var response = await _client.SearchAsync<PersonelElasticDto>(s => s
                    .Index(_settings.IndexName)
                    .Size(Math.Min(size, 10))
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
                             .Should(
                                sh => sh.Term(t => t.Field(f => f.TcKimlikNo).Value(numericTerm).Boost(1000.0f)),
                                sh => sh.Term(t => t.Field(f => f.SicilNo).Value(numericTerm).Boost(1000.0f))
                             )
                             .MinimumShouldMatch(1);
                        })
                    )
                );

                return response.IsValidResponse ? response.Documents : Enumerable.Empty<PersonelElasticDto>();
            }
            catch
            {
                return Enumerable.Empty<PersonelElasticDto>();
            }
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
                                .AsciiFolding("tr_ascii", af => af.PreserveOriginal(false)) // FALSE: ü→u dönüşümü yapılsın
                                .EdgeNGram("tr_edge", en => en.MinGram(2).MaxGram(20))
                                .Synonym("tr_synonym", syn => syn.Synonyms(_synonyms))
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

                // Detaylı hata logla
                _logger.LogError("Toplu indexleme hatası: {Error}", bulkResponse.DebugInformation);
                
                // İlk 5 hatalı item'ı detaylı logla
                var errorItems = bulkResponse.Items.Where(i => !i.IsValid).Take(5);
                foreach (var item in errorItems)
                {
                    _logger.LogError("Bulk hata detayı - Index: {Index}, ID: {Id}, Error: {Error}, Cause: {Cause}", 
                        item.Index, 
                        item.Id, 
                        item.Error?.Reason ?? "Bilinmeyen",
                        item.Error?.CausedBy?.Reason ?? "Yok");
                }
                
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
                var response = await _client.DeleteAsync<PersonelElasticDto>(tcKimlikNo, d => d.Index(_settings.IndexName));
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

                var normalizedTerm = searchTerm.Trim();
                var isNumeric = normalizedTerm.All(char.IsDigit);

                // Sayısal arama: önce kesin TC/Sicil eşleşmesi dene (tek kişi olması beklenir)
                if (isNumeric)
                {
                    var exact = await SearchExactNumericAsync(normalizedTerm, departmanIds, sadeceAktif, size);
                    if (exact.Any())
                    {
                        return exact;
                    }
                }

                // Kelime sayısını kontrol et
                var wordCount = normalizedTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                var isMultiWord = wordCount > 1;

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

                            if (isMultiWord)
                            {
                                // ÇOK KELİMELİ ARAMA: TÜM kelimeler bulunmalı (AND)
                                var shouldQueries = new List<Action<QueryDescriptor<PersonelElasticDto>>>();

                                // MUST: kelimelerin tamamı en az bir alanda yakalanmalı.
                                // Not: son kelime kısmi yazıldıysa (örn: "müdür yar") MultiMatch AND kaçırabilir,
                                // bu yüzden MatchPhrasePrefix alternatifini de kabul ediyoruz.
                                var tokens = searchTerm.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                var lastToken = tokens.Length > 0 ? tokens[^1] : string.Empty;
                                var allowPrefixFallback = !string.IsNullOrWhiteSpace(lastToken) && lastToken.Length <= 3;

                                // TC/Sicil tam eşleşme (en yüksek öncelik)
                                shouldQueries.Add(sh => sh.Term(t => t.Field(f => f.TcKimlikNo).Value(searchTerm).Boost(1000.0f)));
                                shouldQueries.Add(sh => sh.Term(t => t.Field(f => f.SicilNo).Value(searchTerm).Boost(1000.0f)));

                                // FullText AND operatörü - TÜM kelimeler bulunmalı (EN ÖNEMLİ)
                                shouldQueries.Add(sh => sh.Match(m => m
                                    .Field(f => f.FullText)
                                    .Query(searchTerm)
                                    .Operator(Operator.And) // TÜM kelimeler olmalı
                                    .Boost(500.0f)
                                ));

                                // AdSoyad AND - ad soyad araması için
                                shouldQueries.Add(sh => sh.Match(m => m
                                    .Field(f => f.AdSoyad)
                                    .Query(searchTerm)
                                    .Operator(Operator.And)
                                    .Boost(400.0f)
                                ));

                                // Multi-field AND: kelimeler farklı alanlara dağılmış olabilir (örn: ilçe + ünvan)
                                // Bunu MUST tarafında zorunlu kılıyoruz; burada sadece skor için boost veriyoruz.
                                shouldQueries.Add(sh => sh.MultiMatch(m => m
                                    .Fields(new[] { "adSoyad^3", "unvanAdi^3", "departmanAdi^2", "servisAdi^2", "fullText" })
                                    .Query(searchTerm)
                                    .Operator(Operator.And)
                                    .Boost(420.0f)
                                ));

                                // Phrase prefix: son kelime kısmi yazıldığında da eşleşsin (örn: "karşıyaka müdür yar")
                                shouldQueries.Add(sh => sh.MatchPhrasePrefix(mp => mp
                                    .Field(f => f.FullText)
                                    .Query(searchTerm)
                                    .Boost(450.0f)
                                ));

                                // Phrase matching: Kelimelerin yakın olması (slop: 3)
                                shouldQueries.Add(sh => sh.MatchPhrase(m => m
                                    .Field(f => f.FullText)
                                    .Query(searchTerm)
                                    .Slop(3)
                                    .Boost(300.0f)
                                ));

                                // Aktif personeller öne çıksın (filtre değil, skor boost)
                                if (!sadeceAktif)
                                {
                                    shouldQueries.Add(sh => sh.Term(t => t
                                        .Field(f => f.Aktif)
                                        .Value(true)
                                        .Boost(50.0f)
                                    ));
                                }

                                // MUST: Her token en az bir alanda geçmeli (sıra bağımsız)
                                var mustTokenQueries = new List<Action<QueryDescriptor<PersonelElasticDto>>>();
                                for (var i = 0; i < tokens.Length; i++)
                                {
                                    var token = tokens[i];
                                    var isLast = i == tokens.Length - 1;
                                    var usePrefix = isLast && allowPrefixFallback;

                                    mustTokenQueries.Add(mu => mu.Bool(mb =>
                                    {
                                        var perTokenShould = new List<Action<QueryDescriptor<PersonelElasticDto>>>
                                        {
                                            sh => sh.Match(m => m.Field(f => f.DepartmanAdi).Query(token).Operator(Operator.Or)),
                                            sh => sh.Match(m => m.Field(f => f.UnvanAdi).Query(token).Operator(Operator.Or)),
                                            sh => sh.Match(m => m.Field(f => f.ServisAdi).Query(token).Operator(Operator.Or)),
                                            sh => sh.Match(m => m.Field(f => f.AdSoyad).Query(token).Operator(Operator.Or)),
                                            sh => sh.Match(m => m.Field(f => f.FullText).Query(token).Operator(Operator.Or))
                                        };

                                        if (usePrefix)
                                        {
                                            perTokenShould.Add(sh => sh.MatchPhrasePrefix(mpp => mpp
                                                .Field(f => f.FullText)
                                                .Query(searchTerm)
                                            ));
                                        }

                                        mb.Should(perTokenShould.ToArray())
                                          .MinimumShouldMatch(1);
                                    }));
                                }

                                b.Filter(filters.ToArray())
                                 .Must(mustTokenQueries.ToArray())
                                 .Should(shouldQueries.ToArray())
                                 .MinimumShouldMatch(1);
                            }
                            else
                            {
                                // TEK KELİME ARAMA
                                var shouldQueries = new List<Action<QueryDescriptor<PersonelElasticDto>>>();
                                var allowNumericPrefix = isNumeric && normalizedTerm.Length < 6;

                                // TC ve Sicil No (en yüksek öncelik)
                                shouldQueries.Add(sh => sh.Term(t => t.Field(f => f.TcKimlikNo).Value(normalizedTerm).Boost(100.0f)));
                                shouldQueries.Add(sh => sh.Term(t => t.Field(f => f.SicilNo).Value(normalizedTerm).Boost(100.0f)));

                                if (allowNumericPrefix)
                                {
                                    shouldQueries.Add(sh => sh.Prefix(p => p.Field(f => f.TcKimlikNo).Value(normalizedTerm).Boost(80.0f)));
                                    shouldQueries.Add(sh => sh.Prefix(p => p.Field(f => f.SicilNo).Value(normalizedTerm).Boost(80.0f)));
                                }

                                if (!isNumeric)
                                {
                                    // Sayısal değilse - metin araması
                                    // Phrase matching
                                    shouldQueries.Add(sh => sh.MatchPhrase(m => m
                                        .Field(f => f.FullText)
                                        .Query(normalizedTerm)
                                        .Boost(30.0f)
                                    ));

                                    // Multi-field
                                    shouldQueries.Add(sh => sh.MultiMatch(m => m
                                        .Fields(new[] { "adSoyad^3", "unvanAdi^2", "departmanAdi", "servisAdi", "fullText" })
                                        .Query(normalizedTerm)
                                        .Operator(Operator.Or)
                                        .Boost(20.0f)
                                    ));

                                    // Match
                                    shouldQueries.Add(sh => sh.Match(m => m
                                        .Field(f => f.FullText)
                                        .Query(normalizedTerm)
                                        .Operator(Operator.Or)
                                        .Boost(15.0f)
                                    ));

                                    // Fuzzy (yazım hatası) - SADECE metin için
                                    shouldQueries.Add(sh => sh.Match(m => m
                                        .Field(f => f.FullText)
                                        .Query(normalizedTerm)
                                        .Fuzziness(new Fuzziness("AUTO"))
                                        .Boost(5.0f)
                                    ));
                                }

                                // Aktif personeller öne çıksın (filtre değil, skor boost)
                                if (!sadeceAktif)
                                {
                                    shouldQueries.Add(sh => sh.Term(t => t
                                        .Field(f => f.Aktif)
                                        .Value(true)
                                        .Boost(10.0f)
                                    ));
                                }

                                b.Filter(filters.ToArray())
                                 .Should(shouldQueries.ToArray())
                                 .MinimumShouldMatch(1);
                            }
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
                    .Indices(_settings.IndexName)
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
