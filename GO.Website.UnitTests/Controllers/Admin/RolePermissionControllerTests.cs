namespace GO.Website.UnitTests.Controllers.Admin
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using Models.Admin.RolePermission;
    using NSubstitute;
    using Website.Controllers.Admin;
    using Xunit;

    public class RolePermissionControllerTests
    {
        private IUserControlSession userControlSession;
        private IRoleService roleService;
        private IFunctionService functionService;
        private readonly Fixture fixture = new();

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewData()
        {
            // Arrange
            var controller = GetController();
            //Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Admin/RolePermission/Index.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task GetRolePermission_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var roleId = fixture.Create<short>();
            userControlSession.SiteId = fixture.Create<short>();

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var functions = fixture.Create<IList<Function>>();
            var hierarchyFunctions = fixture.Create<IList<Function>>();
            var permissions = fixture.Create<IList<FunctionPermission>>();

            functionService.GetBySiteIdAsync(userControlSession.SiteId).Returns(functions);
            functionService.GetHierarchyBySiteIdAsync(userControlSession.SiteId).Returns(hierarchyFunctions);
            functionService.GetPermissionBySiteAndRoleIdAsync(userControlSession.SiteId, roleId).Returns(permissions);
            //Act
            var result = await controller.GetRolePermission(roleId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actualFunctions = jsonResult.Value.GetType().GetProperty("functions")?.GetValue(jsonResult.Value);
            var actualPermissions = jsonResult.Value.GetType().GetProperty("permissions")?.GetValue(jsonResult.Value);
            var actualHierarchyFunctions = jsonResult.Value.GetType().GetProperty("hierarchyFunctions")?.GetValue(jsonResult.Value);
            Assert.Equal(functions, actualFunctions);
            Assert.Equal(hierarchyFunctions, actualHierarchyFunctions);
            Assert.Equal(permissions, actualPermissions);
        }

        [Fact]
        public async Task AssignPermission_WhenCalled_ReturnExpectedData()
        {
            // Arrange
            var controller = GetController();
            var model = fixture.Create<AssignPermissionToRole>();
            var command = fixture.Create<CommandResult>();
            userControlSession.SiteId = fixture.Create<short>();


            roleService.AssignPermissionAsync(userControlSession.SiteId, model.RoleId, model.Permissions).Returns(command);
            //Act
            var result = await controller.AssignPermission(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(command, jsonResult.Value);
        }

        #region helper

        private RolePermissionController GetController()
        {
            userControlSession = Substitute.For<IUserControlSession>();
            roleService = Substitute.For<IRoleService>();
            functionService = Substitute.For<IFunctionService>();

            return new RolePermissionController(
                userControlSession,
                roleService,
                functionService
            );
        }

        #endregion helper
    }
}
