using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Common
{
    public interface IFieldPermissionValidationService
    {
        Task LoadFieldPermissionsAsync();
        bool CanEditField(string dtoTypeName, string fieldName, Dictionary<string, YetkiSeviyesi> userPermissions, out string? permissionKey);
        Task<List<string>> ValidateFieldPermissionsAsync<TDto>(TDto dto, Dictionary<string, YetkiSeviyesi> userPermissions, TDto? originalDto = null, string? pagePermissionKey = null, string? userTcKimlikNo = null) where TDto : class;
        void RevertUnauthorizedFields<TDto>(TDto dto, TDto originalDto, List<string> unauthorizedFields) where TDto : class;
        Task RefreshCacheAsync();
    }
}
