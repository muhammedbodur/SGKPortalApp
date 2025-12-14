using Microsoft.Extensions.Logging;
using SGKPortalApp.BusinessLogicLayer.Interfaces.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces;
using SGKPortalApp.DataAccessLayer.Repositories.Interfaces.Common;

namespace SGKPortalApp.BusinessLogicLayer.Services.Common
{
    public class ModulControllerIslemService : IModulControllerIslemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModulControllerIslemService> _logger;

        public ModulControllerIslemService(
            IUnitOfWork unitOfWork,
            ILogger<ModulControllerIslemService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponseDto<List<DropdownItemDto>>> GetDropdownAsync()
        {
            try
            {
                var repo = _unitOfWork.GetRepository<IModulControllerIslemRepository>();
                var dropdown = await repo.GetDropdownAsync();

                return ApiResponseDto<List<DropdownItemDto>>
                    .SuccessResult(dropdown.ToList(), "Dropdown listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ModulControllerIslem dropdown listesi getirilirken hata oluştu");
                return ApiResponseDto<List<DropdownItemDto>>
                    .ErrorResult("Dropdown listesi getirilirken bir hata oluştu", ex.Message);
            }
        }
    }
}
