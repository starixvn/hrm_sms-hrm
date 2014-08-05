// -----------------------------------------------------------------------
// <copyright file="OvertimeRequestRepository.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Overtime Request Repository.
    /// </summary>
    public class OvertimeRequestRepository : IOvertimeRequestRepository
    {
        /// <summary>
        /// Method Get OvertimeRequest By Id.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>RequestEntity model.</returns>
        public RequestEntity GetOvertimeRequestById(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<RequestEntity, Employee, Team, Employee, Employee, RequestTypeModel, RequestStatus, RequestEntity>(
                    (re, reter, gr, ve, appr, rety, stt) =>
                    {
                        reter.Team = gr;
                        re.Requester = reter;
                        re.VerifiedBy = ve;
                        re.ApprovedBy = appr;
                        re.RequestType = rety;
                        re.Status = stt;
                        return re;
                    },
                    "RequesterID,GroupId,VerifierID,ApproverID,RequestTypeId,StatusId",
                    criteria.GetSpParams());
            }
        }

        /// <summary>
        /// Method Get Overtime Report In A Year.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>RequestEntity List.</returns>
        public IList<RequestEntity> GetOvertimeReportInYear(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<RequestEntity, Employee, Team, Employee, Employee, RequestTypeModel, RequestStatus, RequestEntity>(
                    (re, reter, gr, ver, appr, rety, stt) =>
                    {
                        reter.Team = gr;
                        re.Requester = reter;
                        re.VerifiedBy = ver;
                        re.ApprovedBy = appr;
                        re.RequestType = rety;
                        re.Status = stt;
                        return re;
                    },
                    "RequesterID,GroupId,VerifierID,ApproverID,RequestTypeId,StatusId",
                    criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// HR View Overtime Report by Monthly.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of Request Entity.</returns>
        public IList<RequestEntity> GetMonthlyOvertimeReport(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<RequestEntity, Employee, Team, Employee, Employee, RequestTypeModel, RequestStatus, RequestEntity>(
                    (re, reter, gr, ver, appr, rety, stt) =>
                    {
                        reter.Team = gr;
                        re.Requester = reter;
                        re.VerifiedBy = ver;
                        re.ApprovedBy = appr;
                        re.RequestType = rety;
                        re.Status = stt;
                        return re;
                    },
                    "RequesterID,GroupId,VerifierID,ApproverID,RequestTypeId,StatusId",
                    criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Method Get Monthly Overtime Report By Normal Calendar.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<RequestEntity, Employee, Team, RequestEntity>(
                    (re, reter, te) =>
                    {
                        reter.Team = te;
                        re.Requester = reter;
                        return re;
                    },
                    "RequesterID,GroupId",
                    criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Function to get member request of login user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of Overtime request.</returns>
        public IList<RequestEntity> GetMemberOvertimeMonthly(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<RequestEntity, Employee, Team, Employee, Employee, RequestTypeModel, RequestStatus, RequestEntity>(
                    (re, reter, gr, ver, appr, rety, stt) =>
                    {
                        reter.Team = gr;
                        re.Requester = reter;
                        re.VerifiedBy = ver;
                        re.ApprovedBy = appr;
                        re.RequestType = rety;
                        re.Status = stt;
                        return re;
                    },
                    "RequesterID,GroupId,VerifierID,ApproverID,RequestTypeId,StatusId",
                    criteria.GetSpParams()).ToList();
            }
        }

        /// <summary>
        /// Query Overtime Record.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List OvertimeExcelReport entity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<OvertimeExcelReport> OvertimeRecord(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<OvertimeExcelReport> entityList = new List<OvertimeExcelReport>();
                var ds = new DataSet();
                ds = db.ExecuteDataSet(criteria.GetSpParams());

                var dataTableOvertimeExcelReport = ds != null && ds.Tables != null && ds.Tables.Count > 0 
                                                            ? ds.Tables[0] : null;

                if (dataTableOvertimeExcelReport != null && dataTableOvertimeExcelReport.Rows != null && 
                                dataTableOvertimeExcelReport.Rows.Count != 0)
                {
                    var columnCount = (dataTableOvertimeExcelReport.Columns.Count - 12) / 2;
                    int rowIndex = 0;

                    foreach (System.Data.DataRow row in dataTableOvertimeExcelReport.Rows)
                    {
                        var overtimeExcelReport = new OvertimeExcelReport();

                        overtimeExcelReport.Team = row["Team"].ToString();
                        overtimeExcelReport.Positions = row["Positions"].ToString();
                        overtimeExcelReport.EmpId = Convert.ToInt32(!string.IsNullOrEmpty(row["EmpId"].ToString()) ? row["EmpId"].ToString() : "0");
                        overtimeExcelReport.EmpCode = row["EmpCode"].ToString();
                        overtimeExcelReport.Nickname = row["Nickname"].ToString();
                        overtimeExcelReport.Fullname = row["Fullname"].ToString();
                        overtimeExcelReport.TotalWorkDays = Convert.ToDouble(!string.IsNullOrEmpty(row["TotalWorkDays"].ToString()) ? row["TotalWorkDays"].ToString() : "0");
                        overtimeExcelReport.UnpaidLeave = Convert.ToDouble(!string.IsNullOrEmpty(row["UnpaidLeave"].ToString()) ? row["UnpaidLeave"].ToString() : "0");
                        overtimeExcelReport.Normal = Convert.ToDouble(!string.IsNullOrEmpty(row["Normal"].ToString()) ? row["Normal"].ToString() : "0");
                        overtimeExcelReport.Offday = Convert.ToDouble(!string.IsNullOrEmpty(row["Offday"].ToString()) ? row["Offday"].ToString() : "0");
                        overtimeExcelReport.Holiday = Convert.ToDouble(!string.IsNullOrEmpty(row["Holiday"].ToString()) ? row["Holiday"].ToString() : "0");
                        overtimeExcelReport.Tax = Convert.ToDouble(!string.IsNullOrEmpty(row["Tax"].ToString()) ? row["Tax"].ToString() : "0");

                        var list = new List<OvertimeDailyReport>();
                        
                        for (int column = 1; column <= columnCount; column++)
                        {
                            var entity = new OvertimeDailyReport()
                            {
                                TotalOvertimeType1 = dataTableOvertimeExcelReport.Rows[rowIndex][column == 1 ? (column + 11) : ((column * 2) + 10)].ToString(),
                                TotalOvertimeType2 = dataTableOvertimeExcelReport.Rows[rowIndex][column == 1 ? (column + 12) : ((column * 2) + 11)].ToString()
                            };
                            list.Add(entity);
                        }

                        overtimeExcelReport.OvertimeDailyReport = list;
                        entityList.Add(overtimeExcelReport);
                        rowIndex++;
                    }
                }

                return entityList;
            }
        }
    }
}
