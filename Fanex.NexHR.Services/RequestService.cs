namespace Fanex.NexHR.Services
{
    using Core.Services;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
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
        private readonly IRequestRepository requestRepository;

        public RequestService()
        {
            requestRepository = new RequestRepository();
        }

        public RequestService(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;
        }


        public static string ToRequestMessage(RequestEntity request, bool isNoFormat = false)
        {
            string message;
            switch (request.RequestType.Id)
            {
                case (int)RequestType.Leave:
                    message = GetMessageForLeave(request, isNoFormat);
                    break;

                case (int)RequestType.ChangeShift:
                    message = GetMessageForChangeShift(request, isNoFormat);
                    break;

                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    message = GetMessageForCheckInOut(request, isNoFormat);
                    break;

                case (int)RequestType.LateIn:
                    message = GetMessageForLateIn(request, isNoFormat);
                    break;

                case (int)RequestType.EarlyOut:
                    message = GetMessageForEarlyOut(request, isNoFormat);
                    break;

                case (int)RequestType.KeepInView:
                    message = GetMessageForKeepInView(request, isNoFormat);
                    break;

                default:
                    message = string.Empty;
                    break;
            }

            return message;
        }

        public static string GetMessageDateString(DateTime fromDate, DateTime toDate)
        {
            string result = string.Empty;

            if ((toDate - fromDate).Days <= 0)
            {
                result = fromDate.ToDateMonthYearFormatedDate();
            }
            else if (fromDate.Year != toDate.Year)
            {
                result = fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                result = (fromDate.Month != toDate.Month) ?
                fromDate.ToMonthAndDayFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate() :
                fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
            }

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

        private static string GetMessageForLeave(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgLeaveWithNoFormat : RequestFormatConstants.MsgLeave;
            var leaveDateTime = GetDateTimeForLeaveRequest(request);
            var leaveDayDetail = GetLeaveDayDetail(request);
            var messageDetail = string.Format(msgTemplate, leaveDateTime, leaveDayDetail);

            return messageDetail;
        }

        private static string GetDateTimeForLeaveRequest(RequestEntity request)
        {
            var leaveDate = GetMessageDateString(request.FromDate, request.ToDate);

            if ((request.TotalDays - (decimal)0.5) == 0)
            {
                leaveDate = string.Format(RequestFormatConstants.MsgLeaveHalfDayWithNoFormat, leaveDate, request.FromTime.ToShortTime(), request.ToTime.ToShortTime());
            }

            return leaveDate;
        }

        private static string GetMessageForChangeShift(RequestEntity request, bool isNoFormat)
        {
            string msgTemplate;
            string messageDetail;
            if (request.FromDate.Date == request.ToDate.Date)
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgChangeShiftOneDayWithNoFormat : RequestFormatConstants.MsgChangeShiftOneDay;
                messageDetail = string.Format(
                        msgTemplate,
                        request.FromShift.Name,
                        request.ToShift.Name,
                        request.FromDate.ToDateMonthYearFormatedDate());
            }
            else
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgChangeShiftWithNoFormat : RequestFormatConstants.MsgChangeShift;
                var date = GetMessageDateString(request.FromDate, request.ToDate);
                messageDetail = string.Format(msgTemplate, date);
            }

            return messageDetail;
        }

        private static string GetMessageForCheckInOut(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgCheckInOutWithNoFormat : RequestFormatConstants.MsgCheckInOut;
            var time = request.RequestedDate.Day == request.FromDate.Day ? request.RequestedDate.ToShortTime() : (request.RequestedDate.ToShortTime() + "+");
            return string.Format(msgTemplate, time, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
        }

        private static string GetMessageForLateIn(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgLateInWithNoFormat : RequestFormatConstants.MsgLateIn;
            var actualLateIn = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("+" + request.ActualLateInEarlyOut + "'");
            
            return string.Format(msgTemplate, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate), actualLateIn);
        }

        private static string GetMessageForEarlyOut(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgEarlyOutWithNoFormat : RequestFormatConstants.MsgEarlyOut;
            var actualEarlyOut = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("-" + request.ActualLateInEarlyOut + "'");

            return string.Format(msgTemplate, request.TotalMins, request.FromShift.Name, GetMessageDateString(request.FromDate, request.ToDate), actualEarlyOut);
        }

        private static string GetMessageForKeepInView(RequestEntity request, bool isNoFormat)
        {
            string msgTemplate;
            string messageDetail;
            if (request.TotalDays.Equals(0.5))
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewHalfdayWithNoFormat : RequestFormatConstants.MsgKeepInViewHalfday;
                messageDetail = string.Format(
                    msgTemplate,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime(),
                    request.FromShift.Name);
            }
            else
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewWithNoFormat : RequestFormatConstants.MsgKeepInView;
                messageDetail = string.Format(msgTemplate, request.FromDate.ToDateMonthYearFormatedDate(), request.FromShift.Name);
            }

            return messageDetail;
        }

        private static string GetLeaveDayDetail(RequestEntity request)
        {
            return (request.LeaveType != null && request.LeaveType.Id == (int)LeaveTypes.GeneralLeave) ?
                            request.LeaveCountDetails : GetTotalDay(request.TotalDays);
        }

        private static string GetTotalDay(decimal totalDays)
        {
            var messageDetail = string.Empty;

            if (totalDays > 1)
            {
                messageDetail = string.Format(RequestFormatConstants.MsgTotalLeaveDays, totalDays.ToString("0.#"));
            }
            else if (totalDays > 0)
            {
                messageDetail = string.Format(RequestFormatConstants.MsgTotalLeaveDay, totalDays.ToString("0.#"));
            }
            else
            {
                messageDetail = string.Empty;
            }

            return messageDetail;
        }
    }
}
