// --------------------------------------------------------------------------
// <copyright file="KeepInViewRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Data;
    using Fanex.Data;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Request Repository.
    /// </summary>
    public class KeepInViewRequestRepository : BaseRequestRepository, IKeepInViewRequestRepository
    {
        /// <summary>
        /// Inserts the late in early out request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Request's id.</returns>
        public override long Insert(object model, out int errorCode, out string message)
        {
            var request = (KeepInViewRequestViewModel)model;
            DateTime? fromTime = null;
            DateTime? toTime = null;
            this.ConvertFromTimeAndToTimeFromStringFormatToDateTimeFormat(request, ref fromTime, ref toTime);
            using (var db = new ObjectDb(Constants.WorkingTimeRequestInsertKeepInViewRequest))
            {
                db.AddInParameter("@EmpId", DbType.Int32, request.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, (int)RequestTypes.KeepInView);
                db.AddInParameter("@Remark", DbType.String, request.Remark);
                db.AddInParameter("@OnDate", DbType.DateTime, request.Date);
                db.AddInParameter("@IsHalfDay", DbType.Boolean, request.HalfDay);
                db.AddInParameter("@ShiftId", DbType.Int32, request.ShiftId);
                db.AddInParameter("@FromTime", DbType.DateTime, fromTime);
                db.AddInParameter("@ToTime", DbType.DateTime, toTime);
                db.AddInParameter("@CreatedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@CreatedBy", DbType.Int32, request.By);
                db.AddInParameter("@VerifierId", DbType.Int32, request.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, request.ApproverId);
                db.AddInParameter("@RefNo", DbType.String, request.RefNo);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                var requestId = result == null ? -1 : Convert.ToInt32(result);
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return requestId;
            }
        }

        /// <summary>
        /// Updates the late in early out request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Result update.</returns>
        public override bool Update(object model, out int errorCode, out string message)
        {
            var request = (KeepInViewRequestViewModel)model;
            DateTime? fromTime = null;
            DateTime? toTime = null;
            this.ConvertFromTimeAndToTimeFromStringFormatToDateTimeFormat(request, ref fromTime, ref toTime);

            using (var db = new ObjectDb(Constants.WorkingTimeRequestUpdateKeepInViewRequest))
            {
                db.AddInParameter("@RequestId", DbType.Int64, request.Id);
                db.AddInParameter("@EmpId", DbType.Int32, request.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, (int)RequestTypes.KeepInView);
                db.AddInParameter("@Remark", DbType.String, request.Remark);
                db.AddInParameter("@OnDate", DbType.DateTime, request.Date);
                db.AddInParameter("@IsHalfDay", DbType.Boolean, request.HalfDay);
                db.AddInParameter("@ShiftId", DbType.Int32, request.ShiftId);
                db.AddInParameter("@FromTime", DbType.DateTime, fromTime);
                db.AddInParameter("@ToTime", DbType.DateTime, toTime);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@ModifiedBy", DbType.Int32, request.By);
                db.AddInParameter("@VerifierId", DbType.Int32, request.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, request.ApproverId);
                db.AddInParameter("@RefNo", DbType.String, request.RefNo);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        /// <summary>
        /// Converts the hour from string to double.
        /// </summary>
        /// <param name="hour">The input hour for converting.</param>
        /// <returns>The double format of hour.</returns>
        public double ConvertHourFromStringToDouble(string hour)
        {
            if (hour.StartsWith("0"))
            {
                hour = hour.Substring(1);
            }

            return Convert.ToDouble(hour);
        }

        /// <summary>
        /// Converts from time and to time from string format to date time format.
        /// </summary>
        /// <param name="keepInViewRequestViewModel">The keep in view request view model.</param>
        /// <param name="fromTime">From time for output.</param>
        /// <param name="toTime">To time for output.</param>
        private void ConvertFromTimeAndToTimeFromStringFormatToDateTimeFormat(
            KeepInViewRequestViewModel keepInViewRequestViewModel,
            ref DateTime? fromTime,
            ref DateTime? toTime)
        {
            if (keepInViewRequestViewModel.HalfDay)
            {
                string[] fromTimeArray = keepInViewRequestViewModel.FromTime.Split(':');
                string[] toTimeArray = keepInViewRequestViewModel.ToTime.Split(':');
                fromTime = new DateTime(keepInViewRequestViewModel.Date.Year, keepInViewRequestViewModel.Date.Month, keepInViewRequestViewModel.Date.Day);
                toTime = new DateTime(keepInViewRequestViewModel.Date.Year, keepInViewRequestViewModel.Date.Month, keepInViewRequestViewModel.Date.Day);

                fromTime = fromTime.Value.AddHours(this.ConvertHourFromStringToDouble(fromTimeArray[0])).AddMinutes(Convert.ToDouble(fromTimeArray[1]));

                if (keepInViewRequestViewModel.CrossDay)
                {
                    toTime = toTime.Value.AddDays(1);
                }

                toTime = toTime.Value.AddHours(this.ConvertHourFromStringToDouble(toTimeArray[0])).AddMinutes(Convert.ToDouble(toTimeArray[1]));
            }
        }
    }
}
