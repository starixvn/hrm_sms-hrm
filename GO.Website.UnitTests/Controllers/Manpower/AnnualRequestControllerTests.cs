namespace GO.Website.UnitTests.Controllers.Manpower
{
    using AutoFixture;
    using GO.Core.Models;
    using GO.Website.Constants;
    using GO.Website.Controllers.Manpower;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class AnnualRequestControllerTests
    {
        private IUserControlSession userControlSession;
        private IManpowerForecastService manpowerForecastService;
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public async Task Create_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var units = fixture.Create<IEnumerable<string>>();
            userControlSession.UserId = fixture.Create<int>();

            manpowerForecastService.GetUnitByUserIdAsync(userControlSession.UserId).Returns(units);
            //Act
            var result = await controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/AnnualRequest/Create.cshtml", viewResult.ViewName);
            var dataResult = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.Model);
            Assert.Equal(units, dataResult);
        }

        [Fact]
        public async Task GetDataForCreateAnnualRequest_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var unitName = fixture.Create<string>();
            var periods = fixture.Create<ManpowerPeriod>();
            var details = fixture.Create<IEnumerable<AnnualManpowerForecastRequestDetail>>();
            userControlSession.UserId = fixture.Create<int>();

            manpowerForecastService.GetDataForCreateAnnualRequestAsync(userControlSession.UserId, 
                unitName).Returns(new ValueTuple<ManpowerPeriod, 
                IEnumerable<AnnualManpowerForecastRequestDetail>>(periods, details));

            //Act
            var result = await controller.GetDataForCreateAnnualRequest(unitName);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualPeriods = jsonResult.Value.GetType().GetProperty("period").GetValue(jsonResult.Value);
            var actualDetails = jsonResult.Value.GetType().GetProperty("details").GetValue(jsonResult.Value);
            Assert.Equal(periods, actualPeriods);
            Assert.Equal(actualDetails, details);
        }

        [Fact]
        public async Task Create_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var param = fixture.Create<CreateAnnualManpowerForecastRequestParam>();
            var command = fixture.Create<CommandResult>();
            userControlSession.UserId = fixture.Create<int>();

            manpowerForecastService.CreateAnnualRequestAsync(userControlSession.UserId, param).Returns(command);
            //Act
            var result = await controller.Create(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(command, jsonResult.Value);
        }

        [Fact]
        public void MyRequest_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            //Act
            var result = controller.MyRequest();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/AnnualRequest/MyRequest.cshtml", viewResult.ViewName);
        }

        [Fact]
        public void Detail_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var id = fixture.Create<int>();
            //Act
            var result = controller.Detail(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/AnnualRequest/Detail.cshtml", viewResult.ViewName);
            var dataResult = Assert.IsType<int>(viewResult.Model);
            Assert.Equal(id, dataResult);
        }

        [Fact]
        public async Task GetMyRequest_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            userControlSession.UserId = fixture.Create<int>();
            var request = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();

            manpowerForecastService.GetMyAnnualRequestAsync(userControlSession.UserId).Returns(request);
            //Act
            var result = await controller.GetMyRequest();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(request, jsonResult.Value);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnExpectedNotFound()
        {
            // Arrange
            var controller = GetController();
            var requestId = fixture.Create<int>();
            AnnualManpowerForecastRequest request = null;
            manpowerForecastService.GetAnnualRequestByIdAsync(requestId).Returns(request);

            //Act
            var result = await controller.GetById(requestId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var requestId = fixture.Create<int>();
            var request = fixture.Create<AnnualManpowerForecastRequest>();
            manpowerForecastService.GetAnnualRequestByIdAsync(requestId).Returns(request);

            //Act
            var result = await controller.GetById(requestId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualRequest = jsonResult.Value.GetType().GetProperty("request").GetValue(jsonResult.Value);
            Assert.Equal(request, actualRequest);
        }

        [Fact]
        public async Task GetById_WhenCalledUnitHead_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var requestId = fixture.Create<int>();
            var id = fixture.Create<int>();
            var request = fixture.Create<AnnualManpowerForecastRequest>();
            request.Creator.CreatorId = id;
            userControlSession.UserId = id;
            request.CurrentProcessId = PeriodProcess.UnitHead;

            manpowerForecastService.GetAnnualRequestByIdAsync(requestId).Returns(request);

            //Act
            var result = await controller.GetById(requestId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualRequest = jsonResult.Value.GetType().GetProperty("request").GetValue(jsonResult.Value);
            Assert.Equal(request, actualRequest);
        }

        [Fact]
        public async Task GetById_WhenCalledVerify_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var requestId = fixture.Create<int>();
            var id = fixture.Create<int>();
            var request = fixture.Create<AnnualManpowerForecastRequest>();
            request.Verifier.UserId = id;
            userControlSession.UserId = id;
            request.CurrentProcessId = PeriodProcess.Verify;

            manpowerForecastService.GetAnnualRequestByIdAsync(requestId).Returns(request);

            //Act
            var result = await controller.GetById(requestId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualRequest = jsonResult.Value.GetType().GetProperty("request").GetValue(jsonResult.Value);
            Assert.Equal(request, actualRequest);
        }

        [Fact]
        public async Task GetById_WhenCalledApprove_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var requestId = fixture.Create<int>();
            var id = fixture.Create<int>();
            var request = fixture.Create<AnnualManpowerForecastRequest>();
            request.Approver.UserId = id;
            userControlSession.UserId = id;
            request.CurrentProcessId = PeriodProcess.Approve;

            manpowerForecastService.GetAnnualRequestByIdAsync(requestId).Returns(request);

            //Act
            var result = await controller.GetById(requestId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualRequest = jsonResult.Value.GetType().GetProperty("request").GetValue(jsonResult.Value);
            Assert.Equal(request, actualRequest);
        }

        [Fact]
        public void MemberRequest_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            //Act
            var result = controller.MemberRequest();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Manpower/AnnualRequest/MemberRequest.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task GetMemberRequests_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var criteria = fixture.Create<GetMemberAnnualManpowerForecastRequestCriteria>();
            userControlSession.UserId = fixture.Create<int>();
            var request = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();

            manpowerForecastService.GetMemberAnnualRequestAsync(criteria).Returns(request);

            //Act
            var result = await controller.GetMemberRequests(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(request, jsonResult.Value);
        }

        [Fact]
        public async Task GetByPeriodId_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var criteria = fixture.Create<GetAnnualManpowerForecastRequestByPeriodIdCriteria>();
            var request = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();

            manpowerForecastService.GetAnnualRequestByPeriodIdAsync(criteria).Returns(request);

            //Act
            var result = await controller.GetByPeriodId(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(request, jsonResult.Value);
        }

        [Fact]
        public async Task UpdateStatus_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var command = fixture.Create<UpdateStatusAnnualManpowerForecastRequestCommand>();
            userControlSession.UserId = fixture.Create<int>();

            var expected = fixture.Create<CommandResult>();

            manpowerForecastService.UpdateStatusAnnualRequestAsync(command).Returns(expected);

            //Act
            var result = await controller.UpdateStatus(command);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region helper

        private AnnualRequestController GetController()
        {
            userControlSession = Substitute.For<IUserControlSession>();
            manpowerForecastService = Substitute.For<IManpowerForecastService>();

            return new AnnualRequestController(
                userControlSession,
                manpowerForecastService
            );
        }

        #endregion helper
    }
}
