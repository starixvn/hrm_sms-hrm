namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Criteria.Country;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Employee;
    using Fanex.NexHR.Web.Providers;

    public class AccountController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;

        public AccountController(
            IEmployeeService employeeService,
            IBaseService baseService)
        {
            this.employeeService = employeeService;
            this.baseService = baseService;
        }

        public ActionResult Index()
        {
            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria { EmpId = CurrentUser.Id };
            Employee model = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            model.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;

            var employeeShiftCriteria = new GetEmployeeShiftCriteria { EmpId = CurrentUser.Id };
            EmployeeShift employeeShift = employeeService.GetEmployeeShift(employeeShiftCriteria);

            ViewBag.IsUpdated = false;
            DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            DisplayShiftForPersonalInformation(employeeShift);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Employee employeeModel)
        {
            employeeModel.Id = CurrentUser.Id;
            employeeModel.Modifier = new Employee { Id = CurrentUser.Id };
            ModelState.Clear();
            UpdateEmployeePersonalInformationCriteria criteria = SetUpdatePersonalInfoCriteria(employeeModel);
            CommandResult result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                ViewBag.IsUpdated = true;
            }
            else
            {
                ModelState.AddModelError(result.Code.ToString(), result.Message);
                ViewBag.IsUpdated = false;
            }

            var getEmployeeByIdCriteria = new GetEmployeeByIdCriteria { EmpId = CurrentUser.Id };
            Employee displayModel = employeeService.GetEmployeeById(getEmployeeByIdCriteria);
            displayModel.AllowEditPersonalInformation = ConfigSettings.AllowEditPersonalInformation;

            var employeeShiftCriteria = new GetEmployeeShiftCriteria { EmpId = CurrentUser.Id };
            var workingCalendar = employeeService.GetEmployeeShift(employeeShiftCriteria);

            DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            DisplayShiftForPersonalInformation(workingCalendar);

            return View(displayModel);
        }

        public ActionResult UpdatePasswordResetStatus(bool status)
        {
            var criteria = new UpdatePasswordResetStatusCriteria
            {
                EmpId = CurrentUser.Id,
                PasswordReset = status
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        public ActionResult EditContact()
        {
            var criteria = new GetEmployeeByIdCriteria { EmpId = CurrentUser.Id };
            Employee employee = employeeService.GetEmployeeById(criteria);
            var model = new EmployeeContactViewModel
            {
                ChineseName = employee.ChineseName,
                Email = employee.Email,
                Phone = employee.Phone,
                Skype = employee.Skype,
                LetsTalk = employee.LetsTalk,
                Address = employee.Address,
                CurrentAddress = employee.CurrentAddress,
                FileUploaded = GetAllFileUploaded(),
                BaseFileLocation = Constants.ProfileUploadPath + "/" + CurrentUser.Code + "/"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContact(EmployeeContactViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new SelfUpdateContactCriteria
                {
                    EmpId = CurrentUser.Id,
                    ChineseName = model.ChineseName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Skype = model.Skype,
                    Yahoo = model.LetsTalk,
                    Address = model.Address,
                    CurrentAddress = model.CurrentAddress
                };

                var result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    ViewBag.Message = result.Message;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
                model.FileUploaded = GetAllFileUploaded();
                model.BaseFileLocation = Constants.ProfileUploadPath + "/" + CurrentUser.Code + "/";
            }
            return View(model);
        }

        public ActionResult GetFileUploaded()
        {
            var fileNames = GetAllFileUploaded();
            ViewBag.BaseUrl = Constants.ProfileUploadPath + "/" + CurrentUser.Code + "/";
            return PartialView("_ListFileUploaded", fileNames);
        }

        private IList<string> GetAllFileUploaded()
        {
            var filePath = new List<string>();
            string path = HostingEnvironment.ApplicationPhysicalPath + "/file/uploads/profile/" + CurrentUser.Code;
            if (Directory.Exists(path))
            {
                var paths = Directory.GetFiles(path);
                foreach (var p in paths)
                {
                    filePath.Add(Path.GetFileName(p));
                }
            }

            return filePath;
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult ResetPassword(int id)
        {
            string hashPassword = Encryptor.ComputeHash(employeeService.HashProvider, "123456");
            var criteria = new ChangePasswordCriteria
            {
                EmpId = id,
                Password = hashPassword,
                PasswordReset = true,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = baseService.ExecuteNonQuery(criteria);
            return Json(result);
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
                int retCode = employeeService.ValidateUser(CurrentUser.Code, model.Password);
                if (retCode == 0)
                {
                    var criteria = new ChangePasswordCriteria
                    {
                        EmpId = CurrentUser.Id,
                        Password = Encryptor.ComputeHash(employeeService.HashProvider, model.NewPassword)
                    };

                    CommandResult result = baseService.ExecuteNonQuery(criteria);
                    switch (result.Code)
                    {
                        case 0: // OK
                            ViewBag.ChangePwdOK = Messages.ChangePwdOK;
                            break;

                        case 1: // deleted
                            ModelState.AddModelError(string.Empty, Messages.DeletedUser);
                            break;

                        case 2: // disabled
                            ModelState.AddModelError(string.Empty, Messages.DisabledUser);
                            break;

                        default: // wrong password
                            ModelState.AddModelError(string.Empty, Messages.WrongPassword);
                            break;
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Messages.WrongPassword);
                }
            }

            return View(model);
        }

        private void DisplayShiftForPersonalInformation(EmployeeShift workingCalendar)
        {
            ViewBag.Sun = workingCalendar.SunShiftName;
            ViewBag.Mon = workingCalendar.MonShiftName;
            ViewBag.Tue = workingCalendar.TueShiftName;
            ViewBag.Wed = workingCalendar.WedShiftName;
            ViewBag.Thu = workingCalendar.ThuShiftName;
            ViewBag.Fri = workingCalendar.FriShiftName;
            ViewBag.Sat = workingCalendar.SatShiftName;
        }

        private void DisplayDefaultValuesOfDropDownListsForPersonalInformation()
        {
            var getAllCountryCriteria = new GetAllCountryCriteria();
            var countries = employeeService.GetAllCountry(getAllCountryCriteria);
            ViewBag.Countries = countries.Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryCode }).ToList();

            ViewBag.Genders = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = Labels.Female,
                    Value = "0"
                },
                new SelectListItem
                {
                    Text = Labels.Male,
                    Value = "1"
                }
            };

            ViewBag.Marriages = Utilities.Marriages;
        }

        private UpdateEmployeePersonalInformationCriteria SetUpdatePersonalInfoCriteria(Employee employeeModel)
        {
            var criteria = new UpdateEmployeePersonalInformationCriteria
            {
                EmpId = employeeModel.Id,
                DateOfBirth = employeeModel.DateOfBirth,
                PlaceOfbirth = employeeModel.PlaceOfBirth,
                Ethnic = employeeModel.Ethnic,
                Native = employeeModel.Native,
                Religion = employeeModel.Religion,
                DateOfIdCard = employeeModel.DateOfIdCard,
                PlaceOfIdCard = employeeModel.PlaceOfIdCard,
                HomePhone = employeeModel.HomePhone,
                Qualification = employeeModel.Qualification,
                Major = employeeModel.Major,
                Phone = employeeModel.Phone,
                Gender = employeeModel.Gender,
                Skype = employeeModel.Skype,
                Yahoo = employeeModel.LetsTalk,
                ChineseName = employeeModel.ChineseName,
                Marriage = employeeModel.Marriage,
                PermanentAddress = employeeModel.Address,
                CurrentAddress = employeeModel.CurrentAddress,
                IdCardNumber = employeeModel.CardNo,
                PassportNo = employeeModel.PassportNo,
                PassportExp = employeeModel.PassportExp,
                ModifiedBy = employeeModel.Modifier.Id,
                CountryCode = employeeModel.CountryCode
            };

            return criteria;
        }
    }
}