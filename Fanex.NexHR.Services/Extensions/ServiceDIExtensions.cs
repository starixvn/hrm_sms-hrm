namespace Fanex.NexHR.Services.Extensions
{
    using Fanex.NexHR.MailDeliver;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services.ExportExcel;
    using Fanex.NexHR.Services.ExportExcel.JobTitle;
    using Implement;
    using Interfaces;
    using Unity;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceCoreDI(this UnityContainer container)
        {
            container.RegisterType<IDepartmentRepository, DepartmentRepository>();
            container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            container.RegisterType<IOvertimeRequestRepository, OvertimeRequestRepository>();
            container.RegisterType<IReportRepository, ReportRepository>();
            container.RegisterType<IRequestRepository, RequestRepository>();
            container.RegisterType<ITeamRepository, TeamRepository>();
            container.RegisterType<IWorkingCalendarRepository, WorkingCalendarRepository>();
            container.RegisterType<IDisciplineRepository, DisciplineRepository>();
            container.RegisterType<IMenuRepository, MenuRepository>();
            container.RegisterType<IScheduleArrangeRepository, ScheduleArrangeRepository>();

            container.RegisterType<IBaseMailService, BaseMailService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<IFunctionService, FunctionService>();
            container.RegisterType<IMenuService, MenuService>();
            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IDepartmentService, DepartmentService>();
            container.RegisterType<IEmployeeService, EmployeeService>();
            container.RegisterType<IExtensionNumberService, ExtensionNumberService>();
            container.RegisterType<IHolidayService, HolidayService>();
            container.RegisterType<ILeaveTypeService, LeaveTypeService>();
            container.RegisterType<ILookupDataService, LookupDataService>();
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

            // excel
            container.RegisterType<IExportExcelService, ExportExcelService>();
            container.RegisterType<IExportExcelAttendanceReportService, ExportExcelAttendanceReportService>();
            container.RegisterType<IExportDisciplineDetailService, ExportDisciplineDetailService>();
            container.RegisterType<IExportDisciplineSummaryService, ExportDisciplineSummaryService>();
            container.RegisterType<IExportExcelEmployeeInfoService, ExportExcelEmployeeInfoService>();
            container.RegisterType<IExportExcelManpowerCategoryService, ExportExcelManpowerCategoryService>();
            container.RegisterType<IExportExcelManpowerService, ExportExcelManpowerService>();
            container.RegisterType<IExportExcelOvertimeReportService, ExportExcelOvertimeReportService>();
            container.RegisterType<IExportExcelYearlyOvertimeReportService, ExportExcelYearlyOvertimeReportService>();
            container.RegisterType<IExportExcelEmployeeSupervisor, ExportExcelEmployeeSupervisor>();
            container.RegisterType<IExportExcelSectionSupervisorService, ExportExcelSectionSupervisorService>();
            container.RegisterType<IExportExcelUnpaidLeaveSummaryReportService, ExportExcelUnpaidLeaveSummaryReportService>();
            container.RegisterType<IExportWeekendLeaveDetailService, ExportWeekendLeaveDetailService>();
            container.RegisterType<IExportWeekendLeaveSummaryService, ExportWeekendLeaveSummaryService>();
            container.RegisterType<IExportExcelScheduleService, ExportExcelScheduleService>();
            container.RegisterType<IJobTitleExportService, JobTitleExportService>();
        }
    }
}