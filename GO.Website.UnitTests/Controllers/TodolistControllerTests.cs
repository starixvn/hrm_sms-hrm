namespace GO.Website.UnitTests.Controllers
{
    using AutoFixture;
    using Fanex.Data;
    using GO.Core.Models;
    using GO.Website.Controllers;
    using GO.Website.Models;
    using GO.Website.Models.Todolist;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using Xunit;

    public class TodolistControllerTests
    {
        private List<ApiSiteSetting> apiSiteSettings;
        private IUserControlSession userControlSession;
        private IAuthService authService;
        private ITodolistService todolistService;
        private IFeedbackService feedbackService;
        private readonly Fixture fixture = new();

        [Fact]
        public async Task GetTodolistBlock_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            usersites.First().SiteId = 1;
            apiSiteSettings.First().SiteId = 1;
            todolistService.GetUserSiteByUserIdAsync(Arg.Any<int>()).Returns(usersites);
            var expected = new
            {
                hrm = apiSiteSettings.Any(x => x.IsHrmSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                hrpp = apiSiteSettings.Any(x => x.IsHrppSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                epa = apiSiteSettings.Any(x => x.IsEpaSystem && usersites.Any(y => y.SiteId == x.SiteId)),
                epa360 = apiSiteSettings.Any(x => x.IsEpa360System && usersites.Any(y => y.SiteId == x.SiteId))
            };

            //Act
            var result = await controller.GetTodolistBlock();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var hrm = jsonResult.Value.GetType().GetProperty("hrm").GetValue(jsonResult.Value);
            var hrpp = jsonResult.Value.GetType().GetProperty("hrpp").GetValue(jsonResult.Value);
            var epa = jsonResult.Value.GetType().GetProperty("epa").GetValue(jsonResult.Value);
            var epa360 = jsonResult.Value.GetType().GetProperty("epa360").GetValue(jsonResult.Value);
            Assert.Equal(expected.hrm, hrm);
            Assert.Equal(expected.hrpp, hrpp);
            Assert.Equal(expected.epa, epa);
            Assert.Equal(expected.epa360, epa360);
        }

        [Fact]
        public async Task GetHrmTodolist_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var todoitems = fixture.Create<IList<TodolistItem>>();

            todolistService.GetUserSiteByUserIdAsync(Arg.Any<int>()).Returns(usersites);
            todolistService.GetTodolistAsync(Arg.Any<IList<UserSite>>(), Arg.Any<IList<ApiSiteSetting>>()).Returns(todoitems);

            //Act
            var result = await controller.GetHrmTodolist();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(todoitems, jsonResult.Value);
        }

        [Fact]
        public async Task GetEpaTodolist_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var todoitems = fixture.Create<IList<TodolistItem>>();

            todolistService.GetUserSiteByUserIdAsync(Arg.Any<int>()).Returns(usersites);
            todolistService.GetTodolistAsync(Arg.Any<IList<UserSite>>(), Arg.Any<IList<ApiSiteSetting>>()).Returns(todoitems);

            //Act
            var result = await controller.GetEpaTodolist();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(todoitems, jsonResult.Value);
        }

        [Fact]
        public async Task GetHrppTodolist_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var todoitems = fixture.Create<IList<TodolistItem>>();

            todolistService.GetUserSiteByUserIdAsync(Arg.Any<int>()).Returns(usersites);
            todolistService.GetTodolistAsync(Arg.Any<IList<UserSite>>(), Arg.Any<IList<ApiSiteSetting>>()).Returns(todoitems);

            //Act
            var result = await controller.GetHrppTodolist();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(todoitems, jsonResult.Value);
        }


        [Fact]
        public async Task GetEpa360Todolist_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var todoitems = fixture.Create<IList<TodolistItem>>();
            var site = apiSiteSettings.FirstOrDefault(x => x.IsEpa360System);
            feedbackService.GetTodolistByUserIdAsync(Arg.Any<int>()).Returns(todoitems);

            //Act
            var result = await controller.GetEpa360Todolist();
            if (site != null)
            {
                foreach (var item in todoitems)
                {
                    item.SiteId = site.SiteId;
                }
            }
            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(todoitems, jsonResult.Value);
        }

        [Fact]
        public async Task ProcessAction_WhenCalled_ReturnExpectedDataStatusCode200()
        {
            // Arrange
            var controller = GetController();
            var userId = fixture.Create<int>();
            var onUserId = fixture.Create<int>();
            var time = fixture.Create<string>();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var param = fixture.Create<ProcessActionParam>();
            param.Action.IsRedirect = true;
            userControlSession.UserId = userId;
            userControlSession.OnUserId = onUserId;

            todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId).Returns(usersites);
            authService.SSOGetTimeAsync().Returns(time);
            //Act
            var result = await controller.ProcessAction(param);
            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ProcessAction_WhenCalled_ReturnExpectedDataURL()
        {
            // Arrange
            var controller = GetController();
            var userId = fixture.Create<int>();
            var onUserId = fixture.Create<int>();
            var time = fixture.Create<string>();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var param = fixture.Create<ProcessActionParam>();
            param.Action.IsRedirect = true;
            param.SiteId = usersites.First().SiteId;
            userControlSession.UserId = userId;
            userControlSession.OnUserId = onUserId;

            todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId).Returns(usersites);
            authService.SSOGetTimeAsync().Returns(time);

            var expectedUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
            var expectedOnUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
            var expectedUsername = !string.IsNullOrWhiteSpace(usersites.First().Username) ? usersites.First().Username : usersites.First().Code;
            expectedUsername = !string.IsNullOrWhiteSpace(expectedUsername) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(expectedUsername)) : string.Empty;
            var expectedTime = HttpUtility.UrlEncode(Cryptography.EncryptPassword(await authService.SSOGetTimeAsync()));
            var expectedUrl = $"{usersites.First().SSOUrl}?userId={expectedUserId}&onUserId={expectedOnUserId}&username={expectedUsername}&time={expectedTime}&redirectUrl={param.Action.Url}";
            //Act
            var result = await controller.ProcessAction(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expectedUrl, jsonResult.Value);

        }

        [Fact]
        public async Task ProcessAction_WhenCalledUserNameAndCodeIsNull_ReturnExpectedDataURL()
        {
            // Arrange
            var controller = GetController();
            var userId = fixture.Create<int>();
            var onUserId = fixture.Create<int>();
            var time = fixture.Create<string>();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var param = fixture.Create<ProcessActionParam>();
            param.Action.IsRedirect = true;
            param.SiteId = usersites.First().SiteId;
            usersites.First().Username = null;
            usersites.First().Code = null;
            userControlSession.UserId = userId;
            userControlSession.OnUserId = onUserId;

            todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId).Returns(usersites);
            authService.SSOGetTimeAsync().Returns(time);

            var expectedUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
            var expectedOnUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
            var expectedUsername = !string.IsNullOrWhiteSpace(usersites.First().Username) ? usersites.First().Username : usersites.First().Code;
            expectedUsername = !string.IsNullOrWhiteSpace(expectedUsername) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(expectedUsername)) : string.Empty;
            var expectedTime = HttpUtility.UrlEncode(Cryptography.EncryptPassword(await authService.SSOGetTimeAsync()));
            var expectedUrl = $"{usersites.First().SSOUrl}?userId={expectedUserId}&onUserId={expectedOnUserId}&username={expectedUsername}&time={expectedTime}&redirectUrl={param.Action.Url}";
            //Act
            var result = await controller.ProcessAction(param);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(expectedUrl, jsonResult.Value);

        }

        [Fact]
        public async Task ProcessAction_WhenCalledIsRedirectFalse_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var userId = fixture.Create<int>();
            var onUserId = fixture.Create<int>();
            var token = fixture.Create<string>();
            var expectedObject = fixture.Create<object>();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var param = fixture.Create<ProcessActionParam>();
            param.Action.IsRedirect = false;
            param.SiteId = apiSiteSettings.First().SiteId;
            userControlSession.UserId = userId;
            userControlSession.OnUserId = onUserId;

            todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId).Returns(usersites);
            todolistService.GetTokenAsync(Arg.Any<ApiSiteSetting>()).Returns(token);
            todolistService.GetAsync<object>(param.Action.Url, token).Returns(expectedObject);
            //Act
            var result = await controller.ProcessAction(param);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ProcessAction_WhenCalledIsRedirectFalseAndMethodPost_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var userId = fixture.Create<int>();
            var onUserId = fixture.Create<int>();
            var token = fixture.Create<string>();
            var usersites = fixture.Create<IEnumerable<UserSite>>();
            var param = fixture.Create<ProcessActionParam>();
            var expected = fixture.Create<CommandResult>();
            param.Action.IsRedirect = false;
            param.SiteId = apiSiteSettings.First().SiteId;
            param.Action.Method = "post";
            userControlSession.UserId = userId;
            userControlSession.OnUserId = onUserId;

            todolistService.GetUserSiteByUserIdAsync(userControlSession.UserId).Returns(usersites);
            todolistService.GetTokenAsync(Arg.Any<ApiSiteSetting>()).Returns(token);
            todolistService.PostAsync<CommandResult>(Arg.Any<string>(), Arg.Any<HttpContent>(), Arg.Any<string>()).Returns(expected);
            //Act
            var result = await controller.ProcessAction(param);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, okObjectResult.Value);
        }



        #region helper

        private TodolistController GetController()
        {
            apiSiteSettings = fixture.Create<List<ApiSiteSetting>>();
            userControlSession = Substitute.For<IUserControlSession>();
            authService = Substitute.For<IAuthService>();
            todolistService = Substitute.For<ITodolistService>();
            feedbackService = Substitute.For<IFeedbackService>();

            return new TodolistController(
                apiSiteSettings,
                userControlSession,
                authService,
                todolistService,
                feedbackService
            );
        }

        #endregion helper
    }
}
