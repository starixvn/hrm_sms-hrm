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
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.EmployeeEmergency;
    using Fanex.NexHR.Criteria.EmployeeRelationship;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Information;

    public class InformationController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly ICommonDataService commonDataService;
        private readonly IEmployeeService employeeService;

        public InformationController(
            ISessionService sessionService,
            IBaseService baseService,
            ICommonDataService commonDataService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.baseService = baseService;
            this.commonDataService = commonDataService;
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
            model.Id = sessionService.Employee.Id;
            var criteria = SetUpdatePersonalInfoCriteria(model);

            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
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
                ModifiedBy = sessionService.Employee.Id,
                CountryCode = employee.CountryCode
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
                    EmployeeId = sessionService.Employee.Id,
                    FullName = viewModel.FullName,
                    Gender = viewModel.Gender,
                    DateOfBirth = viewModel.BirthDay,
                    Phone = viewModel.Phone,
                    Relationship = viewModel.Relationship,
                    IsEmergency = viewModel.IsEmergency,
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

        public ActionResult GetEmployeeEmergency()
        {
            var criteria = new GetEmergencyByEmployeeIdCriteria
            {
                EmployeeId = sessionService.Employee.Id
            };

            var emergencies = baseService.GetData<EmployeeEmergency>(criteria);

            ViewBag.IsAllowInsert = !emergencies.Any();
            return PartialView("_ListEmergency", emergencies);
        }

        public ActionResult InsertEmergency()
        {
            var viewModel = new EmployeeEmergencyViewModel
            {
                BirthDay = new DateTime(2000, 1, 1)
            };

            return PartialView("_AddEmergency", viewModel);
        }

        [HttpPost]
        public ActionResult InsertEmergency(EmployeeEmergencyViewModel viewModel)
        {
            var result = new CommandResult { Code = 1, Message = string.Empty };

            if (ModelState.IsValid)
            {
                var criteria = new InsertEmergencyCriteria
                {
                    EmployeeId = sessionService.Employee.Id,
                    FullName = viewModel.FullName,
                    Gender = viewModel.Gender,
                    DateOfBirth = viewModel.BirthDay,
                    Phone = viewModel.Phone,
                    Relationship = viewModel.Relationship,
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

        public ActionResult EditEmergency(int id)
        {
            var criteria = new GetEmergencyByIdCriteria
            {
                EmergencyId = id
            };
            var emergency = baseService.GetDataEntity<EmployeeEmergency>(criteria);
            var viewModel = new EmployeeEmergencyViewModel
            {
                FullName = emergency.FullName,
                Gender = emergency.Gender,
                Phone = emergency.Phone,
                BirthDay = emergency.DateOfBirth,
                Relationship = emergency.Relationship
            };
            return PartialView("_EditEmergency", viewModel);
        }

        [HttpPost]
        public ActionResult EditEmergency(int id, EmployeeEmergencyViewModel viewModel)
        {
            var result = new CommandResult { Code = 1, Message = string.Empty };
            if (ModelState.IsValid)
            {
                var criteria = new EditEmergencyCriteria
                {
                    EmergencyId = id,
                    FullName = viewModel.FullName,
                    Gender = viewModel.Gender,
                    DateOfBirth = viewModel.BirthDay,
                    Phone = viewModel.Phone,
                    Relationship = viewModel.Relationship,
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

        [HttpPost]
        public ActionResult DeleteEmergency(int id)
        {
            var criteria = new DeleteEmergencyCriteria
            {
                EmergencyId = id,
                ActorId = sessionService.Employee.Id
            };

            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}