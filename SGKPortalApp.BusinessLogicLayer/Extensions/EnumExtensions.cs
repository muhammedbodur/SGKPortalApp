using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SGKPortalApp.BusinessLogicLayer.Extensions
{
    /// <summary>
    /// Enum extension metodları
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Enum değerinin Display attribute'ündeki Name değerini döndürür
        /// </summary>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        /// <summary>
        /// Enum değerinin Display attribute'ündeki Description değerini döndürür
        /// </summary>
        public static string? GetDisplayDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return null;

            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Description;
        }
    }
}
