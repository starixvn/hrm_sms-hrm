import React from 'react';

import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import { renderNumberOfHeadcount } from '../../../common.funcs';
import Equal from '@jsroot/components/CompareText';

const DataGrid = ({ requests, companies, unitTypes, periodSelected }) => {
    const MaxStep = 4;

    const renderFirstRowHeader = () => {
        const year = periodSelected.year;
        const totalCompany = companies.length;
        const defaultWidth = 280 * totalCompany;
        return (
            <tr key="first-row-header">
                <th className="name-step-background hide-border fixed-header fixed-header-unit" rowSpan={3} colSpan={2}>{LangHelper.getResource('Unit')}</th>
                <th className="name-step-background hide-border fixed-header fixed-header-department" rowSpan={3}>{LangHelper.getResource('Deparment')}</th>
                <th className="name-step-background section hide-border fixed-header fixed-header-section" rowSpan={3}>{LangHelper.getResource('Section')}</th>
                <th className="name-step-background" style={{ width: defaultWidth }} colSpan={3 * totalCompany + 1}>
                    {(year - 1) + LangHelper.getResource('ManpowerHeadcountCurrent')}
                </th>
                <th className="name-step-background" style={{ width: defaultWidth }} colSpan={3 * totalCompany + 1}>
                    {(year - 1) + LangHelper.getResource('ApprovedForecastHeadcount')}
                </th>
                <th className="name-step-background" style={{ width: defaultWidth }} colSpan={3 * totalCompany + 1}>
                    {LangHelper.getResource('AdditionalHeadcount')}
                </th>
                <th className="name-step-background" style={{ width: defaultWidth }} colSpan={3 * totalCompany + 1}>
                    {year + LangHelper.getResource('ManpowerHeadcountForecast')}
                </th>
            </tr>);
    };

    const renderSecondRowHeader = () => {
        const headers = [];
        for (let step = 0; step < MaxStep; step++) {
            companies.map(company => {
                headers.push(<th className="name-company-background" key={`${company}-${step}`} colSpan={3}>{company}</th>);
            });

            headers.push(
                <th className="name-step-background grand-total hide-border"
                    key={`grand-total-${step}`} rowSpan={2}
                >{LangHelper.getResource('GrandTotal')}</th>
            );
        }
        return headers;
    };

    const renderThirdRowHeader = () => {
        const headers = [];
        for (let step = 0; step < MaxStep; step++) {
            companies.map(company => {
                headers.push(
                    <th className="name-company-background" key={`${company}-${step}-local`} >{LangHelper.getResource('Local')}</th>,
                    <th className="name-company-background" key={`${company}-${step}-expat`}>{LangHelper.getResource('Expat')}</th>,
                    <th className="name-company-background" key={`${company}-${step}-total`}>{LangHelper.getResource('Total')}</th>
                );
            });
        }

        return headers;
    };

    const buildHeader = () => {
        const row = [];
        row.push(renderFirstRowHeader());
        row.push(
            <tr key="second-row-header">
                {renderSecondRowHeader()}
            </tr>,
            <tr key="third-row-header">
                {renderThirdRowHeader()}
            </tr>
        );

        return row;
    };

    const getSection = (request, detaildepartment) => {
        const sections = [];
        request.details.map(detail => {
            if (Equal(detail.departmentName, detaildepartment.departmentName)
                && sections.findIndex(section => Equal(section, detail.sectionName)) === -1) {
                sections.push(detail.sectionName);
            }
        });
        return sections;
    };

    const getDepartment = request => {
        const departments = [];
        request.details.map(detail => {
            if (departments.findIndex(x => Equal(x.departmentName, detail.departmentName)) === -1) {
                departments.push({
                    departmentName: detail.departmentName,
                    sections: getSection(request, detail)
                });
            }
        });
        return departments;
    };

    const renderCurrentHeadcount = (request, deparment, section) => {
        const rows = [];
        let grandtotal = 0;
        const unitType = request.unitName.slice(0, 2);
        companies.map(company => {
            let checkNone = 0;
            const key = `section-CurrentHeadcount-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-${company}`;

            request.details.map(detail => {
                if (detail.unitName === request.unitName && Equal(detail.departmentName, deparment.departmentName)
                    && Equal(detail.sectionName, section) && company === detail.companyName) {
                    grandtotal += detail.totalCurrentHeadcount;
                    checkNone++;

                    rows.push(
                        <td key={`${key}-local`}>
                            {renderNumberOfHeadcount(detail.localCurrentHeadcount)}
                        </td>,
                        <td key={`${key}-expat`}>
                            {renderNumberOfHeadcount(detail.expatCurrentHeadcount)}
                        </td>,
                        <td key={`${key}-total`}>
                            {renderNumberOfHeadcount(detail.totalCurrentHeadcount)}
                        </td>);
                }
            });
            if (checkNone === 0) {
                rows.push(
                    <td key={`${key}-local`}></td>,
                    <td key={`${key}-expat`}></td>,
                    <td key={`${key}-total`}></td>
                );
            }
        });

        const key = `section-CurrentHeadcount-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-grantotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderApprovedForecastHeadcount = (request, deparment, section) => {
        const rows = [];
        let grandtotal = 0;
        const unitType = request.unitName.slice(0, 2);
        companies.map(company => {
            let checkNone = 0;
            const key = `section-ApprovedForecast-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-${company}`;

            request.details.map(detail => {
                if (detail.unitName === request.unitName && Equal(detail.departmentName, deparment.departmentName)
                    && Equal(detail.sectionName, section) && company === detail.companyName) {
                    grandtotal += detail.totalApprovedHeadcount;
                    checkNone++;

                    rows.push(
                        <td key={`${key}-local`}>
                            {renderNumberOfHeadcount(detail.localApprovedHeadcount)}
                        </td>,
                        <td key={`${key}-expat`}>
                            {renderNumberOfHeadcount(detail.expatApprovedHeadcount)}
                        </td>,
                        <td key={`${key}-total`}>
                            {renderNumberOfHeadcount(detail.totalApprovedHeadcount)}
                        </td>
                    );
                }
            });
            if (checkNone === 0) {
                rows.push(
                    <td key={`${key}-local`}></td>,
                    <td key={`${key}-expat`}></td>,
                    <td key={`${key}-total`}></td>
                );
            }
        });

        const key = `section-ApprovedForecast-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-grantotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderAdditionalHeadcount = (request, deparment, section) => {
        const rows = [];
        let grandtotal = 0;
        const unitType = request.unitName.slice(0, 2);

        companies.map(company => {
            let checkNone = 0;
            const key = `section-Additional-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-${company}`;

            request.details.map(detail => {
                if (detail.unitName === request.unitName && Equal(detail.departmentName, deparment.departmentName)
                    && Equal(detail.sectionName, section) && company === detail.companyName) {
                    grandtotal += detail.totalAddition;
                    checkNone++;

                    rows.push(
                        <td key={`${key}-local`}>
                            {renderNumberOfHeadcount(detail.localAddition)}
                        </td>,
                        <td key={`${key}-expat`}>
                            {renderNumberOfHeadcount(detail.expatAddition)}
                        </td>,
                        <td key={`${key}-total`}>
                            {renderNumberOfHeadcount(detail.totalAddition)}
                        </td>
                    );
                }
            });
            if (checkNone === 0) {
                rows.push(
                    <td key={`${key}-local`}></td>,
                    <td key={`${key}-expat`}></td>,
                    <td key={`${key}-total`}></td>
                );
            }
        });

        const key = `section-Additional-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-grantotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderForecastHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        const unitType = request.unitName.slice(0, 2);

        companies.map(company => {
            let checkNone = 0;
            const key = `section-Forecast-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-${company}`;

            request.details.map(detail => {
                if (detail.unitName === request.unitName && Equal(detail.departmentName, deparment.departmentName)
                    && Equal(detail.sectionName, section) && company === detail.companyName) {
                    grandtotal += detail.totalApprovedHeadcount + detail.totalAddition;
                    checkNone++;

                    row.push(
                        <td key={`${key}-local`}>
                            {renderNumberOfHeadcount(detail.localApprovedHeadcount + detail.localAddition)}
                        </td>,
                        <td key={`${key}-expat`}>
                            {renderNumberOfHeadcount(detail.expatApprovedHeadcount + detail.expatAddition)}
                        </td>,
                        <td key={`${key}-total`}>
                            {renderNumberOfHeadcount(detail.totalApprovedHeadcount + detail.totalAddition)}
                        </td>
                    );
                }
            });
            if (checkNone === 0) {
                row.push(
                    <td key={`${key}-local`}></td>,
                    <td key={`${key}-expat`}></td>,
                    <td key={`${key}-total`}></td>
                );
            }
        });

        const key = `section-Forecast-${unitType}-${request.unitName}-${deparment.departmentName}-${section}-grantotal`;
        row.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return row;
    };

    const renderSection = (deparment, request) => {
        const sectionRow = [];
        let rowSpanDeparment = 2;
        deparment.sections.map(section => {
            rowSpanDeparment++;
            const headcountRow = [];
            headcountRow.push(renderCurrentHeadcount(request, deparment, section));
            headcountRow.push(renderApprovedForecastHeadcount(request, deparment, section));
            headcountRow.push(renderAdditionalHeadcount(request, deparment, section));
            headcountRow.push(renderForecastHeadcount(request, deparment, section));
            sectionRow.push(
                <tr className="highlight-row" key={request.unitName + deparment.departmentName + section}>
                    <td className="text-left fixed-body fixed-body-section">{section}</td>
                    {headcountRow}
                </tr>
            );
        });
        return { sectionRow, rowSpanDeparment };
    };

    const renderTotalCurrentHeadcountDepartment = (request, deparment) => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName
                    && Equal(detail.departmentName, deparment.departmentName)
                    && company === detail.companyName) {
                    local += detail.localCurrentHeadcount;
                    expat += detail.expatCurrentHeadcount;
                }
            });

            const key = `department-${company}-${deparment.departmentName}-totalcurrentheadcount`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>
            );
            grandtotal += local + expat;
        });

        const key = `department-${deparment.departmentName}-totalcurrentheadcount-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderTotalApprovedForecastHeadcountDepartment = (request, deparment) => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName
                    && Equal(detail.departmentName, deparment.departmentName)
                    && company === detail.companyName) {
                    local += detail.localApprovedHeadcount;
                    expat += detail.expatApprovedHeadcount;
                }
            });

            const key = `department-${company}-${deparment.departmentName}-totalApprovedForecastheadcount`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>
            );
            grandtotal += local + expat;
        });

        const key = `department-${deparment.departmentName}-totalApprovedForecastheadcount-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderTotalAdditionalHeadcountDepartment = (request, deparment) => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName
                    && Equal( detail.departmentName, deparment.departmentName)
                    && company === detail.companyName) {
                    local += detail.localAddition;
                    expat += detail.expatAddition;
                }
            });

            const key = `department-${company}-${deparment.departmentName}-totalAdditionalheadcount`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>
            );
            grandtotal += local + expat;
        });

        const key = `department-${deparment.departmentName}-totalApprovedForecastheadcount-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderTotalForecastHeadcountDepartment = (request, deparment) => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName
                    && Equal(detail.departmentName, deparment.departmentName)
                    && company === detail.companyName) {
                    local += detail.localApprovedHeadcount + detail.localAddition;
                    expat += detail.expatApprovedHeadcount + detail.expatAddition;
                }
            });

            const key = `department-${company}-${deparment.departmentName}-totalForecastheadcount`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>
            );
            grandtotal += local + expat;
        });

        const key = `department-${deparment.departmentName}-totalForecastheadcount-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderTotalHeadcountDepartment = (request, deparment) => {
        const totalHeadcountDepartmentRow = [];
        totalHeadcountDepartmentRow.push(renderTotalCurrentHeadcountDepartment(request, deparment));
        totalHeadcountDepartmentRow.push(renderTotalApprovedForecastHeadcountDepartment(request, deparment));
        totalHeadcountDepartmentRow.push(renderTotalAdditionalHeadcountDepartment(request, deparment));
        totalHeadcountDepartmentRow.push(renderTotalForecastHeadcountDepartment(request, deparment));
        return (
            <tr className="total-department" key={`${request.unitName}-${deparment.departmentName}-totalHeadcount`}>
                <td className="text-left fixed-body fixed-body-total-department">Total Headcount</td>
                {totalHeadcountDepartmentRow}
            </tr>
        );
    };

    const calculaTotalCurrentHeadcountUnitType = (company, unitType) => {
        let local = 0;
        let expat = 0;
        requests.map(request => {
            if (unitType === request.unitName.slice(0, 2)) {
                request.details.map(detail => {
                    if (company === detail.companyName) {
                        local += detail.localCurrentHeadcount;
                        expat += detail.expatCurrentHeadcount;
                    }
                });
            }
        });
        return { local, expat };
    };

    const renderTotalCurrentHeadcountUnitType = unitType => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalCurrentHeadcountUnitType(company, unitType);
            const key = `unitType-${company}-${unitType}-totalcurrentheadcount`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>
            );
            grandtotal += local + expat;
        });

        const key = `unitType-${unitType}-totalcurrentheadcount-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const calculaTotalApprovedHeadcountUnitType = (company, unitType) => {
        let local = 0;
        let expat = 0;
        requests.map(request => {
            if (unitType === request.unitName.slice(0, 2)) {
                request.details.map(detail => {
                    if (company === detail.companyName) {
                        local += detail.localApprovedHeadcount;
                        expat += detail.expatApprovedHeadcount;
                    }
                });
            }
        });
        return { local, expat };
    };

    const renderTotalApprovedForecastHeadcountUnitType = unitType => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalApprovedHeadcountUnitType(company, unitType);
            const key = `unitType-${company}-${unitType}-totalApprovedforecast`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>);
            grandtotal += local + expat;
        });

        const key = `unitType-${unitType}-totalApprovedforecast-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const calculaTotalAdditionalHeadcountUnitType = (company, unitType) => {
        let local = 0;
        let expat = 0;
        requests.map(request => {
            if (unitType === request.unitName.slice(0, 2)) {
                request.details.map(detail => {
                    if (company === detail.companyName) {
                        local += detail.localAddition;
                        expat += detail.expatAddition;
                    }
                });
            }
        });
        return { local, expat };
    };

    const renderTotalAdditionalHeadcountUnitType = unitType => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalAdditionalHeadcountUnitType(company, unitType);
            const key = `unitType-${company}-${unitType}-totalAdditionalHeadcount`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>
            );
            grandtotal += local + expat;
        });

        const key = `unitType-${unitType}-totalAdditionalHeadcount-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const calculaTotalForecastHeadcountUnitType = (company, unitType) => {
        let local = 0;
        let expat = 0;
        requests.map(request => {
            if (unitType === request.unitName.slice(0, 2)) {
                request.details.map(detail => {
                    if (company === detail.companyName) {
                        local += detail.localApprovedHeadcount + detail.localAddition;
                        expat += detail.expatApprovedHeadcount + detail.expatAddition;
                    }
                });
            }
        });
        return { local, expat };
    };

    const renderTotalForecastHeadcountUnitType = unitType => {
        const rows = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalForecastHeadcountUnitType(company, unitType);
            const key = `unitType-${company}-${unitType}-totalForecastHeadcount`;
            rows.push(
                <td key={`${key}-local`}>{renderNumberOfHeadcount(local)}</td>,
                <td key={`${key}-expat`}>{renderNumberOfHeadcount(expat)}</td>,
                <td key={`${key}-total`}>{renderNumberOfHeadcount(local + expat)}</td>
            );
            grandtotal += local + expat;
        });

        const key = `unitType-${unitType}-totalForecastHeadcount-grandtotal`;
        rows.push(<td key={key}>{renderNumberOfHeadcount(grandtotal)}</td>);

        return rows;
    };

    const renderTotalHeadcountUnitType = unitType => {
        const totalHeadcountRow = [];
        totalHeadcountRow.push(renderTotalCurrentHeadcountUnitType(unitType));
        totalHeadcountRow.push(renderTotalApprovedForecastHeadcountUnitType(unitType));
        totalHeadcountRow.push(renderTotalAdditionalHeadcountUnitType(unitType));
        totalHeadcountRow.push(renderTotalForecastHeadcountUnitType(unitType));
        return (
            <tr className="total-unittype" key={`${unitType}-totalHeadcount`}>
                <td className="bold fixed-body fixed-body-total-unittype" colSpan={4}>Total Headcount ({unitType})</td>
                {totalHeadcountRow}
            </tr>
        );
    };

    const renderDeparment = request => {
        const deparments = getDepartment(request);
        let rowSpanUnit = 1;
        const departmentRow = [];

        deparments.map(deparment => {
            const { sectionRow, rowSpanDeparment } = renderSection(deparment, request);
            const totalHeadcountDepartmentRow = renderTotalHeadcountDepartment(request, deparment);
            rowSpanUnit += rowSpanDeparment;
            departmentRow.push(
                <tr key={request.unitName + deparment.departmentName}>
                    <td className="bold fixed-body fixed-body fixed-body-department" rowSpan={rowSpanDeparment}>
                        {deparment.departmentName}
                    </td>
                </tr>);
            departmentRow.push(sectionRow);
            departmentRow.push(totalHeadcountDepartmentRow);
        });
        return { departmentRow, rowSpanUnit };
    };

    const renderUnit = unitType => {
        let rowSpanUnitType = 1;
        const unitRow = [];
        requests.map(request => {
            if (unitType === request.unitName.slice(0, 2)) {
                const { departmentRow, rowSpanUnit } = renderDeparment(request);
                rowSpanUnitType += rowSpanUnit;
                unitRow.push(<tr key={request.unitName}>
                    <td className="bold unit fixed-body fixed-body-unit" rowSpan={rowSpanUnit}>
                        <a className="link-unitname" href={Site.resolveClientUrl(`ManpowerForecast/AnnualRequest/Detail/${request.requestId}`)}>{request.unitName}</a>
                    </td>
                </tr>);
                unitRow.push(departmentRow);
            }
        });
        return { unitRow, rowSpanUnitType };
    };

    const buildBody = () => {
        const row = [];
        unitTypes.map(unitType => {
            const { unitRow, rowSpanUnitType } = renderUnit(unitType);
            const totalHeadcountUnitType = renderTotalHeadcountUnitType(unitType);
            row.push(<tr key={unitType}><td className="bold unit fixed-body fixed-body-unittype" rowSpan={rowSpanUnitType}>{unitType}</td></tr>);
            row.push(unitRow);
            row.push(totalHeadcountUnitType);
        });
        return row;
    };

    const buildTableNoneData = () => {
        return <div data-testid='table-summary' className="table-summary">
            <table className="table">
                <thead>
                    <tr key="first-row-header">
                        <th className="name-step-background hide-border fixed-header fixed-header-unit">{LangHelper.getResource('Unit')}</th>
                        <th className="name-step-background hide-border fixed-header fixed-header-department">{LangHelper.getResource('Deparment')}</th>
                        <th className="name-step-background section hide-border fixed-header fixed-header-section">{LangHelper.getResource('Section')}</th>
                        <th className="name-step-background">
                            {(periodSelected.year - 1) + LangHelper.getResource('ManpowerHeadcountCurrent')}
                        </th>
                        <th className="name-step-background">
                            {(periodSelected.year - 1) + LangHelper.getResource('ApprovedForecastHeadcount')}
                        </th>
                        <th className="name-step-background">
                            {LangHelper.getResource('AdditionalHeadcount')}
                        </th>
                        <th className="name-step-background">
                            {periodSelected.year + LangHelper.getResource('ManpowerHeadcountForecast')}
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr key={'body'} className="no-data">
                        <td colSpan={7}>{LangHelper.getResource('ThereIsNoData')}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    };

    return (
        <>
            {
                periodSelected != null && requests.length > 0 &&
                <div data-testid='table-summary' className="table-summary">
                    <table className="table">
                        <thead>
                            {buildHeader()}
                        </thead>
                        <tbody>
                            {buildBody()}
                        </tbody>
                    </table>
                </div>
            }
            {
                periodSelected != null && requests.length === 0 && buildTableNoneData()
            }
        </>
    );
};

export default DataGrid;