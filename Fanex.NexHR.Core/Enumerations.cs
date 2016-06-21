// --------------------------------------------------------------------------
// <copyright file="Enumerations.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.30">Created</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Fanex.Common.Extensions;

    public static class Enumerations
    {
        /// <summary>
        /// Get ENUM Description.
        /// </summary>
        /// <param name="enumType">ENUM type.</param>
        /// <returns>String string value of an ENUM.</returns>
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
