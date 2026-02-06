using Microsoft.AspNetCore.Components;

namespace SGKPortalApp.PresentationLayer.Pages.Common.Ilce
{
    public partial class Manage
    {
        [Parameter] public int Id { get; set; }

        protected override bool IsEditMode => true;
    }
}
