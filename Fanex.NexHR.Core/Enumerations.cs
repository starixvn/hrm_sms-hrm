namespace Fanex.NexHR.Core
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Fanex.Common.Extensions;

    public static class Enumerations
    {
        public static string GetStringValue(Enum enumType)
        {
            FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
            StringValueAttribute[] attributes = (StringValueAttribute[])fi.GetCustomAttributes(typeof(StringValueAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Value;
            }
            else
            {
                return enumType.ToString();
            }
        }
    }
}
