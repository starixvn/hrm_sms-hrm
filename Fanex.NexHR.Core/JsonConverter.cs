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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class JsonConverter
    {
        /// <summary>
        /// JSON Method.
        /// </summary>
        public static class JsonExtensionMethod
        {
            public static string ToJson(object o)
            {
                var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                return jsonSerializer.Serialize(o);
            }

            public static T FromJson<T>(string s)
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                return serializer.Deserialize<T>(s);
            }

            public static List<T> FromJsonArray<T>(string s)
            {
                var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                return javaScriptSerializer.Deserialize<List<T>>(s);
            }
        }
    }
}
