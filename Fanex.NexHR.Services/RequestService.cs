namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RequestService : BaseService, IRequestService
    {
        #region constant

        private const string _MsgLeave = "<span class='req leave'>{1}</span> {0} days";
        private const string _MsgLeaveHalfDay = "<span class='req leave'>{0} ({1}-{2})</span> 0.5 day";
        private const string _MsgLeaveOneDay = "<span class='req leave'>{0}</span> 1 day";
        private const string _MsgLeavePublicHoliday = "<span class='req leave'>{0}</span>";
        private const string _MsgChangeShiftOneDay = "<span class='req changeshift'>{0} <span class='arr'>&rarr;</span> {1}</span> {2}";
        private const string _MsgChangeShift = "<span class='req changeshift'>{0}</span>";
        private const string _MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";
        private const string _MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2} ({3})";
        private const string _MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2} ({3})";
        private const string _MsgKeepInView = "<span class='req keepinview'>{0}</span> {1}";
        private const string _MsgKeepInViewHalfday = "<span class='req keepinview'>{0} ({1} - {2})</span> {3}";

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

        #endregion constant

        private readonly IRequestRepository requestRepository;

        public RequestService()
        {
            requestRepository = new RequestRepository();
        }

        public RequestService(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;
        }


        public static string ToRequestMessage(RequestEntity request)
        {
            string message;
            switch (request.RequestType.Id)
            {
                case (int)RequestType.Leave:
                    message = GetMessageForLeave(request);
                    break;

                case (int)RequestType.ChangeShift:
                    message = GetMessageForChangeShift(request);
                    break;

                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    message = GetMessageForCheckInOut(request);
                    break;

                case (int)RequestType.LateIn:
                    var actualLateIn = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("+" + request.ActualLateInEarlyOut + "'");
                    message = string.Format(_MsgLateIn, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate), actualLateIn);
                    break;

                case (int)RequestType.EarlyOut:
                    var actualEarlyOut = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("-" + request.ActualLateInEarlyOut + "'");
                    message = string.Format(_MsgEarlyOut, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate), actualEarlyOut);
                    break;

                case (int)RequestType.KeepInView:
                    double totalDays = (double)request.TotalDays;
                    message = GetMessageForKeepInView(request, totalDays);
                    break;

                default:
                    message = string.Empty;
                    break;
            }

            return message;
        }

        private static string GetMessageForCheckInOut(RequestEntity request)
        {
            var time = request.RequestedDate.Day == request.FromDate.Day ? request.RequestedDate.ToShortTime() : (request.RequestedDate.ToShortTime() + "+");
            return string.Format(_MsgCheckInOut, time, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
        }

        private static string GetMessageForKeepInView(RequestEntity request, double totalDays)
        {
            if (totalDays.Equals(0.5))
            {
                return string.Format(
                    _MsgKeepInViewHalfday,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime(),
                    request.FromShift.Name);
            }

            return string.Format(_MsgKeepInView, request.FromDate.ToDateMonthYearFormatedDate(), request.FromShift.Name);
        }

        public static string GetMessageDateString(DateTime fromDate, DateTime toDate)
        {
            if ((toDate - fromDate).Days < 0)
            {
                return fromDate.ToDateMonthYearFormatedDate();
            }

            if (fromDate.Year != toDate.Year)
            {
                return fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
            }

            string result = string.Empty;
            result = (fromDate.Month != toDate.Month) ?
                fromDate.ToMonthAndDayFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate() :
                fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();

            return result;
        }

        public RequestEntity GetRequestById(GetRequestByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            RequestEntity request = null;
            if (criteria.IsValid())
            {
                request = requestRepository.GetRequestById(criteria);
            }

            return request;
        }

        public IList<RequestEntity> GetRequests(GetRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> entityList = null;
            if (criteria.IsValid())
            {
                entityList = requestRepository.GetRequests(criteria);
            }

            return entityList;
        }

        public decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            decimal number = 0;
            if (criteria.IsValid())
            {
                number = requestRepository.GetNumberRequestInMonth(criteria);
            }

            return number;
        }

        public IList<RequestEntity> GetHistoryRequest(GetHistoryRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<RequestEntity> histories = null;
            if (criteria.IsValid())
            {
                histories = requestRepository.GetHistoryRequest(criteria);
            }

            return histories;
        }

        public VerifierAndApproverForRequest GetVerifierAndApprover(GetVerifierAndApproverCriteria criteria)
        {
            VerifierAndApproverForRequest supervisor = new VerifierAndApproverForRequest();

            if (criteria != null && criteria.IsValid())
            {
                supervisor = requestRepository.GetVerifierAndApprover(criteria);
            }

            return supervisor;
        }

        public RequestEntity GetCheckInOutRequestToIgnoreById(GetCheckInOutRequestToIgnoreByIdCriteria criteria)
        {
            RequestEntity request = null;
            if (criteria != null && criteria.IsValid())
            {
                request = requestRepository.GetRequestToIgnoreById(criteria);
            }

            return request;
        }

        public RequestEntity GetlateInEarlyOutRequestToIgnoreById(GetLateInEarlyOutRequestToIgnoreByIdCriteria criteria)
        {
            RequestEntity request = null;
            if (criteria != null && criteria.IsValid())
            {
                request = requestRepository.GetRequestToIgnoreById(criteria);
            }

            return request;
        }

        public IList<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(GetChangeShiftDetailToIgnoreByIdCriteria criteria)
        {
            List<ChangeShiftDetailModel> details = GetData<ChangeShiftDetailModel>(criteria).ToList();
            return details;
        }

        public IList<LeaveDetailModel> GetLeaveRequestDetailToIgnoreById(GetLeaveRequestDetailToIgnoreByIdCriteria criteria)
        {
            IList<LeaveDetailModel> leaveDetails = GetData<LeaveDetailModel>(criteria).ToList();
            return leaveDetails;
        }

        protected static string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            if (totalDays > 1)
            {
                return string.Format(_MailContentLeave, fromDate.ToDateMonthYearFormatedDate(), toDate.ToDateMonthYearFormatedDate(), totalDays);
            }

            if (totalDays == 1)
            {
                return string.Format(_MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
            }

            return string.Format(_MailContentLeaveHalfDay, requestedDate.ToDateMonthYearFormatedDate(), fromDate.ToShortTime(), toDate.ToShortTime());
        }

        private static string GetMessageForLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;

            messageDetail = (request.LeaveType != null && request.LeaveType.Id == (int)LeaveTypes.GeneralLeave) ?
                            GetMessageForGeneralLeave(request) : GetMessageForOtherLeave(request);

            return messageDetail;
        }

        private static string GetMessageForChangeShift(RequestEntity request)
        {
            var messageDetail = string.Empty;
            if (request.FromDate.Date == request.ToDate.Date)
            {
                messageDetail = string.Format(
                        _MsgChangeShiftOneDay,
                        request.FromShift.Name,
                        request.ToShift.Name,
                        request.FromDate.ToDateMonthYearFormatedDate());
            }
            else
            {
                var date = GetMessageDateString(request.FromDate, request.ToDate);
                messageDetail = string.Format(_MsgChangeShift, date);
            }

            return messageDetail;
        }

        private static string GetMessageForGeneralLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;
            var leaveDate = GetMessageDateString(request.FromDate, request.ToDate);

            if (request.TotalDays >= 1)
            {
                messageDetail = string.Format("<span class='req leave'>{0}</span> {1}", leaveDate, request.LeaveCountDetails);
            }

            if ((request.TotalDays - (decimal)0.5) == 0)
            {
                messageDetail = string.Format(
                "<span class='req leave'>{0} ({1}-{2})</span> {3}",
                leaveDate,
                request.FromTime.ToShortTime(),
                request.ToTime.ToShortTime(),
                request.LeaveCountDetails);
            }

            return messageDetail;
        }

        private static string GetMessageForOtherLeave(RequestEntity request)
        {
            var messageDetail = string.Empty;
            var leaveDate = GetMessageDateString(request.FromDate, request.ToDate);

            if (request.TotalDays > 1)
            {
                messageDetail = string.Format(_MsgLeave, request.TotalDays, leaveDate);
            }
            else if (request.TotalDays == 1)
            {
                messageDetail = string.Format(_MsgLeaveOneDay, leaveDate);
            }
            else if ((request.TotalDays - (decimal)0.5) == 0)
            {
                messageDetail = string.Format(
                _MsgLeaveHalfDay, leaveDate, request.FromTime.ToShortTime(), request.ToTime.ToShortTime());
            }
            else
            {
                messageDetail = string.Format(_MsgLeavePublicHoliday, leaveDate);
            }

            return messageDetail;
        }
    }
}