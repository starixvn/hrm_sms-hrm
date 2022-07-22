namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Log;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Employee;
    using Simplicode.Imaging;

    public class EmployeeController : BaseController
    {
        private readonly ICommonDataService commonDataService;
        private readonly IEmployeeService employeeService;
        private readonly IRoleService roleService;
        private readonly IAuthService authService;
        private readonly IEmployeeLogService employeeLogService;
        private readonly ITeamService teamService;
        private readonly IJobLevelService jobLevelService;

        public EmployeeController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ICommonDataService commonDataService,
            IRoleService roleService,
            IAuthService authService,
            IEmployeeLogService employeeLogService,
            ITeamService teamService,
            IJobLevelService jobLevelService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.commonDataService = commonDataService;
            this.roleService = roleService;
            this.authService = authService;
            this.employeeLogService = employeeLogService;
            this.teamService = teamService;
            this.jobLevelService = jobLevelService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ContactList)]
        public ActionResult Index()
        {
            var model = new UserManagementViewModel
            {
                IsEnable = true,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage,
                TeamIds = string.Join(",", teamService.GetAll().Select(x => x.Id))
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.ContactList)]
        public ActionResult ContactList(UserManagementViewModel model)
        {
            IList<Employee> employees = new List<Employee>();
            if (model != null)
            {
                var criteria = new GetAllEmployeeCriteria
                {
                    Query = model.Keyword,
                    TeamIds = model.TeamIds,
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
                ViewBag.IsShowJobtitle = roleService.GetRolesByUserId(sessionService.Employee.Id).Any(r => r.Code == RoleSystem.HR.ToString() || r.Code == RoleSystem.SystemAdmin.ToString());
            }

            return PartialView("_ContactList", employees);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult UserManagement(int? jobTitleId = null, string teamIds = null)
        {
            teamIds = teamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
            IList<LookupData> listJobTitle = commonDataService.GetLookupDataByCode(Constants.JobTitle);
            IList<SelectListItem> jobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == (jobTitleId != null ? jobTitleId.ToString() : string.Empty)
            }).ToList();

            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                TeamIds = teamIds,
                IsEnable = true,
                IsLocal = GetCategory(),
                JobTitles = jobTitles,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage,
                IsShowDropdownCategory = sessionService.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read)
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
                    TeamIds = model.TeamIds,
                    EnableUser = model.IsEnable,
                    IsDeleteUser = model.IsDelete,
                    TitleId = model.JobTitleId,
                    IsLocalUser = GetCategory(model.IsLocal),
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
            Employee model = employeeService.GetEmployeeById(id);
            EmployeeShift employeeShift = employeeService.GetShiftByEmployeeId(id);

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
            return View(new CreateEmployeeViewModel());
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.CreateAccount)]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var param = new CreateEmployeeParam
                {
                    Code = model.Code,
                    DomainUsername = model.DomainUsername,
                    FullName = model.FullName,
                    Nickname = model.Nickname,
                    TeamId = model.TeamId
                };

                CommandResult commandResult = employeeService.Create(sessionService.Employee.Id, param);
                if (commandResult.IsSuccessful)
                {
                    return RedirectToAction("Edit", new { id = commandResult.OutputId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            Employee employee = employeeService.GetEmployeeById(id);
            if (employee != null)
            {
                if ((sessionService.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Host) && !employee.IsLocal)
                    && (sessionService.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Host) && employee.IsLocal))
                {
                    return RedirectToAction("AccessDenied", "Error");
                }

                employee.Coutries = commonDataService.GetAllCountry();
                IList<SupervisorNameModel> supervisors = GetSupervisors(id);
                employee.Verifier = GetVerifiers(supervisors);
                employee.Approver = GetApprovers(supervisors);
                employee.Disciplinarian = GetDisciplineSupervisors(supervisors);
                employee.EmpShiftList = GetShiftMonitors(supervisors);
                employee.ActingEmpList = GetActingSupervisors(supervisors);
                ViewBag.LengthOfService = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate);

                ComposeViewBagsForEditEmployee();
                return View(employee);
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
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
                    EditEmployeeParam param = MappingEditEmployeeParam(model);
                    CommandResult commandResult = employeeService.Edit(sessionService.Employee.Id, param);
                    if (commandResult.IsSuccessful)
                    {
                        ViewBag.Updated = true;
                        UpdateUserSession(model.Id, model.Code);
                    }
                    else
                    {
                        ViewBag.Updated = false;
                        ModelState.AddModelError(string.Empty, commandResult.Message);
                    }
                }

                model.Coutries = commonDataService.GetAllCountry();
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
        public ActionResult Delete(DeleteEmployeeParam param)
        {
            CommandResult commandResult = employeeService.Delete(sessionService.Employee.Id, param);

            return Json(commandResult);
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
                        const int JpegCompression = 100;
                        ////var resizer = new ResizeFilter(80, 80, ResizeMethod.Crop, AnchorLocation.Middle);
                        ////processor.AddFilter(resizer);
                        processor.OutputFormat = OutputFormat.Jpeg;
                        processor.JpegCompression = JpegCompression;
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

        [HttpGet]
        public ActionResult ManageSupervisor(int id, short roleId)
        {
            ViewBag.EmployeeId = id;
            ViewBag.RoleId = roleId;
            return View();
        }

        public CustomJsonResult GetEmployeeSupervisorByRoleId(int employeeId, RoleSystem roleId)
        {
            IEnumerable<SupervisorModel> supervisors = employeeService.GetEmployeeSupervisorByRoleId(employeeId, roleId);

            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public CustomJsonResult EditEmployeeSupervisor(int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
        {
            CommandResult commandResult = employeeService.EditEmployeeSupervisor(sessionService.Employee.Id, employeeId, roleId, supervisors);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult ShiftManager(int id)
        {
            ViewBag.EmployeeId = id;
            ViewBag.RoleId = (short)RoleSystem.ShiftManager;

            return View();
        }

        public CustomJsonResult GetShiftManagerByEmployeeId(int employeeId)
        {
            IEnumerable<SupervisorModel> supervisors = employeeService.GetEmployeeSupervisorByRoleId(employeeId, RoleSystem.ShiftManager);
            return new CustomJsonResult
            {
                Data = supervisors.Select(x => new Supervisor { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName }),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public void ToogleLeftNavigation(bool isShow)
        {
            sessionService.IsShowLeftNavigation = isShow;
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult DisciplineManager(int id)
        {
            ViewBag.EmployeeId = id;
            ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
            return View();
        }

        public CustomJsonResult GetDisciplineManagerByEmployeeId(int employeeId)
        {
            IEnumerable<Supervisor> supervisors = employeeService.GetDisciplineSettingByEmployeeId(employeeId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
            const byte TotalItems = 10;

            return Json(employees.Take(TotalItems), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployees(
            string query,
            string teamIds = "",
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null,
            bool? isLocal = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                TeamIds = teamIds,
                EnableUser = enable,
                IsDeleteUser = isDelete,
                TitleId = titleId,
                IsLocalUser = isLocal
            };
            IList<Employee> employees = employeeService.GetEmployees(criteria);

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

        public JsonResult GetMemberByGender(string keyword, int gender)
        {
            var employees = employeeService.GetEmployeeByGender(keyword, gender);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateEmployeeCode()
        {
            string newCode = employeeService.GenerateEmployeeCode();
            return Json(newCode, JsonRequestBehavior.AllowGet);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult ActivityLog(int id)
        {
            Employee employee = employeeService.GetEmployeeById(id);
            IEnumerable<LogObjectDetail> logs = employeeLogService.GetLogDetailById(id);
            ViewBag.Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName);

            return View("ActivityLog", logs);
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
            const byte MaxDisplayItems = 3;

            if (supervisors != null)
            {
                supervisorsFilter = supervisors.Where(x => x.ActionType == roleSystem).Take(MaxDisplayItems).ToList();
                supervisorsName = string.Join(", ", supervisorsFilter.Select(x => x.NickName));

                if (supervisorsFilter.Count > MaxDisplayItems)
                {
                    supervisorsName += ",...";
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
            return GetSupervisors(supervisors, (int)RoleSystem.OnBehalfRequest);
        }

        private static string GetDisciplineSupervisors(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (int)RoleSystem.DisciplineManager);
        }

        private void ComposeViewBagsForEditEmployee()
        {
            ViewBag.JobTitles = commonDataService.GetLookupDataByCode(Constants.JobTitle);
            ViewBag.JobLevel = jobLevelService.GetJobLevel();
            ViewBag.Marriages = Utilities.Marriages;
            ViewBag.Roles = roleService.GetRoles().Where(i => !i.Code.Equals(RoleSystem.Member.ToString()));
        }

        private EditEmployeeParam MappingEditEmployeeParam(Employee model)
        {
            return new EditEmployeeParam
            {
                EmployeeId = model.Id,
                Code = model.Code.Trim(),
                DomainUsername = model.DomainUsername,
                FullName = model.FullName,
                Nickname = model.NickName,
                EmployeeCard = model.EmployeeCard,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                JobTitleId = model.JobTitleId,
                JobLevelId = model.JobLevelId,
                Enabled = model.Enabled,
                Phone = model.Phone,
                Gender = model.Gender,
                Roles = BuildRole(model.Roles),
                JoinedDate = model.JoinedDate,
                CompanyDate = model.CompanyDate,
                ConfirmedDate = model.ConfirmedDate,
                ResignedDate = model.ResignedDate,
                Skype = model.Skype,
                LetsTalk = model.LetsTalk,
                Marriage = model.Marriage,
                PermanentAddress = model.Address,
                CurrentAddress = model.CurrentAddress,
                IdCardNumber = model.CardNo,
                PassportNo = model.PassportNo,
                PassportExp = model.PassportExp,
                TeamId = model.Team.Id,
                IsLocal = model.IsLocal,
                CountryCode = model.CountryCode,
                CanOT = model.CanOT,
                SpecialOT = model.SpecialOT,
                WorkingDays = model.WorkingDays,
                WorkingHours = model.WorkingHours,
                IsTopManager = model.IsTopManager,
                OffDaysbyWeek = model.OffDaysbyWeek,
                Unit = model.Unit,
                PlaceOfBirth = model.PlaceOfBirth,
                Ethnic = model.Ethnic,
                Native = model.Native,
                Religion = model.Religion,
                DateOfIdCard = model.DateOfIdCard,
                PlaceOfIdCard = model.PlaceOfIdCard,
                HomePhone = model.HomePhone,
                Qualification = model.Qualification,
                Major = model.Major,
                IsExpatUseHoliday = !model.IsLocal && model.IsExpatUseHoliday,
                PickupRemark = model.PickupRemark,
                PromotionDate = model.PromotionDate,
                TeamDate = model.TeamDate,
                MilitaryFromDate = model.MilitaryFromDate,
                MilitaryToDate = model.MilitaryToDate
            };
        }

        private static string BuildRole(IList<SystemRoleModel> roles)
        {
            var result = string.Empty;
            if (roles != null)
            {
                result = string.Join(",", roles.Select(i => i.RoleId));
            }

            return string.IsNullOrEmpty(result) ? BuildDefaultRole() : BuildDefaultRole() + "," + result;
        }

        private static string BuildDefaultRole()
        {
            return ((int)RoleSystem.Member).ToString();
        }

        private bool? GetCategory(bool? isLocal = null)
        {
            bool? category = isLocal;
            if (!sessionService.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read))
            {
                if (sessionService.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Read))
                {
                    category = true;
                }
                else
                {
                    if (sessionService.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Read))
                    {
                        category = false;
                    }
                }
            }

            return category;
        }
    }
}