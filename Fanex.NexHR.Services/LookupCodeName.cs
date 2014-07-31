// --------------------------------------------------------------------------
// <copyright file="LookupCodeName.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// LookupCodeName Struct.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Reviewed. Suppression is OK here.")]
    public struct LookupCodeName
    {
        public const string JobTitle = "JT";
        public const string MemoCategory = "MC";
    }
}