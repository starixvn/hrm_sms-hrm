// --------------------------------------------------------------------------
// <copyright file="OutputAttribute.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.4">Created</change>
// </history>

namespace Fanex.NexHR.Core.Attributes
{
    using System;

    /// <summary>
    /// Output Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OutputAttribute : Attribute
    {
        public OutputAttribute()
        {
        }
    }
}
