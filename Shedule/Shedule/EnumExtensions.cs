using System;
using System.ComponentModel;

namespace Shedule
{
    public static class EnumExtensions
    {
        public static T ParseFromDescription<T>(this string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    is DescriptionAttribute attribute)
                {
                    if (attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException($"{description} не найден в enum {typeof(T).Name}");
        }
    }
}