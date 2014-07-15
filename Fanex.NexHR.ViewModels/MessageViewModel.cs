// -----------------------------------------------------------------------
// <copyright file="MessageViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;

    /// <summary>
    /// The Message view model.
    /// </summary>
    public class MessageViewModel
    {
        public int MessageId { get; set; }

        public string Subject { get; set; }

        public int Category { get; set; }

        public DateTime CreatedDate { get; set; }

        public string SendTo { get; set; }
    }
}
