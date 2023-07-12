using AutoFixture;
using GO.Website.Controllers;
using GO.Website.Models;
using GO.Website.Models.Todolist;
using GO.Website.Services;
using HRM.UserControl.Client.Core.Services;
using HRM.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GO.Website.UnitTests.Controllers
{
    public class TodolistControllerTests
    {
        private List<ApiSiteSetting> apiSiteSettings;
        private IUserControlSession userControlSession;
        private IAuthService authService;
        private ITodolistService todolistService;
        private IFeedbackService feedbackService;
        private readonly Fixture fixture = new();

        [Fact]
        public async Task GetTodolistBlock_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            var usersites = fixture.Create<IEnumerable<UserSite>>();

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
        public async Task GetHrmTodolist_WhenCalled_ReturnExpectedViewData()
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
        public async Task GetEpaTodolist_WhenCalled_ReturnExpectedViewData()
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
        public async Task GetHrppTodolist_WhenCalled_ReturnExpectedViewData()
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
        public async Task GetEpa360Todolist_WhenCalled_ReturnExpectedViewData()
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
