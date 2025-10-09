using System.Text.RegularExpressions;

namespace SGKPortalApp.BusinessObjectLayer.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseErrorType ErrorType { get; set; }
        public string UserFriendlyMessage { get; set; }
        public string? ConstraintName { get; set; }

        public DatabaseException(string message, DatabaseErrorType errorType, string? constraintName = null) 
            : base(message)
        {
            ErrorType = errorType;
            ConstraintName = constraintName;
            UserFriendlyMessage = GetUserFriendlyMessage(errorType, constraintName);
        }

        private string GetUserFriendlyMessage(DatabaseErrorType errorType, string? constraintName)
        {
            return errorType switch
            {
                DatabaseErrorType.ForeignKeyViolation => GetForeignKeyMessage(constraintName),
                DatabaseErrorType.UniqueConstraintViolation => GetUniqueConstraintMessage(constraintName),
                DatabaseErrorType.NullConstraintViolation => "Zorunlu alanlar boş bırakılamaz",
                DatabaseErrorType.CheckConstraintViolation => "Geçersiz veri girişi",
                _ => "Veritabanı işlemi sırasında bir hata oluştu"
            };
        }

        private string GetForeignKeyMessage(string? constraintName)
        {
            if (string.IsNullOrEmpty(constraintName))
                return "İlişkili kayıt bulunamadı. Lütfen gerekli alanları kontrol edin.";

            // Constraint adından otomatik field adı çıkar
            var fieldName = ExtractFieldNameFromConstraint(constraintName);
            return $"{fieldName} seçimi gerekli veya geçersiz";
        }

        private string GetUniqueConstraintMessage(string? constraintName)
        {
            if (string.IsNullOrEmpty(constraintName))
                return "Bu kayıt zaten mevcut";

            // Constraint adından otomatik field adı çıkar
            var fieldName = ExtractFieldNameFromConstraint(constraintName);
            return $"Bu {fieldName} ile kayıtlı veri zaten mevcut";
        }

        /// <summary>
        /// Constraint adından otomatik olarak field adını çıkarır
        /// Örnek: FK_PER_Personeller_CMN_HizmetBinalari → Hizmet Binası
        /// </summary>
        private string ExtractFieldNameFromConstraint(string constraintName)
        {
            // 1. Son kelimeyi al: FK_PER_Personeller_CMN_HizmetBinalari → HizmetBinalari
            var parts = constraintName.Split('_');
            var fieldName = parts.LastOrDefault() ?? constraintName;
            
            // 2. "Id" suffix'ini kaldır: DepartmanId → Departman
            if (fieldName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                fieldName = fieldName.Substring(0, fieldName.Length - 2);
            
            // 3. Çoğul eklerini kaldır: Departmanlar → Departman
            fieldName = RemovePluralSuffix(fieldName);
            
            // 4. PascalCase → Title Case: HizmetBinasi → Hizmet Binasi
            fieldName = ConvertPascalCaseToTitleCase(fieldName);
            
            // 5. Türkçe karakter düzeltmeleri (otomatik)
            fieldName = FixTurkishCharacters(fieldName);
            
            return fieldName;
        }

        /// <summary>
        /// Türkçe çoğul eklerini kaldırır
        /// </summary>
        private string RemovePluralSuffix(string text)
        {
            // Türkçe çoğul ekleri: -lari, -leri, -lar, -ler (uzundan kısaya)
            var pluralSuffixes = new[] { "lari", "leri", "lar", "ler" };
            
            foreach (var suffix in pluralSuffixes)
            {
                if (text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    return text.Substring(0, text.Length - suffix.Length);
            }
            
            return text;
        }

        /// <summary>
        /// PascalCase'i Title Case'e çevirir
        /// Örnek: HizmetBinasi → Hizmet Binasi
        /// </summary>
        private string ConvertPascalCaseToTitleCase(string pascalCase)
        {
            // Büyük harflerden önce boşluk ekle
            return Regex.Replace(pascalCase, "([A-Z])", " $1").Trim();
        }

        /// <summary>
        /// Türkçe karakter düzeltmelerini otomatik yapar
        /// </summary>
        private string FixTurkishCharacters(string text)
        {
            // Kelime başındaki I → İ (sonraki harf küçükse)
            if (text.StartsWith("I") && text.Length > 1 && char.IsLower(text[1]))
                text = "İ" + text.Substring(1);
            
            // Kelime içindeki " I" → " İ" (boşluktan sonra gelen I, sonraki harf küçükse)
            text = Regex.Replace(text, @"\s+I([a-z])", m => " İ" + m.Groups[1].Value);
            
            // "Unvan" → "Ünvan"
            text = text.Replace("Unvan", "Ünvan");
            
            return text;
        }
    }

    public enum DatabaseErrorType
    {
        ForeignKeyViolation,
        UniqueConstraintViolation,
        NullConstraintViolation,
        CheckConstraintViolation,
        Unknown
    }
}
