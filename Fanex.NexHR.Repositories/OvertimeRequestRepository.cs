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
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OvertimeRequestRepository : IOvertimeRequestRepository
    {
        public IList<OvertimeRequest> GetOvertimeRequests(int empId, DateTime requestMonth)
        {
            using (ObjectDb db = new ObjectDb("Overtime_GetInOutMonthlyByEmpId"))
            {
                return db.Query<OvertimeRequest>(new { EmpId = empId, Month = requestMonth }).ToList();
            }
        }

        public IList<OvertimeRequest> HRGetOvertimeRequest(int supervisorId, int empId, DateTime requestMonth)
        {
            using (ObjectDb db = new ObjectDb("Overtime_GetOTMonthlyOfEmpByHR"))
            {
                return db.Query<OvertimeRequest>(new { SupervisorID = supervisorId, EmpId = empId, Month = requestMonth }).ToList();
            }
        }

        public IList<OvertimeRequest> HRGetNormalOvertimeRequest(int supervisorId, int empId, DateTime requestMonth)
        {
            using (ObjectDb db = new ObjectDb("OverTime_GetNormalDailyByHR"))
            {
                return db.Query<OvertimeRequest>(new { SupervisorID = supervisorId, EmpId = empId, Month = requestMonth }).ToList();
            }
        }

        /// <summary>
        /// Method Get OvertimeRequest By Id.
        /// </summary>
        /// <param name="requestId">Overtime request id.</param>
        /// <returns>RequestEntity model.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public RequestEntity GetOvertimeRequestById(int requestId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb("Overtime_GetRequestByRequestID"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                EmpId = reader.ToInt("RequesterID"),
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                VerifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("VerifierBy"),
                                    FullName = reader.To<string>("FullNameVerifierBy"),
                                    Id = reader.To<int>("VerifierID")
                                },
                                ApprovedBy = new Employee
                                {
                                    NickName = reader.To<string>("ApproverBy"),
                                    FullName = reader.To<string>("FullNameApproverBy"),
                                    Id = reader.To<int>("ApproverID")
                                }
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.SingleOrDefault();
        }

        /// <summary>
        /// Method Update Overtime.
        /// </summary>
        /// <param name="model">MyOvertime ViewModel.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateOvertimeRequest(MyOverTimeViewModel model, out int errorCode, out string message)
        {
            using (ObjectDb db = new ObjectDb("Overtime_SET_OTRequest"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, model.EmpId);
                db.AddInParameter("@ProcessedBy", DbType.Int32, model.ProcessedBy);
                db.AddInParameter("@ProcessedDate", DbType.DateTime, model.ProcessedDate);
                db.AddInParameter("@VerifierId", DbType.Int32, model.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, model.ApproverId);
                db.AddInParameter("@ListDetail", DbType.String, model.OvertimeRequestDetail);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                if (errorCode != 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Method Get Overtime Report In A Year.
        /// </summary>
        /// <param name="empId">Employee id.</param>
        /// /// <param name="year">Request for year.</param>
        /// <returns>RequestEntity List.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetOvertimeReportInYear(int empId, int year)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb("OverTime_GetRequestMonthlyByEmpId"))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = empId, year = year }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                EmpId = reader.ToInt("RequesterID"),
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                VerifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("VerifierBy"),
                                    FullName = reader.To<string>("FullNameVerifierBy"),
                                    Id = reader.To<int>("VerifierID")
                                },
                                ApprovedBy = new Employee
                                {
                                    NickName = reader.To<string>("ApproverBy"),
                                    FullName = reader.To<string>("FullNameApproverBy"),
                                    Id = reader.To<int>("ApproverID")
                                }
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.ToList();
        }

        /// <summary>
        /// Method GetOvertimeRequest.
        /// </summary>
        /// <param name="role">The role of viewer.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>
        /// List RequestEntity.
        /// </returns>
        public IList<RequestEntity> GetOvertimeRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null, 
            byte? isLocal = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest ? "WorkingTimeRequest_GetRequestByDate_OvertimeType" : "WorkingTimeRequest_GetRequestByEmp_OvertimeType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = role, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                Remark = reader.ToString("Remark"),
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                OvertimeType = reader.ToShort("OvertimeType"),
                                IsOvertimeHours = reader.ToBool("IsOvertimeHours"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method Get OvertimeRequestHistory.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <param name="requestTypeId">Request type id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetOvertimeRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("Id"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                Remark = reader.ToString("Remark"),
                                OvertimeType = reader.ToShort("OvertimeType"),
                                IsOvertimeHours = reader.ToBool("IsOvertimeHours"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },

                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },

                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                ModifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("ProcessedBy"),
                                    FullName = reader.To<string>("FullNameProcessedBy")
                                },
                                ModifiedDate = reader.ToDateTime("ProcessedDate")
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        public IList<OvertimeAnnualReport> GetYearlyOvertimeReport(int year, string team, int? empId, int supervisorId, string status)
        {
            using (ObjectDb db = new ObjectDb("OverTime_GetRequestYearlyByHR"))
            {
                return db.Query<OvertimeAnnualReport>(new { EmpId = empId, SupervisorID = supervisorId, Year = year, groupids = team, status = status }).ToList();
            }
        }

        public IList<OvertimeAnnualReport> GetYearlyOvertimeReportByNormalCalendar(int year, string team, int? empId, int supervisorId, string status)
        {
            using (ObjectDb db = new ObjectDb("OverTime_GetNormalYearlyByHR"))
            {
                return db.Query<OvertimeAnnualReport>(new { EmpId = empId, SupervisorID = supervisorId, Year = year, groupids = team, status = status }).ToList();
            }
        }

        /// <summary>
        /// HR View Overtime Report by Monthly.
        /// </summary>
        /// <param name="date">Request Date.</param>
        /// <param name="empId">Employee id.</param>
        /// <param name="groupids">Group's id.</param>
        /// <param name="status">Request status.</param>
        /// <returns>List of Request Entity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetMonthlyOvertimeReport(DateTime date, int? empId, string groupids, string status)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb("OverTime_GetRequestMonthlyByHR"))
            {
                using (IDataReader reader = db.ExecuteReader(new { empId, fromdate = date, todate = date, groupids, status }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                EmpId = reader.ToInt("RequesterID"),
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                VerifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("VerifierBy"),
                                    FullName = reader.To<string>("FullNameVerifierBy"),
                                    Id = reader.To<int>("VerifierID")
                                },
                                ApprovedBy = new Employee
                                {
                                    NickName = reader.To<string>("ApproverBy"),
                                    FullName = reader.To<string>("FullNameApproverBy"),
                                    Id = reader.To<int>("ApproverID")
                                }
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.ToList();
        }

        /// <summary>
        /// Method Get Monthly Overtime Report By Normal Calendar.
        /// </summary>
        /// <param name="fromDate">Get From Date.</param>
        /// <param name="toDate">Get to Date.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="groupids">Group's identify.</param>
        /// <param name="status">Current status.</param>
        /// <returns>List of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(DateTime fromDate, DateTime toDate, int? empId, string groupids, string status)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb("OverTime_GetNormalMonthlyByHR"))
            {
                using (IDataReader reader = db.ExecuteReader(new { empId, fromdate = fromDate, todate = toDate, groupids, status }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                EmpId = reader.ToInt("RequesterID"),
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                }
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.ToList();
        }

        public IList<OvertimeRequest> GetMemberOvertimeDaily(int supervisorId, string roles, int employeeId, DateTime month)
        {
            using (ObjectDb db = new ObjectDb("Overtime_GetOTMonthlyOfEmpByHR"))
            {
                return db.Query<OvertimeRequest>(new { SupervisorID = supervisorId, role = roles, EmpId = employeeId, Month = month }).ToList();
            }
        }

        /// <summary>
        /// Function to get member request of login user.
        /// </summary>
        /// <param name="supervisorId">Login user id.</param>
        /// <param name="roles">Role of user login.</param>
        /// <param name="month">Month need query.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <param name="teamIds">Input Team Ids.</param>
        /// <param name="status">Status of request.</param>
        /// <returns>List of Overtime request.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetMemberOvertimeMonthly(int supervisorId, string roles, DateTime month, int? employeeId, string teamIds, string status)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb("OverTime_GetRequestMonthlyByHR"))
            {
                using (IDataReader reader = db.ExecuteReader(new
                {
                    SupervisorID = supervisorId,
                    role = roles,
                    EmpId = employeeId,
                    fromdate = month,
                    todate = month,
                    groupIds = string.IsNullOrWhiteSpace(teamIds) ? null : teamIds,
                    status = string.IsNullOrWhiteSpace(status) ? null : status
                }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                EmpId = reader.ToInt("RequesterID"),
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                VerifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("VerifierBy"),
                                    FullName = reader.To<string>("FullNameVerifierBy"),
                                    Id = reader.To<int>("VerifierID")
                                },
                                ApprovedBy = new Employee
                                {
                                    NickName = reader.To<string>("ApproverBy"),
                                    FullName = reader.To<string>("FullNameApproverBy"),
                                    Id = reader.To<int>("ApproverID")
                                }
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.ToList();
        }

        public IList<OvertimeRequest> OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups)
        {
            using (ObjectDb db = new ObjectDb("Report_OvertimeDetail"))
            {
                var entity = db.Query<OvertimeRequest>(new { fromDate, toDate, empId, JobTitleId = jobTitle, groupIds = groups }).ToList();
                return entity;
            }
        }

        public IList<OvertimeRequest> GetOvertimeByRequestIds(string requestIds)
        {
            using (ObjectDb db = new ObjectDb("OverTime_GetDetailByListRequestID"))
            {
                return db.Query<OvertimeRequest>(new { ListRequestID = requestIds }).ToList();
            }
        }

        /// <summary>
        /// Query Overtime Record.
        /// </summary>
        /// <param name="requestDate">Requested date.</param>
        /// <param name="calendar">Normal or fiscal calendar.</param>
        /// <param name="empId">Employee's id.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>List OvertimeExcelReport entity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<OvertimeExcelReport> OvertimeRecord(DateTime requestDate, int calendar, int? empId, out int errorCode, out string message)
        {
            var keyName = calendar == 1 ? "ExportExcel_OvertimeFiscal" : "ExportExcel_OvertimeNormal";
            using (var db = new ObjectDb(keyName))
            {
                IList<OvertimeExcelReport> entityList = new List<OvertimeExcelReport>();
                var ds = new DataSet();
                db.AddInParameter("@Month", DbType.DateTime, requestDate);
                db.AddInParameter("@EmpID", DbType.Int32, empId);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                ds = db.ExecuteDataSet();
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                var dataTableOvertimeExcelReport = ds != null ? ds.Tables[0] : null;
                var columnCount = (dataTableOvertimeExcelReport.Columns.Count - 12) / 2;
                var rowCont = dataTableOvertimeExcelReport.Rows.Count;
                int rowIndex = 0;

                if (dataTableOvertimeExcelReport.Rows.Count != 0)
                {
                    foreach (System.Data.DataRow row in dataTableOvertimeExcelReport.Rows)
                    {
                        var overtimeExcelReport = new OvertimeExcelReport();

                        overtimeExcelReport.Team = row["Team"].ToString();
                        overtimeExcelReport.Positions = row["Positions"].ToString();
                        overtimeExcelReport.EmpId = Convert.ToInt32(row["EmpId"].ToString() != string.Empty ? row["EmpId"].ToString() : "0");
                        overtimeExcelReport.EmpCode = row["EmpCode"].ToString();
                        overtimeExcelReport.Nickname = row["Nickname"].ToString();
                        overtimeExcelReport.Fullname = row["Fullname"].ToString();
                        overtimeExcelReport.TotalWorkDays = Convert.ToDouble(row["TotalWorkDays"].ToString() != string.Empty ? row["TotalWorkDays"].ToString() : "0");
                        overtimeExcelReport.UnpaidLeave = Convert.ToDouble(row["UnpaidLeave"].ToString() != string.Empty ? row["UnpaidLeave"].ToString() : "0");
                        overtimeExcelReport.Normal = Convert.ToDouble(row["Normal"].ToString() != string.Empty ? row["Normal"].ToString() : "0");
                        overtimeExcelReport.Offday = Convert.ToDouble(row["Offday"].ToString() != string.Empty ? row["Offday"].ToString() : "0");
                        overtimeExcelReport.Holiday = Convert.ToDouble(row["Holiday"].ToString() != string.Empty ? row["Holiday"].ToString() : "0");
                        overtimeExcelReport.Tax = Convert.ToDouble(row["Tax"].ToString() != string.Empty ? row["Tax"].ToString() : "0");

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
