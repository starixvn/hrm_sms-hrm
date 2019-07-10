namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;

    public class OvertimeRequestRepository : IOvertimeRequestRepository
    {
        public OvertimeRequest GetOvertimeRequestByMonth(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria))
                {
                    var overtime = multi.Read<OvertimeRequest, Employee, Employee, Employee, Employee, OvertimeRequest>(
                        (ot, e, ve, ap, hr) =>
                        {
                            ot.Employee = e;
                            ot.VerifiedBy = ve;
                            ot.ApprovedBy = ap;
                            ot.ProcessedBy = hr;
                            return ot;
                        },
                        "EmpId, VerifierId, ApproverId, HRId").FirstOrDefault();
                    overtime.OvertimeDaily = multi.Read<OvertimeDaily>().ToList();
                    return overtime;
                }
            }
        }

        public OvertimeRequest GetOvertimeDailyReport(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria))
                {
                    var overtime = multi.Read<OvertimeRequest, Employee, Employee, Employee, Employee, OvertimeRequest>(
                        (ot, e, ve, ap, hr) =>
                        {
                            ot.Employee = e;
                            ot.VerifiedBy = ve;
                            ot.ApprovedBy = ap;
                            ot.ProcessedBy = hr;
                            return ot;
                        },
                        "EmpId, VerifierId, ApproverId, HRId").FirstOrDefault();
                    overtime.OvertimeDaily = multi.Read<OvertimeDaily>().ToList();
                    return overtime;
                }
            }
        }

        public IEnumerable<MonthlyOvertimeRequest> GetMonthlyRequestByEmployeeId(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, MonthlyOvertimeRequest>(
                    (re, stt, verifier, approver) =>
                    {
                        re.Status = stt;
                        re.Verifier = verifier;
                        re.Approver = approver;
                        return re;
                    },
                    "StatusId, VerifierId, ApproverId",
                    criteria.GetSpParams());
            }
        }

        public MonthlyOvertimeRequest GetMonthlyRequestDetailsByEmployeeId(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria))
                {
                    var monthlyRequest = multi.Read<MonthlyOvertimeRequest, RequestStatus, Requester, Requester, Requester, MonthlyOvertimeRequest>(
                    (re, stt, requester, verifier, approver) =>
                    {
                        re.Requester = requester;
                        re.Status = stt;
                        re.Verifier = verifier;
                        re.Approver = approver;
                        return re;
                    },
                    "StatusId, EmployeeId, VerifierId, ApproverId").FirstOrDefault();

                    if (monthlyRequest != null)
                    {
                        monthlyRequest.DailyOvertimes = multi.Read<DailyOvertimeRequest>();
                    }

                    return monthlyRequest;
                }
            }
        }

        public IEnumerable<MonthlyOvertimeRequest> GetMemberMonthlyRequest(ICriteria criteria)
        {
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<MonthlyOvertimeRequest, RequestStatus, Requester, EntityBase<int>, Requester, Requester, MonthlyOvertimeRequest>(
                    (req, stt, requester, section, verifier, approver) =>
                    {
                        requester.Section = section;
                        req.Status = stt;
                        req.Requester = requester;
                        req.Verifier = verifier;
                        req.Approver = approver;
                        return req;
                    },
                    "StatusId, EmployeeId, GroupId, VerifierId, ApproverId",
                    criteria.GetSpParams());
            }
        }

        public IList<OvertimeExcelReport> OvertimeRecord(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                var entityList = new List<OvertimeExcelReport>();
                var ds = db.ExecuteDataSet(criteria.GetSpParams());
                var dataTableOvertimeExcelReport = ds != null && ds.Tables != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;

                if (dataTableOvertimeExcelReport != null && dataTableOvertimeExcelReport.Rows != null && dataTableOvertimeExcelReport.Rows.Count != 0)
                {
                    var columnCount = (dataTableOvertimeExcelReport.Columns.Count - 12) / 2;
                    int rowIndex = 0;

                    foreach (DataRow row in dataTableOvertimeExcelReport.Rows)
                    {
                        var overtimeExcelReport = SetOvertimeExcelReport(row);

                        var list = new List<OvertimeDailyReport>();

                        for (int column = 1; column <= columnCount; column++)
                        {
                            var entity = SetValueOvertimeDailyReport(dataTableOvertimeExcelReport, column, rowIndex);
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

        private static OvertimeExcelReport SetOvertimeExcelReport(DataRow row)
        {
            OvertimeExcelReport overtimeExcelReport = new OvertimeExcelReport();
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

            return overtimeExcelReport;
        }

        private static OvertimeDailyReport SetValueOvertimeDailyReport(DataTable dataTableOvertimeExcelReport, int column, int rowIndex)
        {
            var entity = new OvertimeDailyReport
            {
                TotalOvertimeType1 = dataTableOvertimeExcelReport.Rows[rowIndex][column == 1 ? (column + 11) : ((column * 2) + 10)].ToString(),
                TotalOvertimeType2 = dataTableOvertimeExcelReport.Rows[rowIndex][column == 1 ? (column + 12) : ((column * 2) + 11)].ToString()
            };

            return entity;
        }
    }
}