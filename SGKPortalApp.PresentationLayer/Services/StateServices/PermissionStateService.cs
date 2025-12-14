using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UserSessionServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Services.StateServices
{
    public class PermissionStateService
    {
        private readonly IPersonelYetkiApiService _personelYetkiApiService;
        private readonly IYetkiApiService _yetkiApiService;
        private readonly IUserInfoService _userInfoService;
        private readonly ILogger<PermissionStateService> _logger;

        private readonly SemaphoreSlim _loadLock = new(1, 1);

        private List<PersonelYetkiResponseDto> _permissions = new();
        private List<YetkiResponseDto> _yetkiler = new();

        public event Action? OnChange;

        public bool IsLoaded { get; private set; }

        public PermissionStateService(
            IPersonelYetkiApiService personelYetkiApiService,
            IYetkiApiService yetkiApiService,
            IUserInfoService userInfoService,
            ILogger<PermissionStateService> logger)
        {
            _personelYetkiApiService = personelYetkiApiService;
            _yetkiApiService = yetkiApiService;
            _userInfoService = userInfoService;
            _logger = logger;
        }

        public async Task EnsureLoadedAsync(bool force = false)
        {
            var tcKimlikNo = _userInfoService.GetTcKimlikNo();
            if (string.IsNullOrWhiteSpace(tcKimlikNo))
                return;

            if (IsLoaded && !force)
                return;

            await _loadLock.WaitAsync();
            try
            {
                if (IsLoaded && !force)
                    return;

                var yetkilerResult = await _yetkiApiService.GetAllAsync();
                if (!yetkilerResult.Success || yetkilerResult.Data == null)
                {
                    _yetkiler = new();
                }
                else
                {
                    _yetkiler = yetkilerResult.Data;
                }

                var permsResult = await _personelYetkiApiService.GetByTcKimlikNoAsync(tcKimlikNo);
                if (!permsResult.Success || permsResult.Data == null)
                {
                    _permissions = new();
                }
                else
                {
                    _permissions = permsResult.Data;
                }

                IsLoaded = true;
                OnChange?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "PermissionStateService.EnsureLoadedAsync hata");
                _yetkiler = new();
                _permissions = new();
                IsLoaded = true;
                OnChange?.Invoke();
            }
            finally
            {
                _loadLock.Release();
            }
        }

        public Task RefreshAsync()
        {
            IsLoaded = false;
            return EnsureLoadedAsync(force: true);
        }

        public YetkiTipleri GetLevel(string controllerAdi, string actionAdi, string? modulControllerIslemAdi = null)
        {
            if (string.IsNullOrWhiteSpace(controllerAdi) || string.IsNullOrWhiteSpace(actionAdi))
                return YetkiTipleri.None;

            var yetki = _yetkiler.FirstOrDefault(y =>
                string.Equals(y.ControllerAdi, controllerAdi, StringComparison.OrdinalIgnoreCase)
                && string.Equals(y.ActionAdi, actionAdi, StringComparison.OrdinalIgnoreCase));

            if (yetki == null)
                return YetkiTipleri.None;

            var allForYetki = _permissions
                .Where(p => p.YetkiId == yetki.YetkiId)
                .ToList();

            if (!allForYetki.Any())
                return YetkiTipleri.None;

            if (string.IsNullOrWhiteSpace(modulControllerIslemAdi))
                return allForYetki.Max(p => p.YetkiTipleri);

            var matching = allForYetki
                .Where(p => string.Equals(p.ModulControllerIslemAdi, modulControllerIslemAdi, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matching.Any())
                return YetkiTipleri.None;

            return matching.Max(p => p.YetkiTipleri);
        }

        public bool CanView(string controllerAdi, string actionAdi, string? modulControllerIslemAdi = null)
            => GetLevel(controllerAdi, actionAdi, modulControllerIslemAdi) >= YetkiTipleri.View;

        public bool CanEdit(string controllerAdi, string actionAdi, string? modulControllerIslemAdi = null)
            => GetLevel(controllerAdi, actionAdi, modulControllerIslemAdi) >= YetkiTipleri.Edit;

        public bool CanDelete(string controllerAdi, string actionAdi, string? modulControllerIslemAdi = null)
            => GetLevel(controllerAdi, actionAdi, modulControllerIslemAdi) >= YetkiTipleri.Delete;
    }
}
