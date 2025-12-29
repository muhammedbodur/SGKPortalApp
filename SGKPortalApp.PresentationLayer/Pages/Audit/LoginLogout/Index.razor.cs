using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.Auth;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Auth;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Auth;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Audit.LoginLogout
{
    public partial class Index
    {
        [Inject] private ILoginLogoutLogApiService _loginLogoutLogApiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;

        private LoginLogoutLogFilterDto Filter = new()
        {
            StartDate = DateTime.Now.AddDays(-7),
            EndDate = DateTime.Now,
            PageNumber = 1,
            PageSize = 25
        };

        private LoginLogoutLogPagedResultDto? result;
        private bool isLoading = false;
        private int ActiveSessionCount = 0;
        private int TodayLoginCount = 0;

        private bool OnlyActiveSession
        {
            get => Filter.OnlyActiveSession ?? false;
            set => Filter.OnlyActiveSession = value ? true : null;
        }

        private bool OnlyFailedLogins
        {
            get => Filter.OnlyFailedLogins ?? false;
            set => Filter.OnlyFailedLogins = value ? true : null;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadStatisticsAsync();
            await SearchAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            ActiveSessionCount = await _loginLogoutLogApiService.GetActiveSessionCountAsync();
            TodayLoginCount = await _loginLogoutLogApiService.GetTodayLoginCountAsync();
        }

        private async Task SearchAsync()
        {
            isLoading = true;
            try
            {
                result = await _loginLogoutLogApiService.GetLogsAsync(Filter);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync("Kayıtlar yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task GoToPage(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > (result?.TotalPages ?? 1))
                return;

            Filter.PageNumber = pageNumber;
            await SearchAsync();
        }

        private async Task OnPageSizeChangedEvent(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int pageSize))
            {
                Filter.PageSize = pageSize;
                Filter.PageNumber = 1;
                await SearchAsync();
            }
        }

        private async Task ResetFilters()
        {
            Filter = new LoginLogoutLogFilterDto
            {
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now,
                PageNumber = 1,
                PageSize = 25
            };

            await SearchAsync();
        }
    }
}
