// --------------------------------------------------------------------------
// <copyright file="ManpowerCategoryModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.4">Created</change>
// </history>

namespace Fanex.NexHR.Models.Report
{
    public class ManpowerCategoryModel
    {
        public string GroupCode { get; set; }

        public string GroupName { get; set; }

        public int Total { get; set; }

        public int TotalExpat { get; set; }

        public int TotalLocal { get; set; }

        public int Management { get; set; }

        public int ExecutiveWithPosition { get; set; }

        public int Executive { get; set; }

        public int Staff { get; set; }

        public int Worker { get; set; }
    }
}
