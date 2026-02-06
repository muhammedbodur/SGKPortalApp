using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
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

namespace SGKPortalApp.PresentationLayer.Pages.Haberler
{
    public partial class HaberAdd : FieldPermissionPageBase
    {
        // ─── DI ──────────────────────────────────────────────

        [Inject] private IHaberApiService HaberApi { get; set; } = default!;
        [Inject] private ImageHelper ImageHelper { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime Js { get; set; } = default!;

        // ─── State ───────────────────────────────────────────

        private bool IsLoading { get; set; } = true;
        private bool IsSaving { get; set; } = false;
        private bool FormSubmitted { get; set; } = false;

        // Form model
        private HaberFormModel Model { get; set; } = new();

        private const int MaxIcerikChars = 5000;
        private int IcerikCharCount { get; set; } = 0;
        private DotNetObjectReference<HaberAdd>? _dotNetRef;

        // Aktiflik (enum → int bridge)
        private int AktiflikValue
        {
            get => Model.Aktiflik == Aktiflik.Aktif ? 1 : 0;
            set => Model.Aktiflik = value == 1 ? Aktiflik.Aktif : Aktiflik.Pasif;
        }

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
            await Task.CompletedTask;
            IsLoading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            _dotNetRef = DotNetObjectReference.Create(this);

            try
            {
                await Js.InvokeVoidAsync(
                    "sgkQuill.init",
                    "#haberAdd-editor",
                    "#haberAdd-toolbar",
                    Model.Icerik,
                    _dotNetRef,
                    MaxIcerikChars);
            }
            catch
            {
            }
        }

        [JSInvokable]
        public Task OnQuillContentChanged(string html, int textLen)
        {
            Model.Icerik = html ?? string.Empty;
            IcerikCharCount = textLen;
            return InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            _dotNetRef?.Dispose();
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

                    // Resize & optimize (800x600, quality 85)
                    var optimized = await ImageHelper.LoadResizeAndOptimizeAsync(ms, maxWidth: 800, maxHeight: 600, quality: 85);
                    if (optimized == null)
                    {
                        UploadError = $"\"{file.Name}\" — resim işleme hatası.";
                        return;
                    }

                    // Preview için base64 data URL oluştur
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

        private void SetPendingVitrin(PendingImageItem item)
        {
            PendingImages.ForEach(p => p.IsVitrin = false);
            item.IsVitrin = true;
        }

        // ─── Vitrin Preview URL (for sidebar preview) ────────

        private string? VitrinPreviewUrl
        {
            get
            {
                var pendingVitrin = PendingImages.FirstOrDefault(p => p.IsVitrin);
                if (pendingVitrin != null) return pendingVitrin.PreviewUrl;

                if (PendingImages.Count > 0) return PendingImages[0].PreviewUrl;

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

            if (string.IsNullOrWhiteSpace(Model.Baslik) || string.IsNullOrWhiteSpace(Model.Icerik))
            {
                StateHasChanged();
                return;
            }

            IsSaving = true;
            try
            {
                // Create haber
                var createDto = new HaberCreateRequestDto
                {
                    Baslik = Model.Baslik,
                    Icerik = Model.Icerik,
                    Sira = Model.Sira,
                    YayinTarihi = Model.YayinTarihi,
                    BitisTarihi = Model.BitisTarihi,
                    Aktiflik = Model.Aktiflik
                };

                var createResult = await HaberApi.CreateHaberAsync(createDto);
                if (!createResult.Success || createResult.Data == null)
                {
                    ShowToast("Haber oluşturma başarısız oldu.", "danger");
                    return;
                }

                var haberId = createResult.Data.HaberId;

                // ── Yeni resimler kaydet ──
                int siraCounter = 1;
                foreach (var pending in PendingImages)
                {
                    // Dosyayı diske kaydet
                    var safeFileName = $"haber_{haberId}_{siraCounter}_{Guid.NewGuid().ToString("N")[..8]}.jpg";
                    var savedPath = await ImageHelper.SaveImageAsync(pending.ImageBytes, safeFileName, subfolder: "haberler");

                    // API'ye resim kaydı ekle
                    var resimDto = new HaberResimCreateRequestDto
                    {
                        HaberId = haberId,
                        ResimUrl = savedPath,
                        IsVitrin = pending.IsVitrin,
                        Sira = siraCounter
                    };

                    await HaberApi.AddResimAsync(haberId, resimDto);
                    siraCounter++;
                }

                ShowToast("Haber başarıyla oluşturuldu.", "success");

                // 1.5s sonra yönetim sayfasına yönlendir
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
