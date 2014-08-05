// --------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.7.3">Created</change>
//   <change who="Anders.Le" date="2012.7.3">Modify avatar function</change>
// </history>
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
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.DashBoard;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeSupervisor;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.MySpace;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    /// <summary>
    /// Employee Controller.
    /// </summary>
    [Authorize]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
    public class EmployeeController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
            : this(null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService = null,
            ITeamService teamService = null,
            ILookupDataService lookupDataService = null,
            IShiftService shiftService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _shiftService = shiftService ?? new ShiftService();
        }

        public ActionResult Index()
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            return View();
        }

        /// <summary>
        /// ActionResult UserManagement.
        /// </summary>
        /// <param name="jobTitleId">Job Title Id.</param>
        /// <param name="groupIds">Group Identify.</param>
        /// <returns>View User Management.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult UserManagement(string jobTitleId = "", string groupIds = "")
        {
            ViewBag.JobTitleId = jobTitleId;
            ViewBag.GroupId = groupIds;
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);

            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = LookupCodename.JobTitle
            };
            IList<LookupData> listJobTitle = _lookupDataService.GetAll(getAllJobTitleCriteria);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == jobTitleId
            }).ToList();

            return View();
        }

        /// <summary>
        /// ActionResult Detail.
        /// </summary>
        /// <param name="id">Employee's id.</param>
        /// <returns>View Details.</returns>
        public ActionResult Detail(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            var model = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.JobTitle = (model.JobTitleId == null)
            ? Resources.Labels.Unknown
            : _lookupDataService.GetLookupByCodeAndId(LookupCodename.JobTitle, model.JobTitleId.Value).Name;

            ViewBag.DefaultCalendar = model.IsDefaultSchedule;
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = id, Date = DateTime.Now };
            ViewBag.Working = _employeeService.GetMySpace(getMySpaceCriteria);
            var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
            var workingCalendar = _employeeService.GetEmployeeShift(employeeShiftCriteria);
            var getCountryByCodeCriteria = new GetCountryByCodeCriteria() { CountryCode = model.CountryCode };
            var countryName = _employeeService.GetCountryByCode(getCountryByCodeCriteria);
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

            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "SystemAdmin")]
        public ActionResult Create()
        {
            ViewBag.Shifts = _shiftService.GetSelectList();
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teamList = _teamService.GetAll(getAllGroupCriteria);
            ViewBag.TeamList = teamList;

            return View(new CreateEmployeeViewModel());
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">CreateEmployee ViewModel.</param>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            ViewBag.Shifts = _shiftService.GetSelectList();
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teamList = _teamService.GetAll(getAllGroupCriteria);
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
            CommandResult result = _employeeService.Insert(insertEmployeeCriteria);
            if (result.Code == 0)
            {
                var criteria = new GetEmployeeByCodeCriteria() { Code = model.Code };
                Employee employee = _employeeService.GetEmployeeByCode(criteria);
                return RedirectToAction("Edit", new { id = employee.Id });
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="id">Employee's id.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            string verifier;
            string approver;
            string shiftManager;
            string actingSupervisor;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            var model = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            var getAllCountryCriteria = new GetAllCountryCriteria();
            var countryList = _employeeService.GetAllCountry(getAllCountryCriteria);
            model.ListCountryCode = countryList.Select(x => new SelectListItem { Text = x.CountryName, Value = x.CountryCode }).ToList();

            CheckAdminUserRole();
            GetListSupervisorName(id, out verifier, out approver, out shiftManager, out actingSupervisor);
            model.Verifier = verifier;
            model.Approver = approver;
            model.EmpShiftList = shiftManager;
            model.ActingEmpList = actingSupervisor;
            this.ComposeViewBagsForEditEmployee();

            return View(model);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Employee model.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee model)
        {
            string verifier, approver, shiftManager, actingSupervisor;
            var getAllCountryCriteria = new GetAllCountryCriteria();
            var countryList = _employeeService.GetAllCountry(getAllCountryCriteria);
            model.Modifier = ConvertUserToEmployee(this.CurrentUser);
            model.ListCountryCode = new List<SelectListItem>();
            CheckRoleUser(model);

            if (ModelState.IsValid)
            {
                UpdateEmployeeCriteria updateEmployeeCriteria = SetUpdateEmployeeCriteria(model);
                CommandResult result = _employeeService.Update(updateEmployeeCriteria);
                ViewBag.Updated = result.Code == 0;

                if (result.Code == 0 && this.CurrentUser.Id == model.Id)
                {
                    var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.Id };
                    var currentUser = _employeeService.GetUserLoginByEmployeeId(getEmployeeByIdCriteria);
                    HttpContext.Session["CurrentUser"] = currentUser;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            model.ListCountryCode = countryList.Select(x => new SelectListItem { Text = x.CountryName, Value = x.CountryCode }).ToList();

            GetListSupervisorName(model.Id, out verifier, out approver, out shiftManager, out actingSupervisor);
            model.Verifier = verifier;
            model.Approver = approver;
            model.EmpShiftList = shiftManager;
            model.ActingEmpList = actingSupervisor;

            CheckAdminUserRole();
            this.ComposeViewBagsForEditEmployee();

            return View(model);
        }

        /// <summary>
        /// ActionResult View Default Schedule.
        /// </summary>
        /// <param name="id">Employee's id.</param>
        /// <returns>View according to schedule.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult DefaultSchedule(int id)
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            Employee employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            ViewBag.Employee = employee;
            ViewBag.Shifts = _shiftService.GetSelectList();
            if (employee.IsDefaultSchedule == true)
            {
                var employeeShiftCriteria = new GetEmployeeShiftCriteria() { EmpId = id };
                var model = _employeeService.GetEmployeeShift(employeeShiftCriteria);
                return View(model);
            }
            else
            {
                ViewBag.Id = id;
                return View("MonthSchedule");
            }
        }

        /// <summary>
        /// ActionResult DefaultSchedule.
        /// </summary>
        /// <param name="model">EmployeeShift model.</param>
        /// <returns>View DefaultSchedule.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
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
                CommandResult result = _employeeService.UpdateEmployeeShift(updateEmployeeShiftCriteria);
                ViewBag.Updated = result.Code == 0;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = model.EmpID };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);

            ViewBag.Shifts = _shiftService.GetSelectList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult MonthSchedule(int id)
        {
            ViewBag.Id = id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = id };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.Shifts = _shiftService.GetSelectList();

            return View();
        }

        /// <summary>
        /// FileResult Avatar.
        /// </summary>
        /// <param name="id">Image name and format.</param>
        /// <returns>File name.</returns>
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

        /// <summary>
        /// Action UploadAvatar.
        /// </summary>
        /// <param name="code">File name.</param>
        /// <param name="file">Http Posted File Base.</param>
        /// <returns>Type of format.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
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

        /// <summary>
        /// Get My Space.
        /// </summary>
        /// <returns>Action Result.</returns>
        public ActionResult MySpace()
        {
            var employeeId = this.CurrentUser.Id;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = employeeId };
            var getMySpaceCriteria = new GetMySpaceCriteria() { EmpId = employeeId, Date = DateTime.Now };
            var getDatesMissingRequestCriteria = new GetEmployeeMissingRequestDateCriteria()
            {
                EmpId = this.CurrentUser.Id,
                Date = DateTime.Now
            };
            MySpaceModel model = _employeeService.GetMySpace(getMySpaceCriteria);

            if (model != null)
            {
                model.DateOfEmployeeForgotAndLateEarlyIO = _employeeService.GetDateOfEmployeeForgotAndLateEarlyIO(getDatesMissingRequestCriteria);
            }

            var employee = this._employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.ResetPassword = employee.PasswordReset;

            return View(model);
        }

        /// <summary>
        /// Management Dashboard.
        /// </summary>
        /// <returns>Action Result.</returns>
        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (this.CurrentUser.IsInRoles(RoleSystem.Verifier) || this.CurrentUser.IsInRoles(RoleSystem.Approver))
            {
                var getHRLeaderDashboardCriteria = new GetHRLeaderDashboardCriteria
                {
                    Empid = this.CurrentUser.Id,
                    Date = DateTime.Now
                };
                IList<HRLeaderDashboardModel> model = _employeeService.GetHRLeaderDashboard(getHRLeaderDashboardCriteria);

                return PartialView("MultiRoleManagementDashboard", model);
            }

            return Content(string.Empty);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TeamManagement()
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = _teamService.GetAll(getAllGroupCriteria);

            return View(teams);
        }

        /// <summary>
        /// ActionResult AddTeam.
        /// </summary>
        /// <param name="teamId">Id of new team.</param>
        /// <returns>View AddTeam.</returns>
        [Authorize(Roles = "HR")]
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
                Team team = _teamService.GetById(criteria);

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

        /// <summary>
        /// ActionResult Change Password.
        /// </summary>
        /// <param name="model">ChangePassword ViewModel.</param>
        /// <returns>View Change Password.</returns>
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
                CommandResult result = _employeeService.ChangePassword(changePasswordCriteria);
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

        /// <summary>
        /// Action to update supervisor for employee.
        /// </summary>
        /// <param name="role">Role of supervisor.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <returns>Action Result.</returns>
        [HttpGet]
        public ActionResult Supervisor(short role, int employeeId)
        {
            var getEmployeeSupervisorCriteria = new GetEmployeeSupervisorCriteria()
            {
                EmpId = employeeId,
                Role = role
            };
            var supervisorModel = _employeeService.GetEmployeeSupervisor(getEmployeeSupervisorCriteria);
            var model = new SupervisorViewModel
            {
                SupervisorList = supervisorModel,
                EmployeeId = employeeId,
                SupervisorRole = role,
                ForView = "Employee"
            };

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);

            return View(model);
        }

        /// <summary>
        /// Action Result Supervisor.
        /// </summary>
        /// <param name="model">Supervisor View Model.</param>
        /// <returns>Action Result.</returns>
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
            CommandResult commandResult = _employeeService.UpdateSupervisor(updateSupervisorCriteria);
            bool updated = commandResult.Code == 0;

            return Json(new { result = updated }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action Update Shift Manger.
        /// </summary>
        /// <param name="role">Role of supervisor.</param>
        /// <param name="employeeId">Employee's Id.</param>
        /// <returns>ShiftManager ViewModel.</returns>
        [HttpGet]
        public ActionResult ShiftManager(short role, int employeeId)
        {
            var getEmployeeSupervisorCriteria = new GetEmployeeSupervisorCriteria()
            {
                EmpId = employeeId,
                Role = role
            };
            IList<SupervisorModel> shiftManagerModel = _employeeService.GetEmployeeSupervisor(getEmployeeSupervisorCriteria);

            var model = new ShiftManagerViewModel
            {
                ShiftManagerList = shiftManagerModel,
                EmployeeId = employeeId,
                SupervisorRole = role,
                ForView = "Employee"
            };

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);

            return View(model);
        }

        /// <summary>
        /// Action insert Shift Manager.
        /// </summary>
        /// <param name="model">ShiftManager View Model.</param>
        /// <returns>Boolean to check.</returns>
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
            CommandResult result = _employeeService.InsertEmployeeShiftManager(insertShiftMangerCriteria);

            if (result.Code != 0)
            {
                success = false;
            }

            return Json(new { result = success }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Store the show/hide setting of left navigation to session.
        /// </summary>
        /// <param name="isShow">Indicate the left navigation is showed or not.</param>
        [HttpPost]
        public void ToogleLeftNavigation(bool isShow)
        {
            var currentUser = (UserLoginModel)HttpContext.Session["CurrentUser"];
            currentUser.IsShowLeftNavigation = isShow;
            HttpContext.Session["CurrentUser"] = currentUser;
        }

        /// <summary>
        /// Method check Role User.
        /// </summary>
        /// <param name="model">Employee Model.</param>
        private void CheckRoleUser(Employee model)
        {
            if (model.RoleList != null && model.RoleList.Count == 1)
            {
                if (int.Parse(model.Roles) == (int)RoleSystem.SystemAdmin)
                {
                    var roleList = Enum.GetValues(typeof(RoleSystem)).Cast<RoleSystem>().Select(x => (int)x).ToList();
                    roleList.RemoveAt(0);
                    model.Roles = string.Join(",", roleList);
                }
            }

            if (model.RoleList == null)
            {
                var role = (int)RoleSystem.Member;
                model.Roles = role.ToString();
            }
        }

        /// <summary>
        /// Method get list supervisor name.
        /// </summary>
        /// <param name="id">Employee's id.</param>
        /// <param name="verifier">List nickname of verifier.</param>
        /// <param name="approver">List nickname of approver.</param>
        /// <param name="shiftManager">List nickname of shift Manager.</param>
        /// <param name="actingSupervisor">List nickname of Acting Supervisor.</param>
        private void GetListSupervisorName(int id, out string verifier, out string approver, out string shiftManager, out string actingSupervisor)
        {
            List<string> listVerifierName = null, listApproverName = null, listShiftName = null, listActingName = null;
            List<SupervisorNameModel> verifierList = null, approverList = null, shiftMonitorList = null, actingList = null;
            string displayMore = "...";
            var getListSupervisor = new GetListSupervisorNameCriteria()
            {
                EmpId = id,
                Role = null,
                RequestType = null,
            };
            IList<SupervisorNameModel> supervisors = _employeeService.GetListSupervisorName(getListSupervisor);
            if (supervisors != null)
            {
                verifierList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Verifier).ToList();
                approverList = supervisors.Where(x => x.ActionType == (int)RoleSystem.Approver).ToList();
                shiftMonitorList = supervisors.Where(x => x.ActionType == (int)RoleSystem.ShiftManager).ToList();
                actingList = supervisors.Where(x => x.ActionType == (int)RoleSystem.ActingSupervisor).ToList();
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
        }

        /// <summary>
        /// Function to convert User login to Employee.
        /// </summary>
        /// <param name="userLogin">UserLogin Model.</param>
        /// <returns>Employee Object.</returns>
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

        /// <summary>
        /// Check User have role admin or not.
        /// </summary>
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

        /// <summary>
        /// Composes the view bags for edit employee.
        /// </summary>
        private void ComposeViewBagsForEditEmployee()
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = LookupCodename.JobTitle
            };
            ViewBag.JobTitles = _lookupDataService.GetAll(getAllJobTitleCriteria);

            ViewBag.Marriages = Utilities.Marriages;
        }

        /// <summary>
        /// Set Data To UpdateEmployeeCriteria.
        /// </summary>
        /// <param name="model">Employee model.</param>
        /// <returns>UpdateEmployee Criteria.</returns>
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
