namespace GO.Website.Controllers.Manpower
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.Report;
    using GO.Website.Filters;
    using GO.Website.Models.Report.Manpower;
    using GO.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using Microsoft.AspNetCore.Mvc;

    [GOPermission(FunctionKey = FunctionKey.JobLevelSummaryReport, Policy = Policy.Read)]
    public class JobLevelSummaryReportController : BaseController
    {
        private const byte LengthJobLevelType = 2;

        private readonly Dictionary<string, string> unitTypes = new()
        {
            { "Product Unit", "PU" },
            { "Business Unit", "BU" },
            { "Support Unit", "SU" }
        };

        private readonly Dictionary<string, string[]> jobLevelTypes = new()
        {
            { "Supervisor", new[] { "M", "P" } },
            { "Employee", new[] { "E" } }
        };

        private readonly IBaseService baseService;
        private readonly IExportExcelService excelService;

        public JobLevelSummaryReportController(
            IBaseService baseService,
            IExportExcelService excelService)
        {
            this.baseService = baseService;
            this.excelService = excelService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Report/JobLevelSummaryReport.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var criteria = new GetManpowerReportCriteria();
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);

            var listUnits = await baseService.GetDataAsync<string>(new GetUnitCriteria());
            var listJobLevels = await baseService.GetDataAsync<string>(new GetJobLevelCriteria());

            var units = GetUnit(listUnits);
            var jobLevels = GetJobLevels(listJobLevels);
            var jobLevelsType = GetJobLevelTypes(listJobLevels);

            GridData gridCountryData = GroupDataByCountry(jobLevels, units, jobLevelsType, employeeInfos);
            GridData gridUnitData = GroupDataByUnit(jobLevels, units, jobLevelsType, employeeInfos);

            return Json(new { employeeInfos, gridCountryData, gridUnitData, jobLevelsType });
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel()
        {
            var criteria = new GetManpowerReportCriteria();
            var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);
            var listUnits = await baseService.GetDataAsync<string>(new GetUnitCriteria());
            var listJobLevels = await baseService.GetDataAsync<string>(new GetJobLevelCriteria());

            var units = GetUnit(listUnits);
            var jobLevels = GetJobLevels(listJobLevels);
            var jobLevelsType = GetJobLevelTypes(listJobLevels);

            GridData gridCountryData = GroupDataByCountry(jobLevels, units, jobLevelsType, employeeInfos);
            GridData gridUnitData = GroupDataByUnit(jobLevels, units, jobLevelsType, employeeInfos);

            var bytes = excelService.ExportManpowerReport(gridUnitData, gridCountryData);
            var reportName = $"JobLevelSummaryReportByUnitAndCountry_{DateTime.Today:yyyyMMdd}";
            return File(bytes, "application/xlsx", $"{reportName}.xlsx");
        }

        private Dictionary<string, string[]> GetUnit(IEnumerable<string> units)
        {
            var dictionaryUnits = new Dictionary<string, string[]>();
            foreach (var unitType in unitTypes)
            {
                var unitNames = units.Where(x => x.Substring(0, LengthJobLevelType).Contains(unitType.Value)).ToArray();
                dictionaryUnits.Add(unitType.Key, unitNames);
            }
            return dictionaryUnits;
        }

        private Dictionary<string, string[]> GetJobLevels(IEnumerable<string> jobLevels)
        {
            var dictionaryJobLevels = new Dictionary<string, string[]>();
            foreach (var jobLevelType in jobLevelTypes)
            {
                var jobLevelNames = new List<string>();
                foreach (var item in jobLevelType.Value)
                {
                    jobLevelNames.AddRange(jobLevels.Where(x => x.Substring(0, 1).Contains(item)).ToList());
                }
                dictionaryJobLevels.Add(jobLevelType.Key, jobLevelNames.ToArray());
            }
            return dictionaryJobLevels;
        }

        private Dictionary<string, string[]> GetJobLevelTypes(IEnumerable<string> jobLevels)
        {
            var dictionaryJobLevelTypes = new Dictionary<string, string[]>();
            foreach (var jobLevelType in jobLevelTypes)
            {
                var jobLevelNames = new List<string>();
                foreach (var item in jobLevelType.Value)
                {
                    jobLevelNames.AddRange(jobLevels.Where(x => x.Substring(0, 1).Contains(item))
                        .Select(y => y.Substring(0, LengthJobLevelType)).Distinct().ToArray());
                }

                dictionaryJobLevelTypes.Add(jobLevelType.Key, SortJobLevel(jobLevelNames));
            }
            return dictionaryJobLevelTypes;
        }

        private static string[] SortJobLevel(List<string> jobLevelNames)
        {
            var sortJobLevel = new List<string>();
            foreach (var item in jobLevelNames)
            {
                var range = int.Parse(item[1..]);
                sortJobLevel.AddRange(jobLevelNames.Where(x => int.Parse(x[1..]).Equals(range) && !sortJobLevel.Any(y => y == x)).ToList());
            }
            return sortJobLevel.ToArray();
        }

        private static GridData GroupDataByCountry(
            Dictionary<string, string[]> jobLevels,
            Dictionary<string, string[]> units,
            Dictionary<string, string[]> jobLevelsType,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            IList<string> countries = employeeInfos.Where(emp => !string.IsNullOrWhiteSpace(emp.Country))
                .Select(emp => emp.Country).Distinct().OrderBy(item => item).ToList();
            (IList<RowItem> supervisors, RowItem totalSupervisor) = GroupDataByCountry(jobLevels.ElementAt(0).Value, countries, units, employeeInfos);
            (IList<RowItem> employees, RowItem totalEmployee) = GroupDataByCountry(jobLevels.ElementAt(1).Value, countries, units, employeeInfos);
            var totalJobTypeSupervisor = GetTotalByJobType(countries, units, jobLevelsType.ElementAt(0).Value, employeeInfos);
            var totalJobTypeEmployee = GetTotalByJobType(countries, units, jobLevelsType.ElementAt(1).Value, employeeInfos);
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
                TotalJobTypeSupervisor = totalJobTypeSupervisor,
                TotalJobTypeEmployee = totalJobTypeEmployee,
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
                Name = "Total " + GetJobTypeLevel(jobLevels) + " Level",
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
            Dictionary<string, string[]> jobLevelsType,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            (IList<RowItem> supervisors, RowItem totalSupervisor) = GroupDataByUnit(jobLevels.ElementAt(0).Value, units, employeeInfos);
            (IList<RowItem> employees, RowItem totalEmployee) = GroupDataByUnit(jobLevels.ElementAt(1).Value, units, employeeInfos);
            RowItem grandTotal = GetGrandTotal(units, totalSupervisor, totalEmployee);
            var totalJobTypeSupervisor = GetTotalByJobType(units, jobLevelsType.ElementAt(0).Value, employeeInfos);
            var totalJobTypeEmployee = GetTotalByJobType(units, jobLevelsType.ElementAt(1).Value, employeeInfos);
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
                TotalJobTypeSupervisor = totalJobTypeSupervisor,
                TotalJobTypeEmployee = totalJobTypeEmployee,
                GrandTotal = grandTotal
            };
        }

        private static string GetJobTypeLevel(string[] jobLevels)
        {
            var jobType = string.Empty;
            foreach (var item in jobLevels)
            {
                if (!jobType.Contains(item.Substring(0, 1)))
                {
                    jobType += jobType != string.Empty ? (", " + item.Substring(0, 1)) : item.Substring(0, 1);
                }
            }
            return jobType;
        }

        private static (IList<RowItem>, RowItem) GroupDataByUnit(
            string[] jobLevels,
            Dictionary<string, string[]> gunits,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var rows = new List<RowItem>();
            var totalRow = new RowItem
            {
                Name = "Total " + GetJobTypeLevel(jobLevels) + " Level",
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

        private static List<RowItem> GetTotalByJobType(
            Dictionary<string, string[]> gunits,
            string[] jobLevelsType,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var totalJobType = new List<RowItem>();
            foreach (var item in jobLevelsType)
            {
                totalJobType.Add(new RowItem
                {
                    Name = item,
                    Cells = gunits.Select(gunit => new GroupCellItem
                    {
                        Name = gunit.Key,
                        Cells = gunit.Value.Select(unit => new CellItem
                        {
                            Name = unit
                        }).ToList()
                    }).ToList()
                });
            };

            foreach (var item in totalJobType)
            {
                var employees = employeeInfos.Where(emp =>
                    item.Name.Equals(emp.JobLevel.Substring(0, LengthJobLevelType), StringComparison.CurrentCultureIgnoreCase)).ToList();
                var row = new RowItem
                {
                    Name = item.Name,
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

                        var totalCell = item.Cells.First(tcell => tcell.Name.Equals(gunit.Key, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(tcell => tcell.Name.Equals(unit, StringComparison.CurrentCultureIgnoreCase));
                        totalCell.TotalLocal += cell.TotalLocal;
                        totalCell.TotalExpat += cell.TotalExpat;
                    }
                    row.Cells.Add(groupCell);
                }
            }

            return totalJobType;
        }

        private static List<RowItem> GetTotalByJobType(
            IList<string> countries,
            Dictionary<string, string[]> gunits,
            string[] jobLevelsType,
            IEnumerable<EmployeeInfo> employeeInfos)
        {
            var totalJobType = new List<RowItem>();
            foreach (var item in jobLevelsType)
            {
                totalJobType.Add(new RowItem
                {
                    Name = item,
                    Cells = countries.Select(country => new GroupCellItem
                    {
                        Name = country,
                        Cells = gunits.Select(unit => new CellItem
                        {
                            Name = unit.Key
                        }).ToList()
                    }).ToList()
                });
            };

            foreach (var item in totalJobType)
            {
                var employees = employeeInfos.Where(emp =>
                    item.Name.Equals(emp.JobLevel.Substring(0, LengthJobLevelType), StringComparison.CurrentCultureIgnoreCase)).ToList();
                var row = new RowItem
                {
                    Name = item.Name,
                    Cells = new List<GroupCellItem>()
                };

                foreach (var country in countries)
                {
                    var groupCell = new GroupCellItem
                    {
                        Name = country,
                        Cells = new List<CellItem>()
                    };
                    foreach (var unit in gunits)
                    {
                        var cell = new CellItem
                        {
                            Name = unit.Key,
                            Total = employees
                                .Count(emp => country.Equals(emp.Country, StringComparison.CurrentCultureIgnoreCase) && unit.Value.Contains(emp.UnitName))
                        };
                        groupCell.Cells.Add(cell);

                        item.Cells.First(tcell => tcell.Name.Equals(country, StringComparison.CurrentCultureIgnoreCase))
                            .Cells.First(tcell => tcell.Name.Equals(unit.Key, StringComparison.CurrentCultureIgnoreCase)).Total += cell.Total;
                    }

                    row.Cells.Add(groupCell);
                }
            }

            return totalJobType;
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