namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    /// <summary>
    /// Sayfalanmış veri response DTO
    /// </summary>
    /// <typeparam name="T">Liste item tipi</typeparam>
    public class PagedResponseDto<T>
    {
        /// <summary>
        /// Veri listesi
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Toplam kayıt sayısı
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Sayfa numarası
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Sayfa boyutu
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Toplam sayfa sayısı
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Bir sonraki sayfa var mı?
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Bir önceki sayfa var mı?
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Başlangıç index (0-based)
        /// </summary>
        public int StartIndex => (PageNumber - 1) * PageSize;

        /// <summary>
        /// Bitiş index (0-based)
        /// </summary>
        public int EndIndex => Math.Min(StartIndex + PageSize - 1, TotalCount - 1);
    }
}