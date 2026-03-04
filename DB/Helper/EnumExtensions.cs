using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DB.Helper
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            var displayAttribute = member?
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? value.ToString();
        }

        public static List<DropdownItem> GetDropdownFromEnum<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new DropdownItem
                {
                    Id = Convert.ToInt32(e),
                    Name = GetDisplayName(e)
                })
                .ToList();
        }

        public static string GetDisplayNameFromInt<TEnum>(int value) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                return string.Empty;

            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), value);

            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();

            return attribute?.Name ?? enumValue.ToString();
        }



    }
}
