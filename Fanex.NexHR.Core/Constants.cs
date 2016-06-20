namespace Fanex.NexHR.Core
{
    public static class Constants
    {
        public const string JobTitle = "JT";

        public const string MemoCategory = "MC";

        public const int GeneralLeave = 0;

        public const int AnnualLeave = 2;

        public const int UnpaidLeave = 25;

        public const int BusinessTrip = 9;

        public const int MaternityLeave = 12;

        public const int WeddingLeave = 6;

        public const int NumberMonthsInFutureCanLeaveRequest = 4;

        public const string PassWordAdmin = "@dm!nhrms";

        public const string DateFormat = "dd MMM, yyyy";

        public const string TimeFormat = "HH:mm";

        public const decimal MaxScoreSubtractDate = (decimal)3.5;

        public const int CurrentPage = 1;

        public const int ItemsPerPage = 50;

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