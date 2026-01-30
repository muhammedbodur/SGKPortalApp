# SGK Personel Arama Sistemi – TAM YOL HARİTASI (.NET 9 + Elasticsearch)

Bu doküman; **SGK ölçeğinde**, **yanlış yazım toleranslı**, **yetki bazlı**, **opsiyonel autocomplete** içeren bir **Personel Arama Sistemi**nin uçtan uca **NASIL KURULACAĞINI** anlatır.

> 📌 Amaç: Bu dosya tek başına yeterli olsun. Kopyala–uygula–geliştir.

---

## 🎯 İş Problemi

Kullanıcı arama kutusuna şunları yazabilir:
- `Karşıyaka Prim`
- `Karşyaka pirim`
- `karsyaka pri`

➡️ **Karşıyaka SGK / Prim Tahakkuk Servisi’nde çalışan PERSONELLER** listelenmelidir.

Kullanıcı:
- Düzgün yazmak zorunda değil
- Türkçe karakter bilmek zorunda değil
- Departman / servis ayrımını bilmek zorunda değil

---

## 🧱 Kullanılan Teknolojiler

### Backend
- **.NET 9 (ASP.NET Core Web API)**
- **EF Core 9**
- **Elastic.Clients.Elasticsearch (resmi client)**

### Arama & Indexleme
- **Elasticsearch 8.x**
- Custom Analyzer
- `asciifolding` (ş/ğ/ı toleransı)
- `fuzzy search`
- `synonym filter`
- (Opsiyonel) `edge_ngram` (autocomplete)

---

## 🧩 Mimari Karar (KRİTİK)

### ❌ Yapılmaması Gerekenler
- Elasticsearch’te join
- Departman / Servis için ayrı index
- Runtime join denemeleri

### ✅ Doğru Yaklaşım
- **1 Personel = 1 Elasticsearch document**
- SQL = source of truth
- Elasticsearch = sadece arama
- Veri **denormalize** edilir

---

## 📄 Elasticsearch Document Modeli

```json
{
  "personelId": 123,
  "ad": "Ahmet",
  "soyad": "Yılmaz",
  "sicilNo": "SGK-45821",
  "merkezKodu": "35-KARSIYAKA",
  "departmanAdi": "KARŞIYAKA SOSYAL GÜVENLİK MERKEZİ",
  "servisAdi": "PRİM TAHAKKUK VE TAHSİLAT SERVİSİ",
  "unvan": "Memur",
  "aktif": true,
  "fullText": "Ahmet Yılmaz KARŞIYAKA SOSYAL GÜVENLİK MERKEZİ PRİM TAHAKKUK VE TAHSİLAT SERVİSİ Memur"
}
```

---

# 1️⃣ AUTOCOMPLETE (OPSİYONEL – AÇ/KAPAT)

## Ne Zaman AÇIK?
- Yazdıkça sonuç gösterilecekse
- UI autocomplete varsa

## Ne Zaman KAPALI?
- Sadece Enter ile arama
- Daha az index boyutu isteniyorsa

---

## 🔧 Autocomplete AÇIK – Index Ayarları

```json
"filter": {
  "tr_edge": {
    "type": "edge_ngram",
    "min_gram": 2,
    "max_gram": 20
  }
}
```

```json
"analyzer": {
  "tr_index": {
    "type": "custom",
    "tokenizer": "standard",
    "filter": ["lowercase", "asciifolding", "tr_edge"]
  },
  "tr_search": {
    "type": "custom",
    "tokenizer": "standard",
    "filter": ["lowercase", "asciifolding"]
  }
}
```

> ❗ Autocomplete kapalıysa `tr_edge` ve `tr_index` **HİÇ EKLENMEZ**.

---

# 2️⃣ SYNONYM (YANLIŞ KELİME HARİTASI)

## Amaç
Kullanıcı yanlış kelime yazsa bile doğru sonuç gelsin.

## Örnek Synonym Tanımı

```json
"filter": {
  "tr_synonym": {
    "type": "synonym",
    "synonyms": [
      "pirim => prim",
      "tahakuk => tahakkuk",
      "tahsılat => tahsilat"
    ]
  }
}
```

## Kullanım Yeri
- **search_analyzer**

> ⚠️ Synonym değişirse index rebuild gerekir.

---

# 3️⃣ TAM INDEX OLUŞTURMA (PROD READY)

```json
PUT sgk_personel
{
  "settings": {
    "analysis": {
      "filter": {
        "tr_synonym": {
          "type": "synonym",
          "synonyms": [
            "pirim => prim",
            "tahakuk => tahakkuk"
          ]
        }
      },
      "analyzer": {
        "tr_search": {
          "type": "custom",
          "tokenizer": "standard",
          "filter": ["lowercase", "asciifolding", "tr_synonym"]
        }
      }
    }
  },
  "mappings": {
    "properties": {
      "personelId": { "type": "long" },
      "merkezKodu": { "type": "keyword" },
      "aktif": { "type": "boolean" },
      "fullText": {
        "type": "text",
        "analyzer": "tr_search"
      }
    }
  }
}
```

---

# 4️⃣ EF CORE → ELASTICSEARCH SENKRONİZASYONU

## Temel İlke
- SQL her zaman doğru kaynak
- Elastic sadece arama

---

## 🔁 Senkronizasyon Yöntemleri

### ✅ 1. SaveChanges Sonrası (ÖNERİLEN)

```csharp
public async Task IndexPersonelAsync(Personel p)
{
    var doc = new PersonelElasticDto
    {
        PersonelId = p.Id,
        MerkezKodu = p.Merkez.Kodu,
        Aktif = p.Aktif,
        FullText = $"{p.Ad} {p.Soyad} {p.Departman.Ad} {p.Servis.Ad} {p.Unvan}"
    };

    await _elasticClient.IndexAsync(doc, i => i
        .Index("sgk_personel")
        .Id(p.Id)
    );
}
```

---

### 2. Background Job (Hangfire)
- Büyük veri güncellemeleri
- Gece senkronu

### 3. Outbox Pattern (Kurumsal)
- Transaction garantisi
- Event driven mimari

---

# 5️⃣ YETKİ BAZLI FİLTRELEME (ZORUNLU)

## Senaryo
Kullanıcı sadece **yetkili olduğu SGK merkezlerini** görebilmeli.

---

## Elastic Query İçinde

```json
"filter": [
  { "term": { "aktif": true } },
  { "terms": { "merkezKodu": ["35-KARSIYAKA", "35-BORNOVA"] } }
]
```

✔️ SQL’e dönmeden
✔️ Performanslı
✔️ Güvenli

---

# 6️⃣ TAM ARAMA QUERY (YANLIŞ YAZIM + YETKİ)

```json
{
  "query": {
    "bool": {
      "filter": [
        { "term": { "aktif": true } },
        { "terms": { "merkezKodu": ["35-KARSIYAKA"] } }
      ],
      "should": [
        {
          "match": {
            "fullText": {
              "query": "Karşyaka pirim",
              "operator": "and"
            }
          }
        },
        {
          "match": {
            "fullText": {
              "query": "Karşyaka pirim",
              "fuzziness": "AUTO",
              "operator": "and"
            }
          }
        }
      ],
      "minimum_should_match": 1
    }
  }
}
```

---

# 7️⃣ .NET 9 – C# SEARCH KODU

```csharp
var response = await client.SearchAsync<PersonelElasticDto>(s => s
    .Index("sgk_personel")
    .Size(20)
    .Query(q => q
        .Bool(b => b
            .Filter(
                f => f.Term(t => t.Aktif, true),
                f => f.Terms(t => t.Field("merkezKodu").Terms(userMerkezleri))
            )
            .Should(
                sh => sh.Match(m => m
                    .Field(f => f.FullText)
                    .Query(searchText)
                    .Operator(Operator.And)
                ),
                sh => sh.Match(m => m
                    .Field(f => f.FullText)
                    .Query(searchText)
                    .Fuzziness(Fuzziness.Auto)
                    .Operator(Operator.And)
                )
            )
            .MinimumShouldMatch(1)
        )
    )
);
```

---

## ✅ SONUÇ

Bu dokümanla:
- Yanlış yazım sorun olmaz
- Tek arama kutusu yeterlidir
- Yetkisiz veri asla gelmez
- Autocomplete isteğe bağlıdır
- SGK ölçeğinde prod-ready mimari kurulur

---

## ➕ İLERİ ADIMLAR

- Autocomplete için ayrı index
- Search Template
- Skor tuning
- Kibana dashboard

---

📌 **Bu .md dosyası doğrudan kurumsal dokümantasyon olarak kullanılabilir.**

