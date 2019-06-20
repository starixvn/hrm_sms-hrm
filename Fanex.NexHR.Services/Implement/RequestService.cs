namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;

    public class RequestService : IRequestService
    {
        private const decimal HalfDayValue = (decimal)0.5;
        private const int OneHourValue = 60;

        private readonly IRequestRepository requestRepository;
        private readonly IBaseService baseService;

        public RequestService(
            IRequestRepository requestRepository,
            IBaseService baseService)
        {
            this.requestRepository = requestRepository;
            this.baseService = baseService;
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
            RequestEntity request = null;
            if (criteria.IsValid())
            {
                request = requestRepository.GetRequestById(criteria);
            }

            return request;
        }

        public IList<RequestEntity> GetRequests(GetRequestCriteria criteria)
        {
            IList<RequestEntity> entityList = null;
            if (criteria.IsValid())
            {
                entityList = requestRepository.GetRequests(criteria);
            }

            return entityList;
        }

        public decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria)
        {
            decimal number = 0;
            if (criteria.IsValid())
            {
                number = requestRepository.GetNumberRequestInMonth(criteria);
            }

            return number;
        }

        public IList<RequestEntity> GetHistoryRequest(GetHistoryRequestCriteria criteria)
        {
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
            return baseService.GetData<ChangeShiftDetailModel>(criteria).ToList();
        }

        public IList<LeaveDisciplineModel> GetLeaveRequestDetailToIgnoreById(GetLeaveRequestDetailToIgnoreByIdCriteria criteria)
        {
            IList<LeaveDisciplineModel> disciplines = null;
            if (criteria != null && criteria.IsValid())
            {
                disciplines = requestRepository.GetLeaveRequestToIgnoreById(criteria);
            }

            return disciplines ?? new List<LeaveDisciplineModel>();
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
            string messageDetail = request.KeepInViewType == (int)KeepInViewTypes.KeepInViewByDay
                ? GetMessageForKeepInViewByDay(request, isNoFormat)
                : GetMessageForKeepInViewByHour(request, isNoFormat);

            return messageDetail;
        }


        private static string GetMessageForKeepInViewByDay(RequestEntity request, bool isNoFormat)
        {
            string msgTemplate;
            string messageDetail;

            if (request.TotalDays.Equals(HalfDayValue))
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewHalfdayWithNoFormat : RequestFormatConstants.MsgKeepInViewHalfday;
                messageDetail = string.Format(
                    msgTemplate,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime());
            }
            else
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewOneDayWithNoFormat : RequestFormatConstants.MsgKeepInViewOneDay;
                messageDetail = string.Format(msgTemplate, request.FromDate.ToDateMonthYearFormatedDate());
            }

            return messageDetail;
        }

        private static string GetMessageForKeepInViewByHour(RequestEntity request, bool isNoFormat)
        {
            string msgTemplate;
            string messageDetail;

            var minutes = (request.ToTime - request.FromTime).TotalMinutes;
            var hours = minutes / 60.0;
            if (minutes > OneHourValue)
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewByHoursWithNoFormat : RequestFormatConstants.MsgKeepInViewByHours;
                messageDetail = string.Format(
                    msgTemplate,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime(),
                    hours.ToString("0.#"));
            }
            else
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewByHourWithNoFormat : RequestFormatConstants.MsgKeepInViewByHour;
                messageDetail = string.Format(
                    msgTemplate,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime(),
                    hours.ToString("0.#"));
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