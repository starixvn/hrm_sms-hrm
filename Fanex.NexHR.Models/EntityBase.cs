// --------------------------------------------------------------------------
// <copyright file="EntityBase.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    public class EntityBase<TId> : IEntity<TId>
    {
        public TId Id { get; set; }

        public string Name { get; set; }

        public virtual string Code { get; set; }
    }
}