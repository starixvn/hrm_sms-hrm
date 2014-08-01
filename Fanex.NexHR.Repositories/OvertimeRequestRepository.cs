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

        /// <summary>
        /// Function to get member request of login user.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of Overtime request.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetMemberOvertimeMonthly(ICriteria criteria)
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
