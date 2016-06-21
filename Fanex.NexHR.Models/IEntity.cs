// --------------------------------------------------------------------------
// <copyright file="IEntity.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// IEntity Object.
    /// </summary>
    /// <typeparam name="TId">Type of object.</typeparam>
    public interface IEntity<TId>
    {
        TId Id { get; set; }

        string Name { get; set; }

        string Code { get; set; }
    }
}
