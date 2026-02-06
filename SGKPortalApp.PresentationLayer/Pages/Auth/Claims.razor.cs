using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.StateServices;
using System.Security.Claims;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    public partial class Claims
    {
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private PermissionStateService PermissionStateService { get; set; } = default!;

        private IEnumerable<Claim>? _claims;
        private Dictionary<string, int>? _permissionsDict;
        private IReadOnlyDictionary<string, YetkiSeviyesi>? _definedPermissions;
        private IReadOnlyDictionary<string, string>? _routeMap;
        private List<PermissionMatrixRow>? _permissionMatrix;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            _claims = authState.User?.Claims;

            var permissionsClaim = _claims?.FirstOrDefault(c => c.Type == "Permissions")?.Value;
            if (!string.IsNullOrEmpty(permissionsClaim))
            {
                try
                {
                    _permissionsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(permissionsClaim);
                }
                catch
                {
                }
            }

            var snapshot = PermissionStateService.GetCacheSnapshot();
            _definedPermissions = snapshot.DefinedPermissions;
            _routeMap = snapshot.RouteMap;

            BuildPermissionMatrix();
        }

        private string FormatJson(string json)
        {
            try
            {
                var obj = System.Text.Json.JsonSerializer.Deserialize<object>(json);
                return System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return json;
            }
        }

        private string GetLevelName(int level) => level switch
        {
            0 => "None",
            1 => "View",
            2 => "Edit",
            _ => level.ToString()
        };

        private string FormatLevel(YetkiSeviyesi? level)
            => level.HasValue ? GetLevelName((int)level.Value) : "-";

        private void BuildPermissionMatrix()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            var allKeys = new HashSet<string>(comparer);

            if (_definedPermissions != null)
                foreach (var key in _definedPermissions.Keys)
                    allKeys.Add(key);

            if (_permissionsDict != null)
                foreach (var key in _permissionsDict.Keys)
                    allKeys.Add(key);

            var routeLookup = new Dictionary<string, string>(comparer);
            if (_routeMap != null)
            {
                foreach (var kvp in _routeMap)
                {
                    if (!routeLookup.ContainsKey(kvp.Value))
                    {
                        routeLookup[kvp.Value] = kvp.Key;
                    }
                }
            }

            _permissionMatrix = allKeys
                .OrderBy(k => k, comparer)
                .Select(key => new PermissionMatrixRow
                {
                    Key = key,
                    Route = routeLookup.TryGetValue(key, out var route) ? route : null,
                    DefinedLevel = _definedPermissions != null && _definedPermissions.TryGetValue(key, out var defined) ? defined : (YetkiSeviyesi?)null,
                    UserLevel = _permissionsDict != null && _permissionsDict.TryGetValue(key, out var userLevelInt) ? (YetkiSeviyesi)userLevelInt : (YetkiSeviyesi?)null
                })
                .ToList();
        }

        private class PermissionMatrixRow
        {
            public string Key { get; set; } = string.Empty;
            public string? Route { get; set; }
            public YetkiSeviyesi? DefinedLevel { get; set; }
            public YetkiSeviyesi? UserLevel { get; set; }
        }
    }
}
