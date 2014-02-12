// --------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.12">Created</change>
// </history>

namespace Fanex.NexHR.Core
{
    /// <summary>
    /// Constants Value.
    /// </summary>
    public static class Constants
    {
        public const int GeneralLeave = 0;

        public const int AnnualLeave = 2;

        public const int UnpaidLeave = 25;

        public const int BusinessTrip = 9;

        public const int WeddingLeave = 6;

        public const int NumberMonthsInFutureCanLeaveRequest = 4;

        /// <summary>
        /// Key name: WorkingTimeRequest_GetAllByRequestID.
        /// </summary>
        public const string WorkingTimeRequestGetAllByRequestID = "WorkingTimeRequest_GetAllByRequestID";

        /// <summary>
        /// Key name: WorkingTimeRequest_GetRequestByRequestDate_KeepInViewType.
        /// </summary>
        public const string WorkingTimeRequestGetRequestByRequestDateKeepInViewType = "WorkingTimeRequest_GetRequestByRequestDate_KeepInViewType";

        /// <summary>
        /// Key name: WorkingTimeRequest_GetRequestByCreateDate_KeepInViewType.
        /// </summary>
        public const string WorkingTimeRequestGetRequestByCreateDateKeepInViewType = "WorkingTimeRequest_GetRequestByCreateDate_KeepInViewType";

        /// <summary>
        /// Key name: WorkingTimeRequest_Delete.
        /// </summary>
        public const string WorkingTimeRequestDelete = "WorkingTimeRequest_Delete";

        /// <summary>
        /// Key name: WorkingTimeRequest_Insert_KeepInViewRequest.
        /// </summary>
        public const string WorkingTimeRequestInsertKeepInViewRequest = "WorkingTimeRequest_Insert_KeepInViewRequest";

        /// <summary>
        /// Key name: WorkingTimeRequest_Update_KeepInViewRequest.
        /// </summary>
        public const string WorkingTimeRequestUpdateKeepInViewRequest = "WorkingTimeRequest_Update_KeepInViewRequest";

        /// <summary>
        /// Key name: WorkingTimeRequest_GetRequestHistory.
        /// </summary>
        public const string WorkingTimeRequestGetRequestHistory = "WorkingTimeRequest_GetRequestHistory";
    }
}
