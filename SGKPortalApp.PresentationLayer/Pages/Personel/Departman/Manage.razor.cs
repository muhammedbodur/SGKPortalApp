using Microsoft.AspNetCore.Components;

namespace SGKPortalApp.PresentationLayer.Pages.Personel.Departman
{
    public partial class Manage
    {
        [Parameter] public int Id { get; set; }

        protected override bool IsEditMode => true;
    }
}
