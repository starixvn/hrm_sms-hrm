namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
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
    using CommandResult = Models.CommandResult;

    [Authorize]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
    public class EmployeeController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly ILookupDataService lookupDataService;
        private readonly IShiftService shiftService;
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;

        public EmployeeController()
            : this(null, null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            ITeamService teamService,
            ILookupDataService lookupDataService,
            IShiftService shiftService,
            ICommandService commandService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.teamService = teamService ?? new TeamService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.shiftService = shiftService ?? new ShiftService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            ViewBag.Groups = GetAllTeam();
            return View();
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult UserManagement(int? jobTitleId = null, string groupIds = "")
        {
            IList<Team> groups = GetAllTeam();
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
                Groups = groups,
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

            return PartialView("_ListEmployee", employees);
        }

        public ActionResult Detail(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
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
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.EmployeeManagement, Policy.Edit);

            return View(model);
        }

        /// [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(Policy.Create, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            ViewBag.Shifts = shiftService.GetSelectList();
            IList<Team> teamList = GetAllTeam();
            ViewBag.TeamList = teamList;

            return View(new CreateEmployeeViewModel());
        }

        /// [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        [HRAuthorize(Policy.Create, FunctionKey.EmployeeManagement)]
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
                Password = model.Password,
                SatShift = model.SatShift,
                SunShift = model.SunShift,
                ThuShift = model.ThuShift,
                TueShift = model.TueShift,
                WedShift = model.WedShift
            };
            CommandResult result = employeeService.Insert(insertEmployeeCriteria);
            if (result.Code == 0)
            {
                var criteria = new GetEmployeeByCodeCriteria() { Code = model.Code };
                Employee employee = employeeService.GetEmployeeByCode(criteria);
                return RedirectToAction("Edit", new { id = employee.Id });
            }

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Edit, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            string verifier = string.Empty,
                approver = string.Empty,
                shiftManager = string.Empty,
                actingSupervisor = string.Empty,
                disciplinarian = string.Empty;

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            Employee model = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var getAllCountryCriteria = new GetAllCountryCriteria();
            IList<Country> countryList = employeeService.GetAllCountry(getAllCountryCriteria);
            model.ListCountryCode = countryList.Select(x => new SelectListItem { Text = x.CountryName, Value = x.CountryCode }).ToList();

            CheckAdminUserRole();
            GetListSupervisorName(id, out verifier, out approver, out shiftManager, out actingSupervisor, out disciplinarian);
            model.Verifier = verifier;
            model.Approver = approver;
            model.Disciplinarian = disciplinarian;
            model.EmpShiftList = shiftManager;
            model.ActingEmpList = actingSupervisor;
            this.ComposeViewBagsForEditEmployee();

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRAuthorize(Policy.Edit, FunctionKey.EmployeeManagement)]
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
            CheckRoleUser(model);

            if (ModelState.IsValid)
            {
                UpdateEmployeeCriteria updateEmployeeCriteria = SetUpdateEmployeeCriteria(model);
                CommandResult result = employeeService.Update(updateEmployeeCriteria);
                ViewBag.Updated = result.Code == 0;

                if (result.Code == 0 && this.CurrentUser.Id == model.Id)
                {
                    var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.Id };
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

            CheckAdminUserRole();
            this.ComposeViewBagsForEditEmployee();

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Edit, FunctionKey.EmployeeManagement)]
        public ActionResult DefaultSchedule(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
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
        [HRAuthorize(Policy.Edit, FunctionKey.EmployeeManagement)]
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
                CommandResult result = employeeService.UpdateEmployeeShift(updateEmployeeShiftCriteria);
                ViewBag.Updated = result.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpID };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            ViewBag.Shifts = shiftService.GetSelectList();
            ViewBag.Id = model.EmpID;

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Edit, FunctionKey.EmployeeManagement)]
        public ActionResult MonthSchedule(int id)
        {
            ViewBag.Id = id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
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
        [HRAuthorize(Policy.Edit, FunctionKey.EmployeeManagement)]
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
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = employeeId };
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = employeeId, Date = DateTime.Now };
            MySpaceModel model = employeeService.GetMySpace(getMySpaceCriteria);

            Employee employee = this.employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.ResetPassword = employee.PasswordReset;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (this.CurrentUser.HasPermission(FunctionKey.VerifyOvertime, Policy.Edit)
                || CurrentUser.HasPermission(FunctionKey.ApproveOvertime, Policy.Edit))
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
        [HRAuthorize(Policy.Create, FunctionKey.SectionManagement)]
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
        public ActionResult ChangePwd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var changePasswordCriteria = new ChangePasswordCriteria()
                {
                    EmpId = this.CurrentUser.Id,
                    Password = model.Password,
                    NewPassword = model.NewPassword
                };
                CommandResult result = employeeService.ChangePassword(changePasswordCriteria);
                switch (result.Code)
                {
                    case 0: // OK
                        ViewBag.ChangePwdOK = Resources.Messages.ChangePwdOK;
                        break;

                    case 1: // deleted
                        ModelState.AddModelError(string.Empty, Resources.Messages.DeletedUser);
                        break;

                    case 2: // disabled
                        ModelState.AddModelError(string.Empty, Resources.Messages.DisabledUser);
                        break;

                    default: // wrong password
                        ModelState.AddModelError(string.Empty, Resources.Messages.WrongPassword);
                        break;
                }
            }

            return View(model);
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
                ModifiedBy = this.CurrentUser.Id
            };
            CommandResult commandResult = employeeService.UpdateSupervisor(updateSupervisorCriteria);
            bool updated = commandResult.Code == 0;

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
            CommandResult result = employeeService.InsertEmployeeShiftManager(insertShiftMangerCriteria);

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
                SupervisorRole = (short)RoleSystem.EditDiscipline
            };

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Edit, FunctionKey.EmployeeManagement)]
        public ActionResult UpdateDisciplineSetting(SupervisorViewModel model)
        {
            var criteria = new UpdateSupervisorCriteria
            {
                EmpIdDisciplines = model.RoleDisciplines,
                EmpId = model.EmployeeId,
                ModifiedBy = CurrentUser.Id,
                Role = (short)RoleSystem.EditDiscipline
            };
            CommandResult result = commandService.Update(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void CheckRoleUser(Employee model)
        {
            if (model.RoleList != null && model.RoleList.Any())
            {
                if (model.RoleList.Any(x => x == (int)RoleSystem.SystemAdmin))
                {
                    var roleList = Enum.GetValues(typeof(RoleSystem)).Cast<RoleSystem>()
                        .Where(x => (int)x != (int)RoleSystem.Member && (int)x != (int)RoleSystem.EditDiscipline)
                        .Select(x => (int)x).ToList();
                    roleList.AddRange(model.RoleList.Where(x => x != (int)RoleSystem.SystemAdmin).Select(x => (int)x));
                    model.Roles = string.Join(",", roleList);
                }
            }

            if (model.RoleList == null)
            {
                var role = (int)RoleSystem.Member;
                model.Roles = role.ToString();
            }
        }

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
            var getListSupervisor = new GetListSupervisorNameCriteria()
            {
                EmpId = id,
                Role = null,
                RequestType = null,
            };
            IList<SupervisorNameModel> supervisors = employeeService.GetListSupervisorName(getListSupervisor);
            if (supervisors != null)
            {
                verifierList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Verifier).ToList();
                approverList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Approver).ToList();
                shiftMonitorList = supervisors.Where(x => x.ActionType == (int)RoleSystem.ShiftManager).ToList();
                actingList = supervisors.Where(x => x.ActionType == (int)RoleSystem.ActingSupervisor).ToList();
                disciplinarianList = supervisors.Where(x => x.ActionType == (int)RoleSystem.EditDiscipline).ToList();
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

        private void CheckAdminUserRole()
        {
            var authorize = false;
            var currentUserRole = this.CurrentUser.RoleList;
            if (currentUserRole != null)
            {
                authorize = currentUserRole.Any(x => x == (int)RoleSystem.SystemAdmin);
            }

            ViewBag.Authorize = authorize;
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

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                Password = model.Password,
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
                IsExpatUseHoliday = model.IsExpatUseHoliday
            };

            return criteria;
        }
    }
}