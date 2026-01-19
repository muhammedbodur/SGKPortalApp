using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SGKPortalApp.Common.Extensions;

namespace SGKPortalApp.Common.Helpers
{
    /// <summary>
    /// String işlemleri için yardımcı metodlar
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Akıllı NickName oluşturur (ZKTeco cihazlar için MAX 8 karakter):
        /// - İsim kelimelerin baş harfleri alınır
        /// - Son kelime kısaltılır
        /// - Nokta ile ayrılır
        /// Örnek: "Muhammed Bodur" => "M.BODUR" (7 char)
        /// Örnek: "Mehmet Ali Birand" => "M.A.BIR" (7 char)
        /// Örnek: "İl Müdürlüğü Görev Kartı" => "I.M.G.K" (7 char)
        /// </summary>
        public static string GenerateNickName(string input, int maxLength = 8)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Türkçe karakterleri temizle ve büyük harfe çevir
            var cleaned = input.RemoveTurkishCharacters().ToUpperInvariant();

            // Kelimelere ayır (boşluk, tire, alt çizgi ile)
            var words = cleaned.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return string.Empty;

            if (words.Length == 1)
            {
                // Tek kelime varsa direkt kullan (max 8 char)
                var singleWord = words[0];
                return singleWord.Length > maxLength ? singleWord.Substring(0, maxLength) : singleWord;
            }

            // Çok kelimeli: 8 karakter limiti için agresif kısaltma
            var parts = new List<string>();

            // İlk n-1 kelimenin baş harflerini al (sadece ilk harf)
            for (int i = 0; i < words.Length - 1; i++)
            {
                var word = words[i];
                if (word.Length > 0)
                {
                    // 8 karakter için sadece baş harf kullan
                    parts.Add(word.Substring(0, 1));
                }
            }

            // Son kelimeyi ekle (kısaltılmış)
            var lastWord = words[words.Length - 1];
            parts.Add(GetAbbreviationForShort(lastWord));

            // Nokta ile birleştir
            var result = string.Join(".", parts);

            // Max uzunlukta kes (gerekirse son kelimeyi daha da kısalt)
            if (result.Length > maxLength)
            {
                // Son kelimeyi kısaltarak tekrar dene
                parts[parts.Count - 1] = lastWord.Length > 2
                    ? lastWord.Substring(0, Math.Min(3, lastWord.Length))
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
        /// Kelime için uygun kısaltma döndürür (8 karakter limiti için)
        /// </summary>
        private static string GetAbbreviation(string word)
        {
            if (word.Length <= 3)
                return word; // Kısa kelimeler tam (IL, A, vb.)

            if (word.Length <= 5)
                return word.Substring(0, Math.Min(3, word.Length)); // Orta kelimeler 3 harf (MDR, GOR)

            // Uzun kelimeler için ilk 4 harf
            return word.Substring(0, Math.Min(4, word.Length));
        }

        /// <summary>
        /// 8 karakter limiti için daha agresif kısaltma
        /// </summary>
        private static string GetAbbreviationForShort(string word)
        {
            if (word.Length <= 3)
                return word; // Kısa kelimeler tam

            // Tüm kelimeler için max 3 harf
            return word.Substring(0, Math.Min(3, word.Length));
        }
    }
}
