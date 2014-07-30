// --------------------------------------------------------------------------
// <copyright file="JsonConverter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.15">Created</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Convert from JSON format to Object and vice versa.
    /// </summary>
    public static class JsonConverter
    {
        public static string ToJson(object o)
        {
            var jsonSerializer = new JavaScriptSerializer();

            return jsonSerializer.Serialize(o);
        }

        public static T FromJson<T>(string s)
        {
            var serializer = new JavaScriptSerializer();

            return serializer.Deserialize<T>(s);
        }

        public static List<T> FromJsonArray<T>(string s)
        {
            var javaScriptSerializer = new JavaScriptSerializer();

            return javaScriptSerializer.Deserialize<List<T>>(s);
        }
    }
}
