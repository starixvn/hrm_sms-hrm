namespace Fanex.NexHR.Core.Constants
{
    public static class Constants
    {
        public const string JobTitle = "JT";

        public const string MemoCategory = "MC";

        public const int GeneralLeave = 0;

        public const int AnnualLeave = 2;

        public const int UnpaidLeave = 25;

        public const int UnpaidLeaveInsurance = 41;

        public const int BusinessTrip = 9;

        public const int MaternityLeave = 12;

        public const int WeddingLeave = 6;

        public const int CompensationLeave = 27;

        public const int NumberMonthsInFutureCanLeaveRequest = 7;

        public const string PassWordAdmin = "@dm!nhrms";

        public const string DateFormat = "dd MMM, yyyy";

        public const string TimeFormat = "HH:mm";

        public const decimal MaxScoreSubtractDate = (decimal)3.5;

        public const int CurrentPage = 1;

        public const int ItemsPerPage = 50;

        public const string BeginRootTag = "<ROOT>";

        public const string EndRootTag = "</ROOT>";

        public const string WorkingTimeRequestGetAllByRequestId = "WorkingTimeRequest_GetAllByRequestID";

        public const string WorkingTimeRequestGetRequestByRequestDateKeepInViewType = "WorkingTimeRequest_GetRequestByRequestDate_KeepInViewType";

        public const string WorkingTimeRequestGetRequestByCreateDateKeepInViewType = "WorkingTimeRequest_GetRequestByCreateDate_KeepInViewType";

        public const string WorkingTimeRequestDelete = "WorkingTimeRequest_Delete";

        public const string WorkingTimeRequestInsertKeepInViewRequest = "WorkingTimeRequest_Insert_KeepInViewRequest";

        public const string WorkingTimeRequestUpdateKeepInViewRequest = "WorkingTimeRequest_Update_KeepInViewRequest";

        public const string WorkingTimeRequestGetRequestHistory = "WorkingTimeRequest_GetRequestHistory";

        public const int TotalWeekendLeaveLimit = 16;

        public const int OffDayShiftId = -1;

        public const string ProfileUploadPath = "/file/uploads/profile/";

        public const string RedColor = "#ff0000";
        public const string OffDayBackgroundColor = "#f8f89c";
        public const string HolidayBackgroundColor = "#f3aa4b";
    }
}