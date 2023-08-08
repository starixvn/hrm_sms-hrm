namespace GO.Website.Controllers.Manpower
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Website.Filters;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.Report;
    using GO.Website.Models.Report.Manpower;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using Microsoft.AspNetCore.Mvc;

    [GOPermission(FunctionKey = FunctionKey.ManpowerReport, Policy = Policy.Read)]
    public class ManpowerReportController : BaseController
    {
        private readonly Dictionary<string, string[]> units = new Dictionary<string, string[]>
        {
            { "Product Unit", new [] { "PU01", "PU02", "PU03", "PU04", "PU05", "PU06", "PU07", "PU08", "PU09", "PU10" } },
            { "Business Unit", new [] { "BU01", "BU02", "BU03", "BU04" } },
            { "Support Unit", new [] { "SU01", "SU02", "SU03", "SU04" } }
        };

        private readonly Dictionary<string, string[]> jobLevels = new Dictionary<string, string[]>
        {
            { "Supervisor", new [] { "M3-A", "M3-B", "P3-A", "P3-B", "M2-A", "M2-B", "P2-A", "P2-B", "M1-A", "M1-B", "P1-A", "P1-B" } },
            { "Employee", new []{ "E3-A", "E3-B", "E3-C", "E2-A", "E2-B", "E2-C", "E1-A", "E1-B", "E1-C", "E0-A", "E0-B", "E0-C" } }
        };

        private readonly IBaseService baseService;
        private readonly IExportExcelService excelService;

        public ManpowerReportController(
            IBaseService baseService,
            IExportExcelService excelService)
        {
            this.baseService = baseService;
            this.excelService = excelService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Report/Manpower.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var criteria = new GetManpowerReportCriteria();
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            GridData gridCountryData = GroupDataByCountry(jobLevels, units, employeeInfos);
            GridData gridUnitData = GroupDataByUnit(jobLevels, units, employeeInfos);

            return Json(new { employeeInfos, gridCountryData, gridUnitData });
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel()
        {
            var criteria = new GetManpowerReportCriteria();
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            GridData gridCountryData = GroupDataByCountry(jobLevels, units, employeeInfos);
            GridData gridUnitData = GroupDataByUnit(jobLevels, units, employeeInfos);

            var bytes = excelService.ExportManpowerReport(gridUnitData, gridCountryData);
            var reportName = $"ManpowerReport_{DateTime.Today:yyyyMMdd}";
            return File(bytes, "application/xlsx", $"{reportName}.xlsx");
        }

        private static GridData GroupDataByCountry(
            Dictionary<string, string[]> jobLevels,
            Dictionary<string, string[]> units,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            IList<string> countries = employeeInfos.Where(emp => !string.IsNullOrWhiteSpace(emp.Country))
                .Select(emp => emp.Country).Distinct().OrderBy(item => item).ToList();
            (IList<RowItem> supervisors, RowItem totalSupervisor) = GroupDataByCountry(jobLevels.ElementAt(0).Value, countries, units, employeeInfos);
            (IList<RowItem> employees, RowItem totalEmployee) = GroupDataByCountry(jobLevels.ElementAt(1).Value, countries, units, employeeInfos);
            RowItem grandTotal = GetGrandTotal(countries, totalSupervisor, totalEmployee);
            var headers = countries.Select(country => new RowItem
            {
                Name = country,
                Cells = units.Select(gunit => new GroupCellItem
                {
                    Name = gunit.Key,
                    Cells = gunit.Value.Select(unit => new CellItem
                    {
                        Name = unit
                    }).ToList()
                }).ToList()
            }).ToList();

            return new GridData
            {
                Headers = headers,
                Supervisors = supervisors,
                Employees = employees,
                TotalSupervisor = totalSupervisor,
                TotalEmployee = totalEmployee,
                GrandTotal = grandTotal
            };
        }

        private static RowItem GetGrandTotal(
            IList<string> countries,
            RowItem totalSupervisor,
            RowItem totalEmployee)
        {
            return new RowItem
            {
                Name = "Grand Total",
                Cells = countries.Select(country =>
                {
                    return new GroupCellItem
                    {
                        Name = country,
                        Cells = new List<CellItem>
                        {
                            new CellItem
                            {
                                Total = totalSupervisor.Cells.First(gcell => country.Equals(gcell.Name)).Cells.Sum(cell => cell.Total)
                                    + totalEmployee.Cells.First(gcell => country.Equals(gcell.Name)).Cells.Sum(cell => cell.Total)
                            }
                        }
                    };
                }).ToList()
            };
        }

        private static (IList<RowItem>, RowItem) GroupDataByCountry(
            string[] jobLevels,
            IList<string> countries,
            Dictionary<string, string[]> units,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var rows = new List<RowItem>();
            var totalRow = new RowItem
            {
                Name = "Total",
                Cells = countries.Select(country => new GroupCellItem
                {
                    Name = country,
                    Cells = units.Select(unit => new CellItem
                    {
                        Name = unit.Key
                    }).ToList()
                }).ToList()
            };
            foreach (var jobLevel in jobLevels)
            {
                var employees = employeeInfos.Where(emp => jobLevel.Equals(emp.JobLevel, StringComparison.CurrentCultureIgnoreCase)).ToList();
                var row = new RowItem
                {
                    Name = jobLevel,
                    Cells = new List<GroupCellItem>()
                };
                foreach (var country in countries)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = country,
                        Cells = new List<CellItem>()
                    };

                    foreach (var unit in units)
                    {
                        var cell = new CellItem
                        {
                            Name = unit.Key,
                            Total = employees
                                .Count(emp => country.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase) && unit.Value.Contains(emp.UnitName))
                        };
                        groupCell.Cells.Add(cell);

                        totalRow.Cells.First(tcell => tcell.Name.Equals(country, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(tcell => tcell.Name.Equals(unit.Key, StringComparison.CurrentCultureIgnoreCase)).Total += cell.Total;
                    }

                    row.Cells.Add(groupCell);
                }

                rows.Add(row);
            }

            return (rows, totalRow);
        }

        public static GridData GroupDataByUnit(
            Dictionary<string, string[]> jobLevels,
            Dictionary<string, string[]> units,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            (IList<RowItem> supervisors, RowItem totalSupervisor) = GroupDataByUnit(jobLevels.ElementAt(0).Value, units, employeeInfos);
            (IList<RowItem> employees, RowItem totalEmployee) = GroupDataByUnit(jobLevels.ElementAt(1).Value, units, employeeInfos);
            RowItem grandTotal = GetGrandTotal(units, totalSupervisor, totalEmployee);
            var headers = units.Select(group => new RowItem
            {
                Name = group.Key,
                Cells = group.Value.Select(unit => new GroupCellItem { Name = unit }).ToList()
            }).ToList();

            return new GridData
            {
                Headers = headers,
                Supervisors = supervisors,
                Employees = employees,
                TotalSupervisor = totalSupervisor,
                TotalEmployee = totalEmployee,
                GrandTotal = grandTotal
            };
        }

        private static (IList<RowItem>, RowItem) GroupDataByUnit(
            string[] jobLevels,
            Dictionary<string, string[]> gunits,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var rows = new List<RowItem>();
            var totalRow = new RowItem
            {
                Name = "Total",
                Cells = gunits.Select(gunit => new GroupCellItem
                {
                    Name = gunit.Key,
                    Cells = gunit.Value.Select(unit => new CellItem
                    {
                        Name = unit
                    }).ToList()
                }).ToList()
            };
            foreach (var jobLevel in jobLevels)
            {
                var employees = employeeInfos.Where(emp => jobLevel.Equals(emp.JobLevel, StringComparison.CurrentCultureIgnoreCase)).ToList();
                var row = new RowItem
                {
                    Name = jobLevel,
                    Cells = new List<GroupCellItem>()
                };
                foreach (var gunit in gunits)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = gunit.Key,
                        Cells = new List<CellItem>()
                    };

                    foreach (var unit in gunit.Value)
                    {
                        var cell = new CellItem
                        {
                            Name = unit,
                            TotalLocal = employees.Count(emp => unit.Equals(emp.UnitName, StringComparison.CurrentCultureIgnoreCase) && emp.IsLocal),
                            TotalExpat = employees.Count(emp => unit.Equals(emp.UnitName, StringComparison.CurrentCultureIgnoreCase) && !emp.IsLocal)
                        };
                        groupCell.Cells.Add(cell);

                        var totalCell = totalRow.Cells.First(tcell => tcell.Name.Equals(gunit.Key, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(tcell => tcell.Name.Equals(unit, StringComparison.CurrentCultureIgnoreCase));
                        totalCell.TotalLocal += cell.TotalLocal;
                        totalCell.TotalExpat += cell.TotalExpat;
                    }

                    row.Cells.Add(groupCell);
                }

                rows.Add(row);
            }

            return (rows, totalRow);
        }

        private static RowItem GetGrandTotal(
            Dictionary<string, string[]> gunits,
            RowItem totalSupervisor,
            RowItem totalEmployee)
        {
            return new RowItem
            {
                Name = "Grand Total",
                Cells = gunits.Select(gunit => new GroupCellItem
                {
                    Name = gunit.Key,
                    Cells = gunit.Value.Select(unit =>
                    {
                        var supervisor = totalSupervisor.Cells.First(gcell => gunit.Key.Equals(gcell.Name)).Cells.First(cell => unit.Equals(cell.Name));
                        var employee = totalEmployee.Cells.First(gcell => gunit.Key.Equals(gcell.Name)).Cells.First(cell => unit.Equals(cell.Name));

                        return new CellItem
                        {
                            Name = unit,
                            TotalLocal = supervisor.TotalLocal + employee.TotalLocal,
                            TotalExpat = supervisor.TotalExpat + employee.TotalExpat
                        };
                    }).ToList()
                }).ToList()
            };
        }
    }
}