namespace Fanex.NexHR.Core.Helpers
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using Fanex.Common.Extensions;

    public static class EnumerationHelper
    {
        public static string GetDescription(Enum enumType)
        {
            var description = string.Empty;
            if (enumType != null)
            {
                FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                description = attributes.Length > 0 ? attributes[0].Description : enumType.ToString();
            }

            return description;
        }

        public static string GetDisplayName(Enum enumType)
        {
            var description = string.Empty;
            if (enumType != null)
            {
                FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
                DisplayAttribute[] attributes =
                    (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
                description = attributes.Length > 0 ? attributes[0].Description : enumType.ToString();
            }

            return description;
        }

        public static string GetStringValue(Enum enumType)
        {
            var description = string.Empty;
            if (enumType != null)
            {
                FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
                StringValueAttribute[] attributes =
                    (StringValueAttribute[])fi.GetCustomAttributes(typeof(StringValueAttribute), false);
                description = attributes.Length > 0 ? attributes[0].Value : enumType.ToString();
            }

            return description;
        }
    }
}