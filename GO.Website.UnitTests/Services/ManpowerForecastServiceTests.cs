namespace GO.Website.UnitTests.Services
{
    using AutoFixture;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models.Manpower;
    using GO.Website.Models.Manpower.Period;
    using GO.Website.Repositories;
    using GO.Website.Services;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;


    public class ManpowerForecastServiceTests
    {
        private IManpowerForecastRepository manpowerForecastRepository;
        private IBaseService baseService;
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public async Task GetUnitByUserIdAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();

            var userId = fixture.Create<int>();
            var expected = fixture.Create<IEnumerable<string>>();
            var criteria = new GetUnitByUserIdCriteria();
            baseService.GetDataAsync<string>(Arg.Do<GetUnitByUserIdCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetUnitByUserIdAsync(userId);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetDataForCreateAnnualRequestAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();

            var userId = fixture.Create<int>();
            var unitName = fixture.Create<string>();
            var expectedPeriod = fixture.Create<IEnumerable<ManpowerPeriod>>();
            var expectedDetails = fixture.Create<IEnumerable<AnnualManpowerForecastRequestDetail>>();
            var criteria = new GetDataForCreateAnnualManpowerForecastRequestCriteria();
            baseService.FetchMultipleAsync<ManpowerPeriod, AnnualManpowerForecastRequestDetail>
                (Arg.Do<GetDataForCreateAnnualManpowerForecastRequestCriteria>(x => criteria = x)).Returns(
                new ValueTuple<IEnumerable<ManpowerPeriod>, IEnumerable<AnnualManpowerForecastRequestDetail>>(expectedPeriod, expectedDetails)); ;
            //Act
            var actual = await service.GetDataForCreateAnnualRequestAsync(userId, unitName);

            //Assert
            Assert.Equal(expectedPeriod.First(), actual.Item1);
            Assert.Equal(expectedDetails, actual.Item2);
        }

        [Fact]
        public async Task GetDataForCreateAnnualRequestAsyncForecasts_WhenCalledHaveForecasts_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();

            var userId = fixture.Create<int>();
            var unitName = fixture.Create<string>();
            var expectedPeriod = fixture.Create<IEnumerable<ManpowerPeriod>>();
            var expectedDetails = fixture.Create<IEnumerable<AnnualManpowerForecastRequestDetail>>();
            expectedDetails.First().SiteId = 1;
            expectedDetails.First().SectionId = 1;
            expectedDetails.First().JobLevel = "test";
            var expectedPrevforecasts = fixture.Create<IEnumerable<AnnualManpowerForecastRequestDetail>>();
            expectedPrevforecasts.First().SiteId = 1;
            expectedPrevforecasts.First().SectionId = 1;
            expectedPrevforecasts.First().JobLevel = "test";

            var criteria = new GetDataForCreateAnnualManpowerForecastRequestCriteria();
            var criteriaForecast = new GetPreviousApprovedManpowerForecastByPeriodIdCriteria();

            baseService.FetchMultipleAsync<ManpowerPeriod, AnnualManpowerForecastRequestDetail>
                (Arg.Do<GetDataForCreateAnnualManpowerForecastRequestCriteria>(x => criteria = x)).Returns(
                new ValueTuple<IEnumerable<ManpowerPeriod>, IEnumerable<AnnualManpowerForecastRequestDetail>>(expectedPeriod, expectedDetails)); ;
            baseService.GetDataAsync<AnnualManpowerForecastRequestDetail>(Arg.Do<GetPreviousApprovedManpowerForecastByPeriodIdCriteria>(x => criteriaForecast = x)).Returns(expectedPrevforecasts);
            //Act
            var actual = await service.GetDataForCreateAnnualRequestAsync(userId, unitName);

            //Assert
            Assert.Equal(expectedPeriod.First(), actual.Item1);
            Assert.Equal(expectedDetails, actual.Item2);
        }

        [Fact]
        public async Task GetMyAnnualRequestAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();

            var userId = fixture.Create<int>();
            var expected = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();
            var criteria = new GetMyAnnualManpowerForecastRequestCriteria();

            manpowerForecastRepository.GetMyAnnualRequestAsync(Arg.Do<GetMyAnnualManpowerForecastRequestCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetMyAnnualRequestAsync(userId);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetMemberAnnualRequestAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var expected = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();
            var criteria = new GetMemberAnnualManpowerForecastRequestCriteria();

            manpowerForecastRepository.GetMemberAnnualRequestAsync(Arg.Do<GetMemberAnnualManpowerForecastRequestCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetMemberAnnualRequestAsync(criteria);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetAnnualRequestByIdAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var requestId = fixture.Create<int>();
            var expectedAnnualManpowerForecastRequest = fixture.Create<AnnualManpowerForecastRequest>();
            var expectedPrevforecasts = fixture.Create<IEnumerable<AnnualManpowerForecastRequestDetail>>();

            var criteria = new GetAnnualManpowerForecastRequestByIdCriteria();
            var criteriaForecast = new GetPreviousApprovedManpowerForecastByPeriodIdCriteria();

            manpowerForecastRepository.GetAnnualRequestByIdAsync(Arg.Do<GetAnnualManpowerForecastRequestByIdCriteria>(x => criteria = x)).Returns(expectedAnnualManpowerForecastRequest);
            baseService.GetDataAsync<AnnualManpowerForecastRequestDetail>(Arg.Do<GetPreviousApprovedManpowerForecastByPeriodIdCriteria>(x => criteriaForecast = x)).Returns(expectedPrevforecasts);
           
            //Act
            var actual = await service.GetAnnualRequestByIdAsync(requestId);

            //Assert
            Assert.Equal(expectedAnnualManpowerForecastRequest, actual);
        }

        [Fact]
        public async Task GetAnnualRequestByIdAsync_WhenCalledHaveForecasts_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var requestId = fixture.Create<int>();
            var expectedAnnualManpowerForecastRequest = fixture.Create<AnnualManpowerForecastRequest>();
            expectedAnnualManpowerForecastRequest.Details.First().SiteId = 1;
            expectedAnnualManpowerForecastRequest.Details.First().SectionId = 1;
            expectedAnnualManpowerForecastRequest.Details.First().JobLevel = "test";
            var expectedPrevforecasts = fixture.Create<IEnumerable<AnnualManpowerForecastRequestDetail>>();
            expectedPrevforecasts.First().SiteId = 1;
            expectedPrevforecasts.First().SectionId = 1;
            expectedPrevforecasts.First().JobLevel = "test";

            var criteria = new GetAnnualManpowerForecastRequestByIdCriteria();
            var criteriaForecast = new GetPreviousApprovedManpowerForecastByPeriodIdCriteria();

            manpowerForecastRepository.GetAnnualRequestByIdAsync(Arg.Do<GetAnnualManpowerForecastRequestByIdCriteria>(x => criteria = x)).Returns(expectedAnnualManpowerForecastRequest);
            baseService.GetDataAsync<AnnualManpowerForecastRequestDetail>(Arg.Do<GetPreviousApprovedManpowerForecastByPeriodIdCriteria>(x => criteriaForecast = x)).Returns(expectedPrevforecasts);

            //Act
            var actual = await service.GetAnnualRequestByIdAsync(requestId);

            //Assert
            Assert.Equal(expectedAnnualManpowerForecastRequest, actual);
        }

        [Fact]
        public async Task GetAnnualRequestByPeriodIdAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var expected = fixture.Create<IEnumerable<AnnualManpowerForecastRequest>>();
            var criteria = new GetAnnualManpowerForecastRequestByPeriodIdCriteria();

            manpowerForecastRepository.GetAnnualRequestByPeriodIdAsync(Arg.Do<GetAnnualManpowerForecastRequestByPeriodIdCriteria>(x => criteria = x)).Returns(expected);
            //Act
            var actual = await service.GetAnnualRequestByPeriodIdAsync(criteria);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateAnnualRequestAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var userId = fixture.Create<int>();
            var param = fixture.Create<CreateAnnualManpowerForecastRequestParam>();
            var expected = fixture.Create<CommandResult>();
            var command = new CreateAnnualManpowerForecastRequestCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<CreateAnnualManpowerForecastRequestCommand>(x => command = x)).Returns(expected);
            //Act
            var actual = await service.CreateAnnualRequestAsync(userId, param);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateAnnualRequestAsync_WhenCalledNullDetails_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var userId = fixture.Create<int>();
            var param = fixture.Create<CreateAnnualManpowerForecastRequestParam>();
            param.Details = null;
            var expected = fixture.Create<CommandResult>();
            var command = new CreateAnnualManpowerForecastRequestCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<CreateAnnualManpowerForecastRequestCommand>(x => command = x)).Returns(expected);
            //Act
            var actual = await service.CreateAnnualRequestAsync(userId, param);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UpdateStatusAnnualRequestAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var expected = fixture.Create<CommandResult>();
            var command = new UpdateStatusAnnualManpowerForecastRequestCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateStatusAnnualManpowerForecastRequestCommand>(x => command = x)).Returns(expected);
            //Act
            var actual = await service.UpdateStatusAnnualRequestAsync(command);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetPreviousApprovedManpowerForecastAsync_WhenCalled_ReturnExpectedData()
        {
            //Arrange
            var service = GetManpowerForecastService();
            var periodId = fixture.Create<int>();
            var expected = fixture.Create<IEnumerable<AnnualManpowerForecastRequestDetail>>();
            var criteria = new GetPreviousApprovedManpowerForecastByPeriodIdCriteria();

            baseService.GetDataAsync<AnnualManpowerForecastRequestDetail>(Arg.Do<GetPreviousApprovedManpowerForecastByPeriodIdCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetPreviousApprovedManpowerForecastAsync(periodId);

            //Assert
            Assert.Equal(expected, actual);
        }

        #region Helpers
        private ManpowerForecastService GetManpowerForecastService()
        {
            baseService = Substitute.For<IBaseService>();
            manpowerForecastRepository = Substitute.For<IManpowerForecastRepository>();
            var service = new ManpowerForecastService(manpowerForecastRepository, baseService);

            return service;
        }

        #endregion Helpers
    }
}
