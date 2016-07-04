namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
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
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Employee;
    using Fanex.NexHR.ViewModels.EmployeeGroup;
    using Fanex.NexHR.ViewModels.Supervisor;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using CommandResult = Models.CommandResult;

    public class EmployeeController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly ILookupDataService lookupDataService;
        private readonly IShiftService shiftService;
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;
        private readonly IRoleService roleService;

        public EmployeeController()
            : this(null, null, null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            ITeamService teamService,
            ILookupDataService lookupDataService,
            IShiftService shiftService,
            ICommandService commandService,
            IRoleService roleService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.teamService = teamService ?? new TeamService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.shiftService = shiftService ?? new ShiftService();
            this.commandService = commandService ?? new CommandService();
            this.roleService = roleService ?? new RoleService();
        }

        public ActionResult Index()
        {
            var model = new UserManagementViewModel()
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
                var criteria = new GetAllEmployeeCriteria()
                {
                    Query = model.Keyword,
                    GroupIds = model.GroupIds,
                    EnableUser = model.IsEnable,
                    IsLocalUser = model.IsLocal,
                    PageNumber = model.CurrentPage,
                    PageSize = model.ItemsPerPage
                };

                employees = employeeService.GetEmployees(criteria);

                var pagerOption = new PagerOptionModel()
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalRow
                };
                ViewBag.PagerOption = pagerOption;
            }

            return PartialView("_ContactList", employees);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult UserManagement(int? jobTitleId = null, string groupIds = "")
        {
            IList<LookupData> listJobTitle = GetAllJobTitle();
            IList<SelectListItem> jobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == (jobTitleId != null ? jobTitleId.ToString() : string.Empty)
            }).ToList();

            var model = new UserManagementViewModel()
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
        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult ListEmployee(UserManagementViewModel model)
        {
            IList<Employee> employees = new List<Employee>();

            if (model != null)
            {
                var criteria = new GetAllEmployeeCriteria()
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

                var pagerOption = new PagerOptionModel()
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalRow
                };
                ViewBag.PagerOption = pagerOption;
            }

            ViewBag.HasPermissionToEdit = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Host);
            ViewBag.HasPermissionToSchedule = CurrentUser.HasPermission(FunctionKey.ScheduleArrange, Policy.Host);

            return PartialView("_ListEmployee", employees);
        }

        public ActionResult Detail(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = id };
            var model = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.JobTitle = (model.JobTitleId == null)
            ? Labels.Unknown
            : lookupDataService.GetLookupByCodeAndId(Constants.JobTitle, model.JobTitleId.Value).Name;

            ViewBag.DefaultCalendar = model.IsDefaultSchedule;
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = id, Date = DateTime.Now };
            ViewBag.Working = employeeService.GetMySpace(getMySpaceCriteria);
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
            EmployeeShift workingCalendar = employeeService.GetEmployeeShift(employeeShiftCriteria);
            var getCountryByCodeCriteria = new GetCountryByCodeCriteria() { CountryCode = model.CountryCode };
            Country countryName = employeeService.GetCountryByCode(getCountryByCodeCriteria);
            if (countryName != null)
            {
                ViewBag.CountryName = countryName.CountryName;
            }

            ViewBag.Sun = workingCalendar.SunShiftName;
            ViewBag.Mon = workingCalendar.MonShiftName;
            ViewBag.Tue = workingCalendar.TueShiftName;
            ViewBag.Wed = workingCalendar.WedShiftName;
            ViewBag.Thu = workingCalendar.ThuShiftName;
            ViewBag.Fri = workingCalendar.FriShiftName;
            ViewBag.Sat = workingCalendar.SatShiftName;
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Host);

            return View(model);
        }

        /// [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            ViewBag.Shifts = shiftService.GetSelectList();
            IList<Team> teamList = GetAllTeam();
            ViewBag.TeamList = teamList;

            return View(new CreateEmployeeViewModel());
        }

        /// [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            ViewBag.Shifts = shiftService.GetSelectList();
            IList<Team> teamList = GetAllTeam();
            model.ProcessBy = this.CurrentUser.Id;
            model.Password = model.Code;
            ViewBag.TeamList = teamList;
            var insertEmployeeCriteria = new InsertEmployeeCriteria
            {
                Code = model.Code,
                CreateBy = model.ProcessBy,
                CreatedDate = DateTime.Now,
                DomainUserName = model.DomainUsername,
                FriShift = model.FriShift,
                FullName = model.FullName,
                IsDefaultSchedule = model.IsDefaultSchedule,
                MainGroupID = model.TeamId,
                MonShift = model.MonShift,
                NickName = model.NickName,
                Password = Encryptor.ComputeHash(employeeService.HashProvider, model.Password),
                SatShift = model.SatShift,
                SunShift = model.SunShift,
                ThuShift = model.ThuShift,
                TueShift = model.TueShift,
                WedShift = model.WedShift
            };

            CommandResult result = commandService.Insert(insertEmployeeCriteria);
            if (result.Code == 0)
            {
                var criteria = new GetEmployeeByCodeCriteria() { Code = model.Code };
                Employee employee = employeeService.GetEmployeeByCode(criteria);
                return RedirectToAction("Edit", new { id = employee.Id });
            }

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            string verifier = string.Empty,
                approver = string.Empty,
                shiftManager = string.Empty,
                actingSupervisor = string.Empty,
                disciplinarian = string.Empty;

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = id };
            Employee model = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var getAllCountryCriteria = new GetAllCountryCriteria();
            IList<Country> countryList = employeeService.GetAllCountry(getAllCountryCriteria);
            model.ListCountryCode = countryList.Select(x => new SelectListItem { Text = x.CountryName, Value = x.CountryCode }).ToList();

            GetListSupervisorName(id, out verifier, out approver, out shiftManager, out actingSupervisor, out disciplinarian);
            model.Verifier = verifier;
            model.Approver = approver;
            model.Disciplinarian = disciplinarian;
            model.EmpShiftList = shiftManager;
            model.ActingEmpList = actingSupervisor;
            this.ComposeViewBagsForEditEmployee();
            ViewBag.Roles = roleService.GetRoles().Where(i => !i.RoleCode.Equals(RoleSystem.Member.ToString()));

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(Employee model)
        {
            string verifier = string.Empty,
                approver = string.Empty,
                shiftManager = string.Empty,
                actingSupervisor = string.Empty,
                disciplinarian = string.Empty;

            var getAllCountryCriteria = new GetAllCountryCriteria();
            IList<Country> countryList = employeeService.GetAllCountry(getAllCountryCriteria);
            model.Modifier = ConvertUserToEmployee(this.CurrentUser);
            model.ListCountryCode = new List<SelectListItem>();
            model.Roles = BuildRole(model.Roles);

            if (ModelState.IsValid)
            {
                UpdateEmployeeCriteria updateEmployeeCriteria = SetUpdateEmployeeCriteria(model);
                CommandResult result = commandService.Update(updateEmployeeCriteria);
                ViewBag.Updated = result.Code == 0;

                if (result.Code == 0 && this.CurrentUser.Id == model.Id)
                {
                    var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = model.Id };
                    UserLoginModel currentUser = employeeService.GetUserLoginByEmployeeId(getEmployeeByIdCriteria);
                    HttpContext.Session["CurrentUser"] = currentUser;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            model.ListCountryCode = countryList.Select(x => new SelectListItem { Text = x.CountryName, Value = x.CountryCode }).ToList();

            GetListSupervisorName(model.Id, out verifier, out approver, out shiftManager, out actingSupervisor, out disciplinarian);
            model.Verifier = verifier;
            model.Approver = approver;
            model.Disciplinarian = disciplinarian;
            model.EmpShiftList = shiftManager;
            model.ActingEmpList = actingSupervisor;

            this.ComposeViewBagsForEditEmployee();
            ViewBag.Roles = roleService.GetRoles().Where(i => !i.RoleCode.Equals(RoleSystem.Member.ToString()));

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public bool Delete(int id, DateTime resignedDate)
        {
            var deleteEmployeeCriteria = new DeleteEmployeeCriteria
            {
                EmpId = id,
                ResignedDate = resignedDate
            };
            CommandResult result = commandService.Delete(deleteEmployeeCriteria);

            return result.Code == 0;
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult DefaultSchedule(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = id };
            Employee employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            ViewBag.Employee = employee;
            ViewBag.Shifts = shiftService.GetSelectList();
            ViewBag.Id = id;
            if (employee.IsDefaultSchedule)
            {
                var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
                EmployeeShift model = employeeService.GetEmployeeShift(employeeShiftCriteria);
                return View(model);
            }
            else
            {
                return View("MonthSchedule");
            }
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult DefaultSchedule(EmployeeShift model)
        {
            if (ModelState.IsValid)
            {
                var updateEmployeeShiftCriteria = new UpdateEmployeeShiftCriteria()
                {
                    EmpId = model.EmpID,
                    FriShift = model.FriShiftId,
                    MonShift = model.MonShiftId,
                    SatShift = model.SatShiftId,
                    SunShift = model.SunShiftId,
                    ThuShift = model.ThuShiftId,
                    TueShift = model.TueShiftId,
                    WedShift = model.WedShiftId
                };
                CommandResult result = commandService.Update(updateEmployeeShiftCriteria);
                ViewBag.Updated = result.IsSuccessful;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = model.EmpID };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            ViewBag.Shifts = shiftService.GetSelectList();
            ViewBag.Id = model.EmpID;

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult MonthSchedule(int id)
        {
            ViewBag.Id = id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = id };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.Shifts = shiftService.GetSelectList();

            return View();
        }

        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = this.CurrentUser.Code + ".jpg";
            }
            else
            {
                id += ".jpg";
            }

            if (System.IO.File.Exists(Server.MapPath("/avatars/" + id)))
            {
                return File("/avatars/" + id, "image/jpeg");
            }

            return File("/avatars/avatar.jpg", "image/jpeg");
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public JsonResult UploadAvatar(string code, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var stream = new MemoryStream();
                var processor = new ImageProcessor();
                var resizer = new ResizeFilter(64, 64, ResizeMethod.Crop, AnchorLocation.Middle);
                processor.AddFilter(resizer);
                processor.OutputFormat = OutputFormat.Jpeg;
                processor.JpegCompression = 100;
                processor.ProcessImage(file.InputStream, stream);

                var image = Image.FromStream(stream);
                image.Save(Server.MapPath("/avatars/") + code + ".jpg");

                return Json(true);
            }

            return Json(null);
        }

        public ActionResult MySpace()
        {
            var employeeId = this.CurrentUser.Id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { EmpId = employeeId };
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = employeeId, Date = DateTime.Now };
            MySpaceModel model = employeeService.GetMySpace(getMySpaceCriteria);

            Employee employee = this.employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.ResetPassword = employee.PasswordReset;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (this.CurrentUser.HasPermission(FunctionKey.VerifyRequestOfMember, Policy.Host))
            {
                var getHRLeaderDashboardCriteria = new GetHRLeaderDashboardCriteria
                {
                    Empid = this.CurrentUser.Id,
                    Date = DateTime.Now
                };
                IList<HRLeaderDashboardModel> model = employeeService.GetHRLeaderDashboard(getHRLeaderDashboardCriteria);

                return PartialView("MultiRoleManagementDashboard", model);
            }

            return Content(string.Empty);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult TeamManagement()
        {
            IList<Team> teams = GetAllTeam();

            return View(teams);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult AddTeam(int teamId = -1)
        {
            TeamViewModel model = null;

            // create new
            if (teamId == -1)
            {
                model = new TeamViewModel
                {
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };
            }
            else
            {
                var criteria = new GetGroupByIdCriteria
                {
                    GroupId = teamId
                };
                Team team = teamService.GetById(criteria);

                if (team != null)
                {
                    model = new TeamViewModel
                    {
                        Id = teamId,
                        TeamCode = team.Code,
                        TeamName = team.Name,
                        AssistantLeaderId = team.AssistantLeader.Id,
                        AssistantLeader = team.AssistantLeader.DisplayName,
                        LeaderId = team.Leader.Id,
                        Leader = team.Leader.DisplayName
                    };
                }
                else
                {
                    // throw exception
                }
            }

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Supervisor(short role, int employeeId)
        {
            var getEmployeeSupervisorCriteria = new GetEmployeeSupervisorCriteria()
            {
                EmpId = employeeId,
                Role = role
            };
            IList<SupervisorModel> supervisorModel = employeeService.GetEmployeeSupervisor(getEmployeeSupervisorCriteria);
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
            var updateSupervisorCriteria = new UpdateSupervisorCriteria()
            {
                EmpId = model.EmployeeId,
                Role = model.SupervisorRole,
                EmpIdLeaves = model.RoleLeaves,
                EmpIdChangeShifts = model.RoleChangeShifts,
                EmpIdLateInEarlyOuts = model.RoleLateInEarlyOuts,
                EmpIdCheckInOuts = model.RoleCheckInOuts,
                EmpIdOvertimes = model.RoleOvertimes,
                EmpIdKeepInViews = model.RoleKeepInViews,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult commandResult = commandService.Update(updateSupervisorCriteria);
            bool updated = commandResult.IsSuccessful;

            return Json(new { result = updated }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ShiftManager(short role, int employeeId)
        {
            var getEmployeeSupervisorCriteria = new GetEmployeeSupervisorCriteria()
            {
                EmpId = employeeId,
                Role = role
            };
            IList<SupervisorModel> shiftManagerModel = employeeService.GetEmployeeSupervisor(getEmployeeSupervisorCriteria);

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
            bool success = true;
            var insertShiftMangerCriteria = new InsertShiftManagerCriteria()
            {
                EmpId = model.EmployeeId,
                EmpIdShift = model.RoleShiftArrange,
                ModifiedBy = this.CurrentUser.Id
            };
            CommandResult result = commandService.Insert(insertShiftMangerCriteria);

            if (result.Code != 0)
            {
                success = false;
            }

            return Json(new { result = success }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ToogleLeftNavigation(bool isShow)
        {
            var currentUser = (UserLoginModel)HttpContext.Session["CurrentUser"];
            currentUser.IsShowLeftNavigation = isShow;
            HttpContext.Session["CurrentUser"] = currentUser;
        }

        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult DisciplineSetting(int employeeId)
        {
            ViewBag.Groups = GetAllTeam();

            var criteria = new GetDisciplineSettingByEmployeeId()
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
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult UpdateDisciplineSetting(SupervisorViewModel model)
        {
            var criteria = new UpdateSupervisorCriteria
            {
                EmpIdDisciplines = model.RoleDisciplines,
                EmpId = model.EmployeeId,
                ModifiedBy = CurrentUser.Id,
                Role = (short)RoleSystem.DisciplineManager
            };
            CommandResult result = commandService.Update(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberByManager(string keyword)
        {
            var criteria = new GetMemberByManagerCriteria()
            {
                SupervisorId = this.CurrentUser.Id,
                Query = keyword,
                ExcludedManager = false
            };
            IList<Employee> employees = employeeService.GetMemberByManager(criteria);

            return Json(employees.Take(10), JsonRequestBehavior.AllowGet);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void GetListSupervisorName(
            int id,
            out string verifier,
            out string approver,
            out string shiftManager,
            out string actingSupervisor,
            out string disciplinarian)
        {
            List<string> listVerifierName = null, listApproverName = null, listShiftName = null, listActingName = null, listDisciplinarianName = null;
            List<SupervisorNameModel> verifierList = null, approverList = null, shiftMonitorList = null, actingList = null, disciplinarianList = null;
            string displayMore = "...";
            var getListSupervisor = new GetListSupervisorNameCriteria() { EmpId = id, Role = null, RequestType = null };
            var supervisors = employeeService.GetListSupervisorName(getListSupervisor);
            if (supervisors != null)
            {
                verifierList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Verifier).ToList();
                approverList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Approver).ToList();
                shiftMonitorList = supervisors.Where(x => x.ActionType == (int)RoleSystem.ShiftManager).ToList();
                actingList = supervisors.Where(x => x.ActionType == (int)RoleSystem.ActingSupervisor).ToList();
                disciplinarianList = supervisors.Where(x => x.ActionType == (int)RoleSystem.DisciplineManager).ToList();
            }

            listVerifierName = verifierList.Select(x => x.NickName).Take(3).ToList();
            if (verifierList.Count > 3)
            {
                listVerifierName.Add(displayMore);
            }

            verifier = string.Join(", ", listVerifierName);
            listApproverName = approverList.Select(x => x.NickName).Take(3).ToList();
            if (approverList.Count > 3)
            {
                listApproverName.Add(displayMore);
            }

            approver = string.Join(", ", listApproverName);
            listShiftName = shiftMonitorList.Select(x => x.NickName).Take(3).ToList();
            if (shiftMonitorList.Count > 3)
            {
                listShiftName.Add(displayMore);
            }

            shiftManager = string.Join(", ", listShiftName);
            listActingName = actingList.Select(x => x.NickName).Take(3).ToList();
            if (actingList.Count > 3)
            {
                listActingName.Add(displayMore);
            }

            actingSupervisor = string.Join(", ", listActingName);
            listDisciplinarianName = disciplinarianList.Select(x => x.NickName).Take(3).ToList();
            if (disciplinarianList.Count > 3)
            {
                listDisciplinarianName.Add(displayMore);
            }

            disciplinarian = string.Join(", ", listDisciplinarianName);
        }

        private Employee ConvertUserToEmployee(UserLoginModel userLogin)
        {
            var employee = new Employee
            {
                Id = userLogin.Id,
                Name = userLogin.Name,
                NickName = userLogin.NickName,
                FullName = userLogin.FullName,
                Code = userLogin.Code
            };

            return employee;
        }

        [NonAction]
        private DateTime GetEndOfWeek(DateTime currentDate)
        {
            var endOfWeek = GetStartOfWeek(currentDate).AddDays(6);
            return endOfWeek;
        }

        [NonAction]
        private DateTime GetStartOfWeek(DateTime currentDate)
        {
            int diff = currentDate.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }

            return currentDate.AddDays(-1 * diff).Date;
        }

        private void ComposeViewBagsForEditEmployee()
        {
            ViewBag.Groups = GetAllTeam();
            ViewBag.JobTitles = GetAllJobTitle();

            ViewBag.Marriages = Utilities.Marriages;
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

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                Roles = model.Roles,
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
                ModifiedBy = model.Modifier.Id,
                Password = Encryptor.ComputeHash(employeeService.HashProvider, model.Password),
                GroupId = model.Team.Id,
                IsLocal = model.IsLocal,
                CountryCode = model.CountryCode,
                CanOT = model.CanOT,
                IsDefaultSchedule = model.IsDefaultSchedule,
                WorkingDays = model.WorkingDays,
                WorkingHours = model.WorkingHours,
                IsTopManager = model.IsTopManager,
                OffDaysbyWeek = model.OffDaysbyWeek,
                Unit = model.Unit,
                PlaceOfbirth = model.PlaceOfBirth,
                Ethnic = model.Ethnic,
                Native = model.Native,
                Religion = model.Religion,
                DateOfIDCard = model.DateOfIdCard,
                PlaceOfIDCard = model.PlaceOfIdCard,
                HomePhone = model.HomePhone,
                Qualification = model.Qualification,
                Major = model.Major,
                BankAccount = model.BankAccount,
                TaxCode = model.TaxCode,
                SocialInsuranceNo = model.SocialInsuranceNo,
                RegisteredHospital = model.RegisteredHospital,
                IsExpatUseHoliday = model.IsLocal ? false : model.IsExpatUseHoliday
            };

            return criteria;
        }

        private string BuildRole(string roles)
        {
            if (roles == ((int)RoleSystem.SystemAdmin).ToString())
            {
                roles = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6}",
                    ((int)RoleSystem.ActingSupervisor).ToString(),
                    ((int)RoleSystem.Approver).ToString(),
                    ((int)RoleSystem.DisciplineManager).ToString(),
                    ((int)RoleSystem.HR).ToString(),
                    ((int)RoleSystem.ShiftManager).ToString(),
                    ((int)RoleSystem.SystemAdmin).ToString(),
                    ((int)RoleSystem.Verifier).ToString());
            }

            if (roles == ((int)RoleSystem.DisciplineManager).ToString())
            {
                roles = string.Format("{0},{1}", ((int)RoleSystem.Approver).ToString(), ((int)RoleSystem.DisciplineManager).ToString());
            }

            roles = string.IsNullOrEmpty(roles) ? ((int)RoleSystem.Member).ToString() : string.Format("{0},{1}", ((int)RoleSystem.Member).ToString(), roles);

            return roles;
        }
    }
}