using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Log
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
        /// Alan adını Türkçe/okunabilir hale getirir
        /// </summary>
        private string GetFriendlyFieldName(string fieldName)
        {
            // Yaygın alan adları için mapping
            var fieldMappings = new Dictionary<string, string>
            {
                // Yetki alanları
                ["YetkiSeviyesi"] = "Yetki Seviyesi",
                ["ModulControllerIslemId"] = "İşlem/Sayfa",

                // Kişisel bilgiler
                ["AdSoyad"] = "Ad Soyad",
                ["TcKimlikNo"] = "TC Kimlik No",
                ["SicilNo"] = "Sicil No",
                ["DogumTarihi"] = "Doğum Tarihi",
                ["Cinsiyet"] = "Cinsiyet",
                ["MedeniDurumu"] = "Medeni Durum",
                ["KanGrubu"] = "Kan Grubu",

                // Organizasyon
                ["DepartmanId"] = "Departman",
                ["ServisId"] = "Servis",
                ["UnvanId"] = "Ünvan",
                ["HizmetBinasiId"] = "Hizmet Binası",

                // İletişim
                ["Email"] = "E-posta",
                ["CepTelefonu"] = "Cep Telefonu",
                ["EvTelefonu"] = "Ev Telefonu",
                ["Adres"] = "Adres",

                // Tarih alanları
                ["EklenmeTarihi"] = "Eklenme Tarihi",
                ["DuzenlenmeTarihi"] = "Düzenlenme Tarihi",
                ["SilinmeTarihi"] = "Silinme Tarihi",

                // Kullanıcı alanları
                ["EkleyenKullanici"] = "Ekleyen Kullanıcı",
                ["DuzenleyenKullanici"] = "Düzenleyen Kullanıcı",
                ["SilenKullanici"] = "Silen Kullanıcı"
            };

            return fieldMappings.TryGetValue(fieldName, out var friendlyName)
                ? friendlyName
                : fieldName;
        }

        /// <summary>
        /// Değeri açıklayıcı hale getirir (enum mapping, vb)
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
                    "1" => "1 (Görüntüleme)",
                    "2" => "2 (Düzenleme)",
                    _ => value
                };
            }

            // Cinsiyet enum mapping
            if (fieldName == "Cinsiyet")
            {
                return value switch
                {
                    "0" => "0 (Belirtilmemiş)",
                    "1" => "1 (Erkek)",
                    "2" => "2 (Kadın)",
                    _ => value
                };
            }

            // MedeniDurumu enum mapping
            if (fieldName == "MedeniDurumu")
            {
                return value switch
                {
                    "0" => "0 (Belirtilmemiş)",
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

            // Boolean değerler
            if (fieldName.Contains("Mi") || fieldName.Contains("Aktif"))
            {
                return value switch
                {
                    "True" or "true" => "Evet",
                    "False" or "false" => "Hayır",
                    _ => value
                };
            }

            return value;
        }
    }
}
