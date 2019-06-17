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
    using Fanex.NexHR.Models.EmployeeRelationship;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Information;

    public class InformationController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public InformationController(
            ISessionService sessionService,
            IBaseService baseService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        public ActionResult UpdateInformation()
        {
            Employee employee = employeeService.GetEmployeeById(sessionService.Employee.Id);
            DisplayDefaultValuesOfDropDownListsForPersonalInformation();
            ViewBag.BaseFileLocation = Constants.ProfileUploadPath + "/" + sessionService.Employee.Code + "/";
            return View(employee);
        }

        private void DisplayDefaultValuesOfDropDownListsForPersonalInformation()
        {
            var countries = employeeService.GetAllCountry();
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
            model.Id = sessionService.Employee.Id;
            model.Modifier = new Employee { Id = sessionService.Employee.Id };
            var criteria = SetUpdatePersonalInfoCriteria(model);

            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
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

        public ActionResult GetFileUploaded()
        {
            var fileNames = GetAllFileUploaded();
            ViewBag.BaseUrl = Constants.ProfileUploadPath + "/" + sessionService.Employee.Code + "/";
            return PartialView("_ListFileUploaded", fileNames);
        }

        private IList<string> GetAllFileUploaded()
        {
            var filePath = new List<string>();
            string path = HostingEnvironment.ApplicationPhysicalPath + "/file/uploads/profile/" + sessionService.Employee.Code;
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
                EmployeeId = sessionService.Employee.Id
            };

            var relationships = baseService.GetData<EmployeeRelationship>(criteria);

            return PartialView("_ListRelationship", relationships);
        }

        public ActionResult InsertRelationship()
        {
            var isExistsEmergency = IsExistsEmergency();
            var viewModel = new EmployeeRelationshipViewModel
            {
                DateOfBirth = DateTime.Now,
                Relationship = isExistsEmergency ? Relationship.Children : Relationship.Emergency,
                IsExistEmergency = isExistsEmergency
            };

            return PartialView("_AddRelationship", viewModel);
        }

        [HttpPost]
        public ActionResult InsertRelationship(EmployeeRelationshipViewModel viewModel)
        {
            var result = new CommandResult { Code = 1, Message = string.Empty };
            if (viewModel.Relationship == Relationship.Emergency && string.IsNullOrEmpty(viewModel.Phone))
            {
                ModelState.AddModelError("Phone", "The phone is required for Emergency");
            }

            if (ModelState.IsValid)
            {
                var criteria = new InsertRelationshipCriteria
                {
                    EmployeeId = sessionService.Employee.Id,
                    FullName = viewModel.FullName,
                    Gender = viewModel.Gender,
                    DateOfBirth = viewModel.DateOfBirth,
                    Relationship = (int)viewModel.Relationship,
                    Phone = viewModel.Phone,
                    Creator = sessionService.Employee.Id
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
            var isExistsEmergency = IsExistsEmergency();
            var viewModel = new EmployeeRelationshipViewModel
            {
                FullName = relationship.FullName,
                Gender = relationship.Gender,
                Phone = relationship.Phone,
                DateOfBirth = relationship.DateOfBirth,
                Relationship = (Relationship)relationship.Relationship,
                IsExistEmergency = (Relationship)relationship.Relationship != Relationship.Emergency && isExistsEmergency
            };
            return PartialView("_EditRelationship", viewModel);
        }

        [HttpPost]
        public ActionResult EditRelationship(int id, EmployeeRelationshipViewModel viewModel)
        {
            var result = new CommandResult { Code = 1, Message = string.Empty };
            if (viewModel.Relationship == Relationship.Emergency && string.IsNullOrEmpty(viewModel.Phone))
            {
                ModelState.AddModelError("Phone", "The phone is required for Emergency");
            }

            if (ModelState.IsValid)
            {
                var criteria = new EditRelationshipCriteria
                {
                    RelationshipId = id,
                    FullName = viewModel.FullName,
                    Gender = viewModel.Gender,
                    DateOfBirth = viewModel.DateOfBirth,
                    Relationship = (int)viewModel.Relationship,
                    Phone = viewModel.Phone,
                    Modifier = sessionService.Employee.Id
                };

                result = baseService.ExecuteNonQuery(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        private bool IsExistsEmergency()
        {
            var criteria = new CheckEmergencyExistsCriteria
            {
                EmployeeId = sessionService.Employee.Id
            };

            return baseService.GetDataEntity<bool>(criteria);
        }

        [HttpPost]
        public ActionResult DeleteRelationship(int id)
        {
            var criteria = new DeleteRelationshipCriteria
            {
                RelationshipId = id,
                ActorId = sessionService.Employee.Id
            };

            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}