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
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeSupervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using Simplicode.Imaging;
    using Simplicode.Imaging.Filters;

    /// <summary>
    /// Employee Controller.
    /// </summary>
    [Authorize]
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
            ViewBag.Groups = _teamService.GetAll();
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
            ViewBag.Groups = _teamService.GetAll();

            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
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
            : _lookupDataService.Get(LookupCodeName.JobTitle, model.JobTitleId.Value).Name;

            ViewBag.DefaultCalendar = model.IsDefaultSchedule;
            ViewBag.Working = _employeeService.GetMySpace(id);
            var workingCalendar = _employeeService.GetEmployeeShift(id);
            var shifts = _shiftService.GetAll();
            var getCountryByCodeCriteria = new GetCountryByCodeCriteria() { CountryCode = model.CountryCode };
            var countryName = _employeeService.GetCountryByCode(getCountryByCodeCriteria);
            if (countryName != null)
            {
                ViewBag.CountryName = countryName.CountryName;
            }

            ViewBag.Sun = workingCalendar.SunShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SunShift).Name;

            ViewBag.Mon = workingCalendar.MonShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.MonShift).Name;

            ViewBag.Tue = workingCalendar.TueShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.TueShift).Name;

            ViewBag.Wed = workingCalendar.WedShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.WedShift).Name;

            ViewBag.Thu = workingCalendar.ThuShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.ThuShift).Name;

            ViewBag.Fri = workingCalendar.FriShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.FriShift).Name;

            ViewBag.Sat = workingCalendar.SatShift == null
                ? "OFF"
                : shifts.FirstOrDefault(i => i.Id == workingCalendar.SatShift).Name;

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
            var teamList = _teamService.GetAll();
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
            var teamList = _teamService.GetAll();
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
            var create = _employeeService.Insert(ModelState, insertEmployeeCriteria);
            if (create)
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

            model.ListCountryCode = new List<SelectListItem>();
            foreach (var item in countryList)
            {
                model.ListCountryCode.Add(new SelectListItem { Text = item.CountryName, Value = item.CountryCode });
            }

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
                var result = _employeeService.Update(model, ModelState);
                if (result != 0)
                {
                    ViewBag.Updated = false;
                }
                else
                {
                    ViewBag.Updated = true;
                }

                if (result == 0 && this.CurrentUser.Id == model.Id)
                {
                    var criteria = new GetEmployeeByIdCriteria() { Empid = model.Id };
                    var currentUser = _employeeService.GetUserLoginByEmployeeId(criteria);
                    HttpContext.Session["CurrentUser"] = currentUser;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            foreach (var item in countryList)
            {
                model.ListCountryCode.Add(new SelectListItem { Text = item.CountryName, Value = item.CountryCode });
            }

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
                var model = _employeeService.GetEmployeeShift(id);
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
                ViewBag.Updated = _employeeService.UpdateEmployeeShift(model);
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
            var model = _employeeService.GetMySpace(employeeId);
            var dateOfEmployeeForgotAndLateEarlyIO = _employeeService.GetDateOfEmployeeForgotAndLateEarlyIO(this.CurrentUser.Id);
            model.DateOfEmployeeForgotAndLateEarlyIO = dateOfEmployeeForgotAndLateEarlyIO;
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria() { Empid = employeeId };
            var employee = this._employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            ViewBag.ResetPassword = employee.PasswordReset;

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ManagementDashboard()
        {
            if (this.CurrentUser.IsInRoles(RolesEnum.Verifier) || this.CurrentUser.IsInRoles(RolesEnum.Approver))
            {
                var model = _employeeService.GetHrLeaderDashboard(DateTime.Today, this.CurrentUser.Id);
                return PartialView("MultiRoleManagementDashboard", model);
            }

            return Content(string.Empty);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TeamManagement()
        {
            IList<Team> teams = _teamService.GetAll();

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
                var team = _teamService.Get(teamId);
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
                int errorCode = _employeeService.ChangePassword(this.CurrentUser.Id, model.Password, model.NewPassword);
                switch (errorCode)
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
            ViewBag.Groups = _teamService.GetAll();

            return View(model);
        }

        [HttpPost]
        public ActionResult Supervisor(SupervisorViewModel model)
        {
            var result = _employeeService.UpdateSupervisor(model, this.CurrentUser.Id);

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
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

            ViewBag.Groups = _teamService.GetAll();

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
            var result = _employeeService.InsertEmployeeShiftManager(model, this.CurrentUser.Id);

            if (result != 0)
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
                if (int.Parse(model.Roles) == (int)RolesEnum.SystemAdmin)
                {
                    var roleList = Enum.GetValues(typeof(RolesEnum)).Cast<RolesEnum>().Select(x => (int)x).ToList();
                    roleList.RemoveAt(0);
                    model.Roles = string.Join(",", roleList);
                }
            }

            if (model.RoleList == null)
            {
                var role = (int)RolesEnum.Member;
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
            var getListSupervisor = new GetListSupervisorNameCriteria()
            {
                EmpId = id,
                Role = null,
                RequestType = null,
            };
            IList<SupervisorNameModel> supervisors = _employeeService.GetListSupervisorName(getListSupervisor);
            if (supervisors != null)
            {
                verifierList = supervisors.Where(x => x.ActionType == (int)RolesEnum.Verifier).ToList();
                approverList = supervisors.Where(x => x.ActionType == (int)RolesEnum.Approver).ToList();
                shiftMonitorList = supervisors.Where(x => x.ActionType == (int)RolesEnum.ShiftManager).ToList();
                actingList = supervisors.Where(x => x.ActionType == (int)RolesEnum.ActingSupervisor).ToList();
            }

            listVerifierName = verifierList.Count > 3 ? verifierList.Select(x => x.NickName).Take(3).ToList() : verifierList.Select(x => x.NickName).ToList();
            if (verifierList.Count > 3)
            {
                listVerifierName.Add("...");
            }

            verifier = string.Join(", ", listVerifierName);

            listApproverName = approverList.Count > 3 ? approverList.Select(x => x.NickName).Take(3).ToList() : approverList.Select(x => x.NickName).ToList();
            if (approverList.Count > 3)
            {
                listApproverName.Add("...");
            }

            approver = string.Join(", ", listApproverName);

            listShiftName = shiftMonitorList.Count > 3 ? shiftMonitorList.Select(x => x.NickName).Take(3).ToList() : shiftMonitorList.Select(x => x.NickName).ToList();
            if (shiftMonitorList.Count > 3)
            {
                listShiftName.Add("...");
            }

            shiftManager = string.Join(", ", listShiftName);

            listActingName = actingList.Count > 3 ? actingList.Select(x => x.NickName).Take(3).ToList() : actingList.Select(x => x.NickName).ToList();
            if (actingList.Count > 3)
            {
                listActingName.Add("...");
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
                authorize = currentUserRole.Any(x => x == (int)RolesEnum.SystemAdmin);
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
            ViewBag.Groups = _teamService.GetAll();
            ViewBag.JobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);

            ViewBag.Marriages = Utilities.Marriages;
        }
    }
}
