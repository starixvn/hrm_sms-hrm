namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeRelationship;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Information;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Session;

    public class InformationController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly ICommonDataService commonDataService;
        private readonly IEmployeeService employeeService;
        private readonly IEmailService emailService;

        public InformationController(
            IUserControlSession userControlSession,
            IBaseService baseService,
            ICommonDataService commonDataService,
            IEmployeeService employeeService,
            IEmailService emailService) : base(userControlSession)
        {
            this.baseService = baseService;
            this.commonDataService = commonDataService;
            this.employeeService = employeeService;
            this.emailService = emailService;
        }

        public ActionResult UpdateInformation()
        {
            Employee employee = employeeService.GetEmployeeById(EmployeeSession.EmpId);
            DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            ViewBag.BaseFileLocation = Constants.ProfileUploadPath + "/" + EmployeeSession.Code + "/";
            return View(employee);
        }

        private void DisplayDefaultValuesOfDropDownListsForPersonalInformation()
        {
            IList<Country> countries = commonDataService.GetAllCountry();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInformation(Employee model)
        {
            ModelState.Clear();
            model.Id = EmployeeSession.EmpId;
            var criteria = SetUpdatePersonalInfoCriteria(model);
            Employee employee = employeeService.GetEmployeeById(EmployeeSession.EmpId);

            var result = baseService.ExecuteNonQuery(criteria);

            if (result.IsSuccessful)
            {
                SendMailUpdateInfo(model, employee);
            }

            return Json(result);
        }

        public bool IsValidRoleToSendMail(IList<Role> roles)
        {
            return roles.Any(role => role.RoleId == (short)RoleSystem.HR
                || role.RoleId == (short)RoleSystem.Verifier
                || role.RoleId == (short)RoleSystem.Approver);
        }

        public bool IsValidUpdateFieldSendMail(Employee model, Employee employee)
        {
            return employee.CardNo != model.CardNo ||
                employee.Phone != model.Phone ||
                employee.Address != model.Address ||
                employee.CurrentAddress != model.CurrentAddress;
        }

        public void SendMailUpdateInfo(Employee model, Employee employee)
        {
            if (IsValidUpdateFieldSendMail(model, employee) && IsValidRoleToSendMail(employee.Roles))
            {
                emailService.SendEmail(
                    ConfigSettings.EmailSetting,
                    receiverEmail: employee.Email,
                    mailCC: string.Empty,
                    subject: Messages.UpdateInformation,
                    body: Messages.UpdatedSuccessfully
                );
            }
        }

        private UpdateEmployeePersonalInformationCriteria SetUpdatePersonalInfoCriteria(Employee employee)
        {
            var criteria = new UpdateEmployeePersonalInformationCriteria
            {
                EmpId = employee.Id,
                Email = employee.Email,
                DateOfBirth = employee.DateOfBirth,
                PlaceOfbirth = employee.PlaceOfBirth,
                Ethnic = employee.Ethnic,
                Native = employee.Native,
                Religion = employee.Religion,
                DateOfIdCard = employee.DateOfIdCard,
                PlaceOfIdCard = employee.PlaceOfIdCard,
                HomePhone = employee.HomePhone,
                Qualification = employee.Qualification,
                Major = employee.Major,
                Phone = employee.Phone,
                Gender = employee.Gender,
                Skype = employee.Skype,
                LetsTalk = employee.LetsTalk,
                ChineseName = employee.ChineseName,
                Marriage = employee.Marriage,
                PermanentAddress = employee.Address,
                CurrentAddress = employee.CurrentAddress,
                IdCardNumber = employee.CardNo,
                PassportNo = employee.PassportNo,
                PassportExp = employee.PassportExp,
                ModifiedBy = EmployeeSession.EmpId,
                CountryCode = employee.CountryCode
            };

            return criteria;
        }

        public ActionResult GetFileUploaded()
        {
            var fileNames = GetAllFileUploaded();
            ViewBag.BaseUrl = Constants.ProfileUploadPath + "/" + EmployeeSession.Code + "/";
            return PartialView("_ListFileUploaded", fileNames);
        }

        private IList<string> GetAllFileUploaded()
        {
            var filePath = new List<string>();
            string path = HostingEnvironment.ApplicationPhysicalPath + "/file/uploads/profile/" + EmployeeSession.Code;
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

        public ActionResult GetEmployeeRelationship()
        {
            var criteria = new GetRelationshipByEmployeeIdCriteria
            {
                EmployeeId = EmployeeSession.EmpId
            };

            var relationships = baseService.GetData<EmployeeRelationship>(criteria);

            return PartialView("_ListRelationship", relationships);
        }

        public ActionResult InsertRelationship()
        {
            var viewModel = new EmployeeRelationshipViewModel
            {
                BirthDay = new DateTime(2000, 1, 1)
            };

            return PartialView("_AddRelationship", viewModel);
        }

        [HttpPost]
        public ActionResult InsertRelationship(EmployeeRelationshipViewModel viewModel)
        {
            var result = new CommandResult { Code = 1, Message = string.Empty };

            if (ModelState.IsValid)
            {
                var criteria = new InsertRelationshipCriteria
                {
                    EmployeeId = EmployeeSession.EmpId,
                    FullName = viewModel.FullName,
                    Gender = viewModel.Gender,
                    DateOfBirth = viewModel.BirthDay,
                    Phone = viewModel.Phone,
                    Relationship = viewModel.Relationship,
                    IsEmergency = viewModel.IsEmergency,
                    Creator = EmployeeSession.EmpId
                };

                result = baseService.ExecuteNonQuery(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        public ActionResult EditRelationship(int id)
        {
            var criteria = new GetRelationshipByIdCriteria
            {
                RelationshipId = id
            };
            var relationship = baseService.GetDataEntity<EmployeeRelationship>(criteria);
            var viewModel = new EmployeeRelationshipViewModel
            {
                FullName = relationship.FullName,
                Gender = relationship.Gender,
                Phone = relationship.Phone,
                Relationship = relationship.Relationship,
                BirthDay = relationship.DateOfBirth,
                IsEmergency = relationship.IsEmergency
            };
            return PartialView("_EditRelationship", viewModel);
        }

        [HttpPost]
        public ActionResult EditRelationship(int id, EmployeeRelationshipViewModel viewModel)
        {
            var result = new CommandResult { Code = 1, Message = string.Empty };
            if (ModelState.IsValid)
            {
                var criteria = new EditRelationshipCriteria
                {
                    RelationshipId = id,
                    FullName = viewModel.FullName,
                    Gender = viewModel.Gender,
                    DateOfBirth = viewModel.BirthDay,
                    Phone = string.IsNullOrEmpty(viewModel.Phone) ? string.Empty : viewModel.Phone,
                    Relationship = viewModel.Relationship,
                    IsEmergency = viewModel.IsEmergency,
                    Modifier = EmployeeSession.EmpId
                };

                result = baseService.ExecuteNonQuery(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteRelationship(int id)
        {
            var criteria = new DeleteRelationshipCriteria
            {
                RelationshipId = id,
                ActorId = EmployeeSession.EmpId
            };

            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}