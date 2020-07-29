namespace Fanex.NexHR.Services.Extensions
{
    using Fanex.NexHR.MailDeliver;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Implement;
    using Fanex.NexHR.Services.Interfaces;
    using Unity;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceCoreDI(this IUnityContainer container)
        {
            container.RegisterType<ISectionRepository, SectionRepository>();
            container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            container.RegisterType<IOvertimeRequestRepository, OvertimeRequestRepository>();
            container.RegisterType<IReportRepository, ReportRepository>();
            container.RegisterType<IRequestRepository, RequestRepository>();
            container.RegisterType<ITeamRepository, TeamRepository>();
            container.RegisterType<IWorkingCalendarRepository, WorkingCalendarRepository>();
            container.RegisterType<IDisciplineRepository, DisciplineRepository>();
            container.RegisterType<IMenuRepository, MenuRepository>();
            container.RegisterType<IScheduleArrangeRepository, ScheduleArrangeRepository>();
            container.RegisterType<IKeepInViewRequestRepository, KeepInViewRequestRepository>();
            container.RegisterType<ILeaveRequestRepository, LeaveRequestRepository>();
            container.RegisterType<IEmployeeWorkingHoursRepository, EmployeeWorkingHoursRepository>();
            container.RegisterType<ICheckInOutRequestRepository, CheckInOutRequestRepository>();
            container.RegisterType<ILateInEarlyOutRequestRepository, LateInEarlyOutRequestRepository>();
            container.RegisterType<IOvertimeReportRepository, OvertimeReportRepository>();
            container.RegisterType<IChangeShiftRequestRepository, ChangeShiftRequestRepository>();
            container.RegisterType<IDepartmentRepository, DepartmentRepository>();

            container.RegisterType<IBaseMailService, BaseMailService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<IFunctionService, FunctionService>();
            container.RegisterType<IMenuService, MenuService>();
            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<ISectionService, SectionService>();
            container.RegisterType<IUnitService, UnitService>();
            container.RegisterType<IEmployeeService, EmployeeService>();
            container.RegisterType<IExtensionNumberService, ExtensionNumberService>();
            container.RegisterType<IHolidayService, HolidayService>();
            container.RegisterType<ICommonDataService, CommonDataService>();
            container.RegisterType<IMemoService, MemoService>();
            container.RegisterType<IMessageService, MessageService>();
            container.RegisterType<IOvertimeRequestService, OvertimeRequestService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IRequestMailService, RequestMailService>();
            container.RegisterType<IRequestService, RequestService>();
            container.RegisterType<IShiftService, ShiftService>();
            container.RegisterType<ITeamService, TeamService>();
            container.RegisterType<IWorkingCalendarService, WorkingCalendarService>();
            container.RegisterType<IChangeShiftRequestService, ChangeShiftRequestService>();
            container.RegisterType<ICheckInOutRequestService, CheckInOutRequestService>();
            container.RegisterType<IKeepInViewRequestService, KeepInViewRequestService>();
            container.RegisterType<ILateInEarlyOutRequestService, LateInEarlyOutRequestService>();
            container.RegisterType<ILeaveRequestService, LeaveRequestService>();
            container.RegisterType<IDisciplineService, DisciplineService>();
            container.RegisterType<IScheduleArrangeService, ScheduleArrangeService>();
            container.RegisterType<IBackgroundJobService, BackgroundJobService>();
            container.RegisterType<IPostnatalService, PostnatalService>();
            container.RegisterType<IPaternityService, PaternityService>();
            container.RegisterType<IBackgroundJobService, BackgroundJobService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<IAnnualLeaveBalanceService, AnnualLeaveBalanceService>();
            container.RegisterType<IEmployeeWorkingHoursService, EmployeeWorkingHoursService>();
            container.RegisterType<IOvertimeReportService, OvertimeReportService>();
            container.RegisterType<IWorkingHoursSummaryReportService, WorkingHoursSummaryReportService>();
            container.RegisterType<IWorkingCalendarSummaryReportService, WorkingCalendarSummaryReportService>();
            container.RegisterType<IWorkingCalendarReportService, WorkingCalendarReportService>();
            container.RegisterType<IManpowerReportService, ManpowerReportService>();
            container.RegisterType<IActionLogService, ActionLogService>();
            container.RegisterType<IEmployeeLogService, EmployeeLogService>();
            container.RegisterType<IShiftStatisticReportService, ShiftStatisticReportService>();
            container.RegisterType<IDepartmentService, DepartmentService>();

            // excel
            container.RegisterType<IExportExcelService, ExportExcelService>();
            container.RegisterType<IExportExcelAttendanceReportService, ExportExcelAttendanceReportService>();
            container.RegisterType<IExportDisciplineDetailService, ExportDisciplineDetailService>();
            container.RegisterType<IExportDisciplineSummaryService, ExportDisciplineSummaryService>();
            container.RegisterType<IExportExcelEmployeeInfoService, ExportExcelEmployeeInfoService>();
            container.RegisterType<IExportSupervisorService, ExportSupervisorService>();
            container.RegisterType<IExportExcelUnpaidLeaveSummaryReportService, ExportExcelUnpaidLeaveSummaryReportService>();
            container.RegisterType<IExportWeekendLeaveDetailService, ExportWeekendLeaveDetailService>();
            container.RegisterType<IExportWeekendLeaveSummaryService, ExportWeekendLeaveSummaryService>();
            container.RegisterType<IExportExcelScheduleService, ExportExcelScheduleService>();
            container.RegisterType<IJobTitleExportService, JobTitleExportService>();
            container.RegisterType<IExportExcelShiftStatisticService, ExportExcelShiftStatisticService>();
            container.RegisterType<IExportExcelTimeRecordService, ExportExcelTimeRecordService>();
            container.RegisterType<IExportOvertimeService, ExportOvertimeService>();
        }
    }
}