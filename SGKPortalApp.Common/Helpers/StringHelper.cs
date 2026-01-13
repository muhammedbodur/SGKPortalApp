using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SGKPortalApp.Common.Helpers
{
    /// <summary>
    /// String işlemleri için yardımcı metodlar
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Akıllı NickName oluşturur:
        /// - İsim kelimelerin baş harfleri alınır
        /// - Son kelime tam yazılır
        /// - Nokta ile ayrılır
        /// Örnek: "Muhammed Bodur" => "M.BODUR"
        /// Örnek: "Mehmet Ali Birand" => "M.A.BIRAND"
        /// Örnek: "İl Müdürlüğü Görev Kartı" => "IL.MDR.GOR"
        /// </summary>
        public static string GenerateNickName(string input, int maxLength = 12)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Türkçe karakterleri temizle ve büyük harfe çevir
            var cleaned = RemoveTurkishCharacters(input).ToUpperInvariant();

            // Kelimelere ayır (boşluk, tire, alt çizgi ile)
            var words = cleaned.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return string.Empty;

            if (words.Length == 1)
            {
                // Tek kelime varsa direkt kullan (max 12 char)
                var singleWord = words[0];
                return singleWord.Length > maxLength ? singleWord.Substring(0, maxLength) : singleWord;
            }

            // Çok kelimeli: İlk kelimelerin baş harfleri + son kelime
            var parts = new List<string>();

            // İlk n-1 kelimenin baş harflerini al
            for (int i = 0; i < words.Length - 1; i++)
            {
                var word = words[i];
                if (word.Length > 0)
                {
                    // Kısa kelimeler (2-3 harf) tam yazılabilir
                    if (word.Length <= 3)
                    {
                        parts.Add(word);
                    }
                    else
                    {
                        // Uzun kelimeler için baş harf veya kısaltma
                        parts.Add(GetAbbreviation(word));
                    }
                }
            }

            // Son kelimeyi ekle (tam veya kısaltılmış)
            var lastWord = words[words.Length - 1];
            parts.Add(GetAbbreviation(lastWord));

            // Nokta ile birleştir
            var result = string.Join(".", parts);

            // Max uzunlukta kes (gerekirse son kelimeyi kısalt)
            if (result.Length > maxLength)
            {
                // Son kelimeyi kısaltarak tekrar dene
                parts[parts.Count - 1] = lastWord.Length > 4
                    ? lastWord.Substring(0, Math.Min(4, lastWord.Length))
                    : lastWord.Substring(0, 1);
                result = string.Join(".", parts);

                // Hala uzunsa, sert kes
                if (result.Length > maxLength)
                {
                    result = result.Substring(0, maxLength);
                }
            }

            return result;
        }

        /// <summary>
        /// Kelime için uygun kısaltma döndürür
        /// Kısa kelimeler tam, uzun kelimeler baş harf veya ilk birkaç harf
        /// </summary>
        private static string GetAbbreviation(string word)
        {
            if (word.Length <= 3)
                return word; // Kısa kelimeler tam (IL, A, vb.)

            if (word.Length <= 5)
                return word.Substring(0, Math.Min(3, word.Length)); // Orta kelimeler 3 harf (MDR, GOR)

            // Uzun kelimeler için ilk 4 harf veya baş harf
            return word.Substring(0, Math.Min(4, word.Length));
        }

        /// <summary>
        /// Türkçe karakterleri İngilizce karşılıklarına çevirir (büyük/küçük harf korunur)
        /// </summary>
        public static string RemoveTurkishCharacters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var turkishChars = "ığüşöçİĞÜŞÖÇ";
            var englishChars = "igusocIGUSOC";

            var sb = new StringBuilder(input);
            for (int i = 0; i < turkishChars.Length; i++)
            {
                sb.Replace(turkishChars[i], englishChars[i]);
            }

            return sb.ToString();
        }
    }
}
