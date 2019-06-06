namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Criteria.Calendar;
    using Criteria.Request;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Employee;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    public class EmployeeController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly ILookupDataService lookupDataService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly IRoleService roleService;
        private readonly IAuthService authService;

        public EmployeeController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            ILookupDataService lookupDataService,
            IBaseService baseService,
            IRoleService roleService,
            IAuthService authService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.lookupDataService = lookupDataService;
            this.baseService = baseService;
            this.roleService = roleService;
            this.authService = authService;
        }

        public ActionResult Index()
        {
            var model = new UserManagementViewModel
            {
                IsEnable = true,
                Groups = GetAllTeam(),
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ContactList(UserManagementViewModel model)
        {
            IList<Employee> employees = new List<Employee>();

            if (model != null)
            {
                var criteria = new GetAllEmployeeCriteria
                {
                    Query = model.Keyword,
                    GroupIds = model.GroupIds,
                    EnableUser = model.IsEnable,
                    IsLocalUser = model.IsLocal,
                    PageNumber = model.CurrentPage,
                    PageSize = model.ItemsPerPage
                };

                employees = employeeService.GetEmployees(criteria);

                var pagerOption = new PagerOptionModel
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalRow
                };
                ViewBag.PagerOption = pagerOption;
            }

            return PartialView("_ContactList", employees);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult UserManagement(int? jobTitleId = null, string groupIds = "")
        {
            IList<LookupData> listJobTitle = GetAllJobTitle();
            IList<SelectListItem> jobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == (jobTitleId != null ? jobTitleId.ToString() : string.Empty)
            }).ToList();

            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                GroupIds = groupIds,
                IsEnable = true,
                Groups = GetAllTeam(),
                JobTitles = jobTitles,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult ListEmployee(UserManagementViewModel model)
        {
            IList<Employee> employees = new List<Employee>();

            if (model != null)
            {
                var criteria = new GetAllEmployeeCriteria
                {
                    Query = model.Keyword,
                    GroupIds = model.GroupIds,
                    EnableUser = model.IsEnable,
                    IsDeleteUser = model.IsDelete,
                    TitleId = model.JobTitleId,
                    IsLocalUser = model.IsLocal,
                    PageNumber = model.CurrentPage,
                    PageSize = model.ItemsPerPage
                };

                employees = employeeService.GetEmployees(criteria);

                var pagerOption = new PagerOptionModel
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalRow
                };
                ViewBag.PagerOption = pagerOption;
            }

            ViewBag.HasPermissionToEdit = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);

            return PartialView("_ListEmployee", employees);
        }

        public ActionResult Detail(int id)
        {
            Employee model = GetEmployeeById(id);
            EmployeeShift employeeShift = GetEmployeeShift(id);

            ViewBag.Sun = employeeShift.SunShiftName;
            ViewBag.Mon = employeeShift.MonShiftName;
            ViewBag.Tue = employeeShift.TueShiftName;
            ViewBag.Wed = employeeShift.WedShiftName;
            ViewBag.Thu = employeeShift.ThuShiftName;
            ViewBag.Fri = employeeShift.FriShiftName;
            ViewBag.Sat = employeeShift.SatShiftName;
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.CreateAccount)]
        public ActionResult Create()
        {
            IList<Team> teams = GetAllTeam();
            ViewBag.Teams = teams;

            return View(new CreateEmployeeViewModel());
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.CreateAccount)]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertEmployeeCriteria
                {
                    Code = model.Code,
                    CreateBy = sessionService.Employee.Id,
                    DomainUserName = model.DomainUsername,
                    FullName = model.FullName,
                    Nickname = model.Nickname,
                    GroupId = model.TeamId,
                    Password = Encryptor.ComputeHash(authService.HashProvider, model.Code)
                };

                CommandResult result = baseService.ExecuteNonQuery(criteria, hasGetOutputParams: true);
                if (result.IsSuccessful)
                {
                    return RedirectToAction("Edit", new { id = criteria.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            IList<Team> teams = GetAllTeam();
            ViewBag.Teams = teams;
            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            Employee model = GetEmployeeById(id);
            if (model != null)
            {
                model.Coutries = GetCountries();

                IList<SupervisorNameModel> supervisors = GetSupervisors(id);
                model.Verifier = GetVerifiers(supervisors);
                model.Approver = GetApprovers(supervisors);
                model.Disciplinarian = GetDisciplineSupervisors(supervisors);
                model.EmpShiftList = GetShiftMonitors(supervisors);
                model.ActingEmpList = GetActingSupervisors(supervisors);
            }

            ComposeViewBagsForEditEmployee();
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(Employee model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    UpdateEmployeeCriteria criteria = SetUpdateEmployeeCriteria(model);
                    CommandResult result = baseService.ExecuteNonQuery(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Updated = true;
                        UpdateUserSession(model.Id, model.Code);
                    }
                    else
                    {
                        ViewBag.Updated = false;
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }

                model.Coutries = GetCountries();

                IList<SupervisorNameModel> supervisors = GetSupervisors(model.Id);
                model.Verifier = GetVerifiers(supervisors);
                model.Approver = GetApprovers(supervisors);
                model.Disciplinarian = GetDisciplineSupervisors(supervisors);
                model.EmpShiftList = GetShiftMonitors(supervisors);
                model.ActingEmpList = GetActingSupervisors(supervisors);
            }

            ComposeViewBagsForEditEmployee();

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(int id, DateTime resignedDate)
        {
            var criteria = new DeleteEmployeeCriteria
            {
                EmpId = id,
                ResignedDate = resignedDate
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            var avatarName = string.IsNullOrEmpty(id) ? sessionService.Employee.Code + ".jpg" : id + ".jpg";
            if (System.IO.File.Exists(Server.MapPath("/avatars/" + avatarName)))
            {
                return File("/avatars/" + avatarName, "image/jpeg");
            }

            return File("/avatars/avatar.jpg", "image/jpeg");
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public JsonResult UploadAvatar(string code, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                using (var stream = new MemoryStream())
                {
                    using (var processor = new ImageProcessor())
                    {
                        var resizer = new ResizeFilter(64, 64, ResizeMethod.Crop, AnchorLocation.Middle);
                        processor.AddFilter(resizer);
                        processor.OutputFormat = OutputFormat.Jpeg;
                        processor.JpegCompression = 100;
                        processor.ProcessImage(file.InputStream, stream);
                        var image = Image.FromStream(stream);
                        image.Save(Server.MapPath("/avatars/") + code + ".jpg");
                        image.Dispose();

                        return Json(true);
                    }
                }
            }

            return Json(null);
        }

        public ActionResult MySpace()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria
            {
                EmpId = sessionService.Employee.Id,
                Date = DateTime.Now
            };
            MySpaceModel model = employeeService.GetMySpace(getMySpaceCriteria);

            Employee employee = GetEmployeeById(sessionService.Employee.Id);
            ViewBag.ResetPassword = employee.PasswordReset;
            model.HasManagementDashboard = sessionService.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || sessionService.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            var criteria = new GetHRLeaderDashboardCriteria
            {
                Empid = sessionService.Employee.Id,
                Date = DateTime.Now
            };
            IList<HRLeaderDashboardModel> model = employeeService.GetHRLeaderDashboard(criteria);

            return PartialView("MultiRoleManagementDashboard", model);
        }

        [HttpGet]
        public ActionResult Supervisor(short role, int employeeId)
        {
            var criteria = new GetEmployeeSupervisorCriteria
            {
                EmpId = employeeId,
                Role = role
            };
            IList<SupervisorModel> supervisorModel = employeeService.GetEmployeeSupervisor(criteria);
            var model = new SupervisorViewModel
            {
                SupervisorList = supervisorModel,
                EmployeeId = employeeId,
                SupervisorRole = role,
                ForView = "Employee"
            };

            ViewBag.Groups = GetAllTeam();

            return View(model);
        }

        [HttpPost]
        public ActionResult Supervisor(SupervisorViewModel model)
        {
            var criteria = new UpdateSupervisorCriteria
            {
                EmpId = model.EmployeeId,
                Role = model.SupervisorRole,
                EmpIdLeaves = model.RoleLeaves,
                EmpIdChangeShifts = model.RoleChangeShifts,
                EmpIdLateInEarlyOuts = model.RoleLateInEarlyOuts,
                EmpIdCheckInOuts = model.RoleCheckInOuts,
                EmpIdOvertimes = model.RoleOvertimes,
                EmpIdKeepInViews = model.RoleKeepInViews,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HttpGet]
        public ActionResult ShiftManager(short role, int employeeId)
        {
            var criteria = new GetEmployeeSupervisorCriteria
            {
                EmpId = employeeId,
                Role = role
            };
            IList<SupervisorModel> shiftManagerModel = employeeService.GetEmployeeSupervisor(criteria);

            var model = new ShiftManagerViewModel
            {
                ShiftManagerList = shiftManagerModel,
                EmployeeId = employeeId,
                SupervisorRole = role,
                ForView = "Employee"
            };

            ViewBag.Groups = GetAllTeam();

            return View(model);
        }

        [HttpPost]
        public ActionResult ShiftManager(ShiftManagerViewModel model)
        {
            var insertShiftMangerCriteria = new InsertShiftManagerCriteria
            {
                EmpId = model.EmployeeId,
                EmpIdShift = model.RoleShiftArrange,
                ModifiedBy = sessionService.Employee.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(insertShiftMangerCriteria);

            return Json(result);
        }

        [HttpPost]
        public void ToogleLeftNavigation(bool isShow)
        {
            sessionService.IsShowLeftNavigation = isShow;
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult DisciplineSetting(int employeeId)
        {
            ViewBag.Groups = GetAllTeam();

            var criteria = new GetDisciplineSettingByEmployeeId
            {
                EmpId = employeeId
            };
            IList<SupervisorModel> supervisors = employeeService.GetDisciplineSettingByEmployeeId(criteria);

            var model = new SupervisorViewModel
            {
                SupervisorList = supervisors,
                EmployeeId = employeeId,
                SupervisorRole = (short)RoleSystem.DisciplineManager
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult UpdateDisciplineSetting(SupervisorViewModel model)
        {
            var criteria = new UpdateSupervisorCriteria
            {
                EmpIdDisciplines = model.RoleDisciplines,
                EmpId = model.EmployeeId,
                ModifiedBy = sessionService.Employee.Id,
                Role = (short)RoleSystem.DisciplineManager
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberByManager(string keyword)
        {
            var criteria = new GetMemberByManagerCriteria
            {
                SupervisorId = sessionService.Employee.Id,
                Query = keyword,
                ExcludedManager = false
            };
            IList<Employee> employees = employeeService.GetMemberByManager(criteria);

            return Json(employees.Take(10), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployees(
            string query,
            string groupIds = "",
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null,
            bool? isLocal = null)
        {
            var getEmployeeCriteria = new GetAllEmployeeCriteria
            {
                Query = query,
                GroupIds = groupIds,
                EnableUser = enable,
                IsDeleteUser = isDelete,
                TitleId = titleId,
                IsLocalUser = isLocal
            };
            IList<Employee> employees = employeeService.GetEmployees(getEmployeeCriteria);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupervisors(string roleIds, string query, string excludeIds)
        {
            var criteria = new GetEmployeeByRoleCriteria
            {
                ExcludeIds = excludeIds ?? string.Empty,
                Query = query.Trim(),
                RoleIds = roleIds
            };
            IList<Employee> employees = employeeService.GetEmployeesByRole(criteria);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRangeDate(int leaveTypeId, int employeeId, DateTime fromDate, DateTime toDate)
        {
            var numberDays = 0;
            switch (leaveTypeId)
            {
                case Constants.MaternityLeave:
                case Constants.CompensationLeave:
                    numberDays = (toDate - fromDate).Days + 1;
                    break;

                case Constants.UnpaidLeave:
                case Constants.BusinessTrip:
                    var criteria = new GetOffDateAndHolidayDateCriteria
                    {
                        EmpId = employeeId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };
                    IEnumerable<DateTime> dates = employeeService.GetOffDateAndHolidayDate(criteria);
                    numberDays = (toDate - fromDate).Days + 1 - (dates != null ? dates.Count() : 0);
                    break;

                default:
                    var getDateRangeCriteria = new GetDateRangeCriteria
                    {
                        EmpId = employeeId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };
                    numberDays = employeeService.GetDateRange(getDateRangeCriteria);
                    break;
            }

            return Json(numberDays, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnavailableDates(int employeeId, DateTime date, int? leaveTypeId = null)
        {
            IEnumerable<DateTime> dates = null;
            var fromDate = new DateTime(date.AddMonths(-5).Year, date.AddMonths(-5).Month, 1);
            var tempDate = date.AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest);
            var toDate = new DateTime(tempDate.Year, tempDate.Month, DateTime.DaysInMonth(tempDate.Year, tempDate.Month));

            if (leaveTypeId.HasValue && (leaveTypeId.Value == Constants.UnpaidLeave
                || leaveTypeId.Value == Constants.BusinessTrip
                || leaveTypeId.Value == Constants.UnpaidLeaveInsurance))
            {
                var criteria = new GetOffDateAndHolidayDateCriteria
                {
                    EmpId = employeeId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                dates = employeeService.GetOffDateAndHolidayDate(criteria);
            }
            else
            {
                var getUnavailableDateCriteria = new GetUnavailableRequestDateCriteria
                {
                    EmpId = employeeId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                dates = employeeService.GetUnavailableDates(getUnavailableDateCriteria);
            }

            return Json(dates.Select(t => t.Date.ToString("yyyy/MM/dd")), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNewEmployeeCode()
        {
            var getNewEmployeeCodeCriteria = new GetNewEmployeeCodeCriteria();
            string newCode = employeeService.NewEmployeeCode(getNewEmployeeCodeCriteria);

            return Json(newCode, JsonRequestBehavior.AllowGet);
        }

        private void UpdateUserSession(int employeeId, string code)
        {
            if (sessionService.Employee.Id == employeeId)
            {
                sessionService.Employee = authService.GetEmployeeByCode(code);
            }
        }

        private IList<SupervisorNameModel> GetSupervisors(int empId)
        {
            var criteria = new GetListSupervisorNameCriteria
            {
                EmpId = empId,
                Role = null,
                RequestType = null
            };
            IList<SupervisorNameModel> supervisors = employeeService.GetListSupervisorName(criteria);
            return supervisors;
        }

        private static string GetSupervisors(IList<SupervisorNameModel> supervisors, int roleSystem)
        {
            string supervisorsName = string.Empty;
            IList<SupervisorNameModel> supervisorsFilter;

            if (supervisors != null)
            {
                supervisorsFilter = supervisors.Where(x => x.ActionType == roleSystem).Take(3).ToList();
                supervisorsName = string.Join(", ", supervisorsFilter.Select(x => x.NickName));

                if (supervisorsFilter.Count > 3)
                {
                    supervisorsName = supervisorsName + ",...";
                }
            }

            return supervisorsName;
        }

        private static string GetVerifiers(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (int)RoleSystem.Verifier);
        }

        private static string GetApprovers(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (int)RoleSystem.Approver);
        }

        private static string GetShiftMonitors(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (int)RoleSystem.ShiftManager);
        }

        private static string GetActingSupervisors(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (int)RoleSystem.ActingSupervisor);
        }

        private static string GetDisciplineSupervisors(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (int)RoleSystem.DisciplineManager);
        }

        private IList<Country> GetCountries()
        {
            var criteria = new GetAllCountryCriteria();
            IList<Country> countries = employeeService.GetAllCountry(criteria);
            return countries;
        }

        private Employee GetEmployeeById(int empId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmpId = empId
            };

            Employee employee = employeeService.GetEmployeeById(criteria);
            return employee;
        }

        private EmployeeShift GetEmployeeShift(int empId)
        {
            var employeeShiftCriteria = new GetEmployeeShiftCriteria
            {
                EmpId = empId
            };

            EmployeeShift employeeShift = employeeService.GetEmployeeShift(employeeShiftCriteria);
            return employeeShift;
        }

        private void ComposeViewBagsForEditEmployee()
        {
            ViewBag.Groups = GetAllTeam();
            ViewBag.JobTitles = GetAllJobTitle();

            ViewBag.Marriages = Utilities.Marriages;
            ViewBag.Roles = roleService.GetRoles().Where(i => !i.Code.Equals(RoleSystem.Member.ToString()));
        }

        private IList<Team> GetAllTeam()
        {
            var criteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(criteria);

            return teams;
        }

        private IList<LookupData> GetAllJobTitle()
        {
            var criteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> jobTitles = lookupDataService.GetAll(criteria);

            return jobTitles;
        }

        private UpdateEmployeeCriteria SetUpdateEmployeeCriteria(Employee model)
        {
            var criteria = new UpdateEmployeeCriteria
            {
                EmpId = model.Id,
                Code = model.Code.Trim(),
                DomainUsername = model.DomainUsername,
                FullName = model.FullName,
                NickName = model.NickName,
                EmployeeCard = model.EmployeeCard,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                JobTitleId = model.JobTitleId,
                Enabled = model.Enabled,
                Phone = model.Phone,
                Gender = model.Gender,
                Roles = BuildRole(model.Roles),
                JoinedDate = model.JoinedDate,
                CompanyDate = model.CompanyDate,
                ConfirmedDate = model.ConfirmedDate,
                ResignedDate = model.ResignedDate,
                Skype = model.Skype,
                Yahoo = model.Yahoo,
                Marriage = model.Marriage,
                PermanentAddress = model.Address,
                CurrentAddress = model.CurrentAddress,
                IdCardNumber = model.CardNo,
                PassportNo = model.PassportNo,
                PassportExp = model.PassportExp,
                ModifiedDate = DateTime.Now,
                ModifiedBy = sessionService.Employee.Id,
                Password = Encryptor.ComputeHash(authService.HashProvider, model.Password ?? string.Empty),
                GroupId = model.Team.Id,
                IsLocal = model.IsLocal,
                CountryCode = model.CountryCode,
                CanOT = model.CanOT,
                WorkingDays = model.WorkingDays,
                WorkingHours = model.WorkingHours,
                IsTopManager = model.IsTopManager,
                OffDaysbyWeek = model.OffDaysbyWeek,
                Unit = model.Unit,
                PlaceOfbirth = model.PlaceOfBirth,
                Ethnic = model.Ethnic,
                Native = model.Native,
                Religion = model.Religion,
                DateOfIdCard = model.DateOfIdCard,
                PlaceOfIdCard = model.PlaceOfIdCard,
                HomePhone = model.HomePhone,
                Qualification = model.Qualification,
                Major = model.Major,
                BankAccount = model.BankAccount,
                TaxCode = model.TaxCode,
                SocialInsuranceNo = model.SocialInsuranceNo,
                RegisteredHospital = model.RegisteredHospital,
                IsExpatUseHoliday = !model.IsLocal && model.IsExpatUseHoliday,
                PickupRemark = model.PickupRemark
            };

            return criteria;
        }

        private static string BuildRole(IList<SystemRoleModel> roles)
        {
            var result = string.Empty;
            if (roles != null)
            {
                if (roles.Any(i => i.RoleId == (int)RoleSystem.SystemAdmin))
                {
                    result = BuildSARole();
                }
                else if (roles.Any(i => i.RoleId == (int)RoleSystem.DisciplineManager))
                {
                    result = BuildDisciplineManagerRole(roles);
                }
                else
                {
                    result = string.Join(",", roles.Select(i => i.RoleId));
                }
            }

            return string.IsNullOrEmpty(result) ? BuildDefaultRole() : BuildDefaultRole() + "," + result;
        }

        private static string BuildDefaultRole()
        {
            return ((int)RoleSystem.Member).ToString();
        }

        private static string BuildDisciplineManagerRole(IList<SystemRoleModel> roles)
        {
            IList<int> roleIds = roles.Where(x => x.RoleId != (int)RoleSystem.Approver || x.RoleId != (int)RoleSystem.DisciplineManager)
                .Select(x => x.RoleId).ToList();
            roleIds.Add((int)RoleSystem.Approver);
            roleIds.Add((int)RoleSystem.DisciplineManager);

            return string.Join(",", roleIds);
        }

        private static string BuildSARole()
        {
            return string.Format(
                "{0},{1},{2},{3},{4},{5},{6}",
                ((int)RoleSystem.ActingSupervisor).ToString(),
                ((int)RoleSystem.Approver).ToString(),
                ((int)RoleSystem.DisciplineManager).ToString(),
                ((int)RoleSystem.HR).ToString(),
                ((int)RoleSystem.ShiftManager).ToString(),
                ((int)RoleSystem.SystemAdmin).ToString(),
                ((int)RoleSystem.Verifier).ToString());
        }
    }
}