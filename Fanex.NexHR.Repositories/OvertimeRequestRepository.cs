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
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

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
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public RequestEntity GetOvertimeRequestById(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                Requester = new Employee
                                {
                                    Id = reader.ToInt("RequesterID"),
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
        /// Method Get Overtime Report In A Year.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>RequestEntity List.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetOvertimeReportInYear(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                Requester = new Employee
                                {
                                    Id = reader.ToInt("RequesterID"),
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
        /// HR View Overtime Report by Monthly.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of Request Entity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetMonthlyOvertimeReport(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                Requester = new Employee
                                {
                                    Id = reader.ToInt("RequesterID"),
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
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetMonthlyOvertimeReportByNormalCalendar(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                Requester = new Employee
                                {
                                    Id = reader.ToInt("RequesterID"),
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
                                RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                NormalHours = reader.ToDecimal("NormalHours"),
                                OffHours = reader.ToDecimal("OffHours"),
                                PublicHours = reader.ToDecimal("PublicHours"),
                                ToTalHours = reader.ToDecimal("ToTalHours"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                IsShowWarning = reader.ToBool("IsShowWarning"),
                                Requester = new Employee
                                {
                                    Id = reader.ToInt("RequesterID"),
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

        public IList<OvertimeRequest> OTExcelReportDetails(DateTime fromDate, DateTime toDate, int? empId, string jobTitle, string groups, byte? isLocal = null)
        {
            using (ObjectDb db = new ObjectDb("Report_OvertimeDetail"))
            {
                var entity = db.Query<OvertimeRequest>(new { fromDate, toDate, empId, JobTitleId = jobTitle, groupIds = groups, isLocal }).ToList();
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
                int rowIndex = 0;

                if (dataTableOvertimeExcelReport.Rows.Count != 0)
                {
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
