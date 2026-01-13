using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.AuditLog;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.AuditLog;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Audit.Log
{
    public partial class Detail
    {
        [Parameter]
        public int LogId { get; set; }

        [Inject]
        private IAuditLogApiService AuditLogApiService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private AuditLogDetailDto? detail;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadDetailAsync();
        }

        private async Task LoadDetailAsync()
        {
            isLoading = true;
            try
            {
                detail = await AuditLogApiService.GetLogDetailAsync(LogId);
            }
            finally
            {
                isLoading = false;
            }
        }

        private string GetActionBadgeClass(DatabaseAction action)
        {
            return action switch
            {
                DatabaseAction.INSERT or DatabaseAction.CREATE_COMPLETE => "bg-success",
                DatabaseAction.UPDATE or DatabaseAction.UPDATE_COMPLETE => "bg-warning text-dark",
                DatabaseAction.DELETE => "bg-danger",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Alan adÄ±nÄ± TÃ¼rkÃ§e/okunabilir hale getirir
        /// </summary>
        private string GetFriendlyFieldName(string fieldName)
        {
            // YaygÄ±n alan adlarÄ± iÃ§in mapping
            var fieldMappings = new Dictionary<string, string>
            {
                // Yetki alanlarÄ±
                ["YetkiSeviyesi"] = "Yetki Seviyesi",
                ["ModulControllerIslemId"] = "Ä°ÅŸlem/Sayfa",
                ["UstIslemId"] = "Ãœst Ä°ÅŸlem",

                // KiÅŸisel bilgiler
                ["AdSoyad"] = "Ad Soyad",
                ["TcKimlikNo"] = "TC Kimlik No",
                ["SicilNo"] = "Sicil No",
                ["DogumTarihi"] = "DoÄŸum Tarihi",
                ["Cinsiyet"] = "Cinsiyet",
                ["MedeniDurumu"] = "Medeni Durum",
                ["KanGrubu"] = "Kan Grubu",

                // Organizasyon
                ["DepartmanId"] = "Departman",
                ["ServisId"] = "Servis",
                ["UnvanId"] = "Ãœnvan",
                ["SendikaId"] = "Sendika",
                ["AtanmaNedeniId"] = "Atanma Nedeni",
                ["HizmetBinasiId"] = "Hizmet BinasÄ±",

                // Lokasyon
                ["IlId"] = "Ä°l",
                ["IlceId"] = "Ä°lÃ§e",
                ["EsininIsIlId"] = "EÅŸinin Ä°ÅŸ Ä°li",
                ["EsininIsIlceId"] = "EÅŸinin Ä°ÅŸ Ä°lÃ§esi",

                // ModÃ¼l/Yetki Sistemi
                ["ModulId"] = "ModÃ¼l",
                ["ModulControllerId"] = "Controller",
                ["UstModulControllerId"] = "Ãœst Controller",

                // SÄ±ramatik - Banko
                ["BankoId"] = "Banko",
                ["YonlendirenBankoId"] = "YÃ¶nlendiren Banko",
                ["HedefBankoId"] = "Hedef Banko",
                ["AktifBankoId"] = "Aktif Banko",
                ["BankoNo"] = "Banko No",

                // SÄ±ramatik - DiÄŸer
                ["TvId"] = "TV",
                ["TvAdi"] = "TV AdÄ±",
                ["KioskId"] = "Kiosk",
                ["KioskAdi"] = "Kiosk AdÄ±",
                ["SiraId"] = "SÄ±ra",
                ["SiraNo"] = "SÄ±ra No",
                ["KanalId"] = "Kanal",
                ["KanalAdi"] = "Kanal AdÄ±",
                ["KanalAltId"] = "Kanal Alt",
                ["KanalAltAdi"] = "Kanal Alt AdÄ±",
                ["KanalIslemId"] = "Kanal Ä°ÅŸlem",
                ["KanalIslemAdi"] = "Kanal Ä°ÅŸlem AdÄ±",
                ["KanalAltIslemId"] = "Kanal Alt Ä°ÅŸlem",
                ["KanalAltIslemAdi"] = "Kanal Alt Ä°ÅŸlem AdÄ±",

                // PDKS
                ["PdksCihazId"] = "PDKS CihazÄ±",
                ["CihazIP"] = "Cihaz IP",
                ["CihazPort"] = "Cihaz Port",

                // Ä°letiÅŸim
                ["Email"] = "E-posta",
                ["CepTelefonu"] = "Cep Telefonu",
                ["EvTelefonu"] = "Ev Telefonu",
                ["Adres"] = "Adres",

                // Tarih alanlarÄ±
                ["EklenmeTarihi"] = "Eklenme Tarihi",
                ["DuzenlenmeTarihi"] = "DÃ¼zenlenme Tarihi",
                ["SilinmeTarihi"] = "Silinme Tarihi",
                ["IslemZamani"] = "Ä°ÅŸlem ZamanÄ±",
                ["KontrolZamani"] = "Kontrol ZamanÄ±",

                // KullanÄ±cÄ± alanlarÄ±
                ["EkleyenKullanici"] = "Ekleyen KullanÄ±cÄ±",
                ["DuzenleyenKullanici"] = "DÃ¼zenleyen KullanÄ±cÄ±",
                ["SilenKullanici"] = "Silen KullanÄ±cÄ±",

                // DiÄŸer yaygÄ±n alanlar
                ["Aktiflik"] = "Aktiflik",
                ["Durum"] = "Durum",
                ["Aciklama"] = "AÃ§Ä±klama",
                ["IslemSayisi"] = "Ä°ÅŸlem SayÄ±sÄ±",
                ["IslemBasari"] = "Ä°ÅŸlem BaÅŸarÄ±",
                ["IslemDurum"] = "Ä°ÅŸlem Durum",
                ["KontrolSayisi"] = "Kontrol SayÄ±sÄ±",
                ["KontrolBasari"] = "Kontrol BaÅŸarÄ±",
                ["KontrolDurum"] = "Kontrol Durum"
            };

            return fieldMappings.TryGetValue(fieldName, out var friendlyName)
                ? friendlyName
                : fieldName;
        }

        /// <summary>
        /// DeÄŸeri aÃ§Ä±klayÄ±cÄ± hale getirir (enum mapping, vb)
        /// </summary>
        private string GetFriendlyValue(string fieldName, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return value ?? "";

            // YetkiSeviyesi enum mapping
            if (fieldName == "YetkiSeviyesi")
            {
                return value switch
                {
                    "0" => "0 (Yetki Yok)",
                    "1" => "1 (GÃ¶rÃ¼ntÃ¼leme)",
                    "2" => "2 (DÃ¼zenleme)",
                    _ => value
                };
            }

            // Cinsiyet enum mapping
            if (fieldName == "Cinsiyet")
            {
                return value switch
                {
                    "0" => "0 (BelirtilmemiÅŸ)",
                    "1" => "1 (Erkek)",
                    "2" => "2 (KadÄ±n)",
                    _ => value
                };
            }

            // MedeniDurumu enum mapping
            if (fieldName == "MedeniDurumu")
            {
                return value switch
                {
                    "0" => "0 (BelirtilmemiÅŸ)",
                    "1" => "1 (Bekar)",
                    "2" => "2 (Evli)",
                    _ => value
                };
            }

            // KanGrubu enum mapping
            if (fieldName == "KanGrubu")
            {
                return value switch
                {
                    "0" => "0 (Bilinmiyor)",
                    "1" => "1 (A RH+)",
                    "2" => "2 (A RH-)",
                    "3" => "3 (B RH+)",
                    "4" => "4 (B RH-)",
                    "5" => "5 (AB RH+)",
                    "6" => "6 (AB RH-)",
                    "7" => "7 (0 RH+)",
                    "8" => "8 (0 RH-)",
                    _ => value
                };
            }

            // Boolean deÄŸerler
            if (fieldName.Contains("Mi") || fieldName.Contains("Aktif"))
            {
                return value switch
                {
                    "True" or "true" => "Evet",
                    "False" or "false" => "HayÄ±r",
                    _ => value
                };
            }

            return value;
        }
    }
}
