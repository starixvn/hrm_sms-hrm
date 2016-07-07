namespace Fanex.NexHR.Core
{
    using Fanex.NexHR.Core.Enums;

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

        public const string WorkingTimeRequestGetAllByRequestID = "WorkingTimeRequest_GetAllByRequestID";

        public const string WorkingTimeRequestGetRequestByRequestDateKeepInViewType = "WorkingTimeRequest_GetRequestByRequestDate_KeepInViewType";

        public const string WorkingTimeRequestGetRequestByCreateDateKeepInViewType = "WorkingTimeRequest_GetRequestByCreateDate_KeepInViewType";

        public const string WorkingTimeRequestDelete = "WorkingTimeRequest_Delete";

        public const string WorkingTimeRequestInsertKeepInViewRequest = "WorkingTimeRequest_Insert_KeepInViewRequest";

        public const string WorkingTimeRequestUpdateKeepInViewRequest = "WorkingTimeRequest_Update_KeepInViewRequest";

        public const string WorkingTimeRequestGetRequestHistory = "WorkingTimeRequest_GetRequestHistory";
    }
}