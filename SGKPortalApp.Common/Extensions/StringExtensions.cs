using System.Globalization;
using System.Text;

namespace SGKPortalApp.Common.Extensions
{
    /// <summary>
    /// String extension metodları - Türkçe karakter desteği ile
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Türkçe karakterleri İngilizce karşılıklarına dönüştürür
        /// İ -> I, ı -> i, ö -> o, ü -> u, ş -> s, ğ -> g, ç -> c
        /// </summary>
        public static string ToNormalizedString(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace('İ', 'I')
                .Replace('ı', 'i')
                .Replace('Ş', 'S')
                .Replace('ş', 's')
                .Replace('Ğ', 'G')
                .Replace('ğ', 'g')
                .Replace('Ü', 'U')
                .Replace('ü', 'u')
                .Replace('Ö', 'O')
                .Replace('ö', 'o')
                .Replace('Ç', 'C')
                .Replace('ç', 'c');
        }

        /// <summary>
        /// Türkçe karakter duyarsız arama yapar
        /// "TİRE" içinde "tire" aratıldığında bulur
        /// </summary>
        public static bool ContainsTurkish(this string source, string search, StringComparison ordinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(search))
                return false;

            var normalizedSource = source.ToNormalizedString().ToLowerInvariant();
            var normalizedSearch = search.ToNormalizedString().ToLowerInvariant();

            return normalizedSource.Contains(normalizedSearch);
        }

        /// <summary>
        /// Türkçe karakter duyarsız eşitlik kontrolü
        /// </summary>
        public static bool EqualsTurkish(this string source, string compare)
        {
            if (source == null && compare == null) return true;
            if (source == null || compare == null) return false;

            var normalizedSource = source.ToNormalizedString().ToLowerInvariant();
            var normalizedCompare = compare.ToNormalizedString().ToLowerInvariant();

            return normalizedSource == normalizedCompare;
        }

        /// <summary>
        /// Türkçe karakter duyarsız başlangıç kontrolü
        /// </summary>
        public static bool StartsWithTurkish(this string source, string prefix)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(prefix))
                return false;

            var normalizedSource = source.ToNormalizedString().ToLowerInvariant();
            var normalizedPrefix = prefix.ToNormalizedString().ToLowerInvariant();

            return normalizedSource.StartsWith(normalizedPrefix);
        }

        /// <summary>
        /// Türkçe karakter duyarsız bitiş kontrolü
        /// </summary>
        public static bool EndsWithTurkish(this string source, string suffix)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(suffix))
                return false;

            var normalizedSource = source.ToNormalizedString().ToLowerInvariant();
            var normalizedSuffix = suffix.ToNormalizedString().ToLowerInvariant();

            return normalizedSource.EndsWith(normalizedSuffix);
        }

        /// <summary>
        /// Türkçe karakterleri kaldırır ve sadece ASCII karakterler bırakır
        /// </summary>
        public static string ToAsciiOnly(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.ToNormalizedString()
                .Where(c => c < 128)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                .ToString();
        }
    }
}