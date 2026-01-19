namespace SGKPortalApp.PresentationLayer.Models.FormModels.PdksIslemleri
{
    public class IzinSorumluFormModel
    {
        public int IzinSorumluId { get; set; }
        public int? DepartmanId { get; set; }
        public int? ServisId { get; set; }
        public string SorumluPersonelTcKimlikNo { get; set; } = string.Empty;
        public int OnaySeviyesi { get; set; } = 1;
        public bool Aktif { get; set; } = true;
        public string? Aciklama { get; set; }
    }
}
