using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO.Website.UnitTests.Criteria.RoleAssignment
{
    using Website.Criteria.RoleAssignment;
    using Xunit;

    public class GetUserRoleCriteriaTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var criteria = new GetUserRoleCriteria();
            var expected = "GO_UserRole_Get";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", 1)]
        public void IsValid_WhenCalledWithParamValid_ReturnTrue(string keyword, short siteId)
        {
            // Arrange
            var criteria = new GetUserRoleCriteria()
            {
                Keyword = keyword,
                SiteId = siteId,
                StatusId = 1,
                PageIndex = 1,
                PageSize = 1,
                TotalRows = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("", 0)]
        public void IsValid_WhenCalledWithParamInValid_ReturnFalse(string keyword, short siteId)
        {
            // Arrange
            var criteria = new GetUserRoleCriteria()
            {
                Keyword = keyword,
                SiteId = siteId
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }
    }
}
