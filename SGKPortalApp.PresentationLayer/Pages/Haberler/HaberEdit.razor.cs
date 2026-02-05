using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Helpers;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using Blazored.TextEditor;

namespace SGKPortalApp.PresentationLayer.Pages.Haberler
{
    public partial class HaberEdit : FieldPermissionPageBase
    {
        // ─── Route Parameter ─────────────────────────────────

        [Parameter] public int Id { get; set; }

        // ─── DI ──────────────────────────────────────────────

        [Inject] private IHaberApiService HaberApi { get; set; } = default!;
        [Inject] private ImageHelper ImageHelper { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        // ─── State ───────────────────────────────────────────

        private BlazoredTextEditor? QuillEditor;
        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool FormSubmitted { get; set; } = false;

        // Form model
        private HaberFormModel Model { get; set; } = new();

        // Aktiflik (enum → int bridge)
        private int AktiflikValue
        {
            get => Model.Aktiflik == Aktiflik.Aktif ? 1 : 0;
            set => Model.Aktiflik = value == 1 ? Aktiflik.Aktif : Aktiflik.Pasif;
        }

        // Mevcut DB resimler (edit modunda)
        private List<HaberResimResponseDto> ExistingImages { get; set; } = new();
        // Silinecek resim IDs
        private List<int> PendingDeleteImageIds { get; set; } = new();

        // Yeni yüklenen resimler (henüz kaydedilmemiş)
        private List<PendingImageItem> PendingImages { get; set; } = new();

        // Upload hata
        private string? UploadError { get; set; }

        // Toast
        private string ToastMessage { get; set; } = "";
        private string ToastType { get; set; } = "success";

        // ─── Init ────────────────────────────────────────────

        protected override async Task OnInitializedAsync()
        {
            await LoadExistingHaber();
            IsLoading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && QuillEditor != null && !string.IsNullOrWhiteSpace(Model.Icerik))
            {
                await QuillEditor.LoadHTMLContent(Model.Icerik);
            }
        }

        private async Task LoadExistingHaber()
        {
            try
            {
                var response = await HaberApi.GetAdminHaberListeAsync(1, 1000);
                if (response.Success && response.Data != null)
                {
                    var haber = response.Data.Items.FirstOrDefault(h => h.HaberId == Id);
                    if (haber != null)
                    {
                        Model.Baslik = haber.Baslik;
                        Model.Icerik = haber.Icerik;
                        Model.Sira = haber.Sira;
                        Model.YayinTarihi = haber.YayinTarihi;
                        Model.BitisTarihi = haber.BitisTarihi;
                        Model.Aktiflik = Aktiflik.Aktif;
                        ExistingImages = haber.Resimler.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowToast($"Haber yükleme hatası: {ex.Message}", "danger");
            }
        }

        // ─── Image Handling ──────────────────────────────────

        private async Task HandleImageUpload(InputFileChangeEventArgs e)
        {
            UploadError = null;
            var maxSize = 5 * 1024 * 1024; // 5 MB
            string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];

            foreach (var file in e.GetMultipleFiles())
            {
                var ext = System.IO.Path.GetExtension(file.Name).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    UploadError = $"\"{file.Name}\" — desteklenmemektedir. Lütfen JPG, PNG, GIF, BMP veya WebP kullanın.";
                    return;
                }

                if (file.Size > maxSize)
                {
                    UploadError = $"\"{file.Name}\" — 5 MB'dan büyük. Lütfen küçük bir dosya seçin.";
                    return;
                }

                try
                {
                    using var stream = file.OpenReadStream(maxSize);
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    ms.Position = 0;

                    var optimized = await ImageHelper.LoadResizeAndOptimizeAsync(ms, maxWidth: 800, maxHeight: 600, quality: 85);
                    if (optimized == null)
                    {
                        UploadError = $"\"{file.Name}\" — resim işleme hatası.";
                        return;
                    }

                    var base64 = Convert.ToBase64String(optimized);
                    var previewUrl = $"data:image/jpeg;base64,{base64}";

                    PendingImages.Add(new PendingImageItem
                    {
                        FileName = file.Name,
                        ImageBytes = optimized,
                        PreviewUrl = previewUrl,
                        IsVitrin = false
                    });
                }
                catch (Exception ex)
                {
                    UploadError = $"\"{file.Name}\" — hata: {ex.Message}";
                    return;
                }
            }

            StateHasChanged();
        }

        private void RemovePendingImage(PendingImageItem item)
        {
            PendingImages.Remove(item);
        }

        private void RemoveExistingImage(HaberResimResponseDto resim)
        {
            PendingDeleteImageIds.Add(resim.HaberResimId);
            ExistingImages.Remove(resim);
        }

        private void SetVitrin(HaberResimResponseDto resim)
        {
            ExistingImages.ForEach(r => r.IsVitrin = false);
            PendingImages.ForEach(p => p.IsVitrin = false);
            resim.IsVitrin = true;
        }

        private void SetPendingVitrin(PendingImageItem item)
        {
            ExistingImages.ForEach(r => r.IsVitrin = false);
            PendingImages.ForEach(p => p.IsVitrin = false);
            item.IsVitrin = true;
        }

        // ─── Vitrin Preview URL ──────────────────────────────

        private string? VitrinPreviewUrl
        {
            get
            {
                var pendingVitrin = PendingImages.FirstOrDefault(p => p.IsVitrin);
                if (pendingVitrin != null) return pendingVitrin.PreviewUrl;

                var existingVitrin = ExistingImages.FirstOrDefault(r => r.IsVitrin);
                if (existingVitrin != null) return existingVitrin.ResimUrl;

                if (PendingImages.Count > 0) return PendingImages[0].PreviewUrl;

                if (ExistingImages.Count > 0) return ExistingImages[0].ResimUrl;

                return null;
            }
        }

        private void OnYayinTarihiInput(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out var dt))
            {
                Model.YayinTarihi = dt;
            }
        }

        private void OnBitisTarihiInput(ChangeEventArgs e)
        {
            var value = e.Value?.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                Model.BitisTarihi = null;
                return;
            }

            if (DateTime.TryParse(value, out var dt))
            {
                Model.BitisTarihi = dt;
            }
        }

        // ─── Save ────────────────────────────────────────────

        private async Task SaveHaber()
        {
            FormSubmitted = true;

            // Editor'dan HTML içeriğini al
            if (QuillEditor != null)
            {
                Model.Icerik = await QuillEditor.GetHTML();
            }

            if (string.IsNullOrWhiteSpace(Model.Baslik) || string.IsNullOrWhiteSpace(Model.Icerik))
            {
                StateHasChanged();
                return;
            }

            IsSaving = true;
            try
            {
                // Update haber
                var updateDto = new HaberUpdateRequestDto
                {
                    HaberId = Id,
                    Baslik = Model.Baslik,
                    Icerik = Model.Icerik,
                    Sira = Model.Sira,
                    YayinTarihi = Model.YayinTarihi,
                    BitisTarihi = Model.BitisTarihi,
                    Aktiflik = Model.Aktiflik
                };

                var updateResult = await HaberApi.UpdateHaberAsync(Id, updateDto);
                if (!updateResult.Success)
                {
                    ShowToast("Güncelleme başarısız oldu.", "danger");
                    return;
                }

                // ── Silinecek resimler sil ──
                foreach (var resimId in PendingDeleteImageIds)
                {
                    await HaberApi.DeleteResimAsync(Id, resimId);
                }

                // ── Yeni resimler kaydet ──
                int siraCounter = ExistingImages.Count + 1;
                foreach (var pending in PendingImages)
                {
                    var safeFileName = $"haber_{Id}_{siraCounter}_{Guid.NewGuid().ToString("N")[..8]}.jpg";
                    var savedPath = await ImageHelper.SaveImageAsync(pending.ImageBytes, safeFileName, subfolder: "haberler");

                    var resimDto = new HaberResimCreateRequestDto
                    {
                        HaberId = Id,
                        ResimUrl = savedPath,
                        IsVitrin = pending.IsVitrin,
                        Sira = siraCounter
                    };

                    await HaberApi.AddResimAsync(Id, resimDto);
                    siraCounter++;
                }

                ShowToast("Haber başarıyla güncellendi.", "success");

                await Task.Delay(1500);
                Nav.NavigateTo("/haberler/yonetim");
            }
            catch (Exception ex)
            {
                ShowToast($"Hata: {ex.Message}", "danger");
            }
            finally
            {
                IsSaving = false;
            }
        }

        // ─── Toast ───────────────────────────────────────────

        private void ShowToast(string message, string type)
        {
            ToastMessage = message;
            ToastType = type;
            _ = Task.Delay(3000).ContinueWith(_ =>
            {
                ToastMessage = "";
                InvokeAsync(StateHasChanged);
            });
        }

        // ─── Models ──────────────────────────────────────────

        private class HaberFormModel
        {
            public string Baslik { get; set; } = "";
            public string Icerik { get; set; } = "";
            public int Sira { get; set; } = 1;
            public DateTime YayinTarihi { get; set; } = DateTime.Now;
            public DateTime? BitisTarihi { get; set; }
            public Aktiflik Aktiflik { get; set; } = Aktiflik.Aktif;
        }

        private class PendingImageItem
        {
            public string FileName { get; set; } = "";
            public byte[] ImageBytes { get; set; } = Array.Empty<byte>();
            public string PreviewUrl { get; set; } = "";
            public bool IsVitrin { get; set; }
        }
    }
}
