// -----------------------------------------------------------------------
// <copyright file="Message.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    /// <summary>
    /// The message model.
    /// </summary>
    public class Message
    {
        public int MessageId { get; set; }

        public string Subject { get; set; }

        public string Contents { get; set; }

        public int Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string SentTo { get; set; }
    }
}
