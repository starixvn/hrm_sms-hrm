import React from 'react';

import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import { renderNumberOfHeadcount } from '../../../common.funcs';
import { isEqual } from '@jsroot/common/string.funcs';

const DataGrid = ({ requests, companies, unitTypes, periodSelected }) => {
    if (companies.length === 0) {
        companies = [''];
    }

    const year = periodSelected ? periodSelected.year : 0;
    const bodyRows = [];
    const totalDeptHeadcounts = [];

    const buildHeader = () => {
        const rows = [];

        rows.push(
            <tr key="first-row-header">
                <th className="name-step-background hide-border fixed-header fixed-header-unit" rowSpan={3} colSpan={2}>{LangHelper.getResource('Unit')}</th>
                <th className="name-step-background hide-border fixed-header fixed-header-department" rowSpan={3}>{LangHelper.getResource('Deparment')}</th>
                <th className="name-step-background section hide-border fixed-header fixed-header-section" rowSpan={3}>{LangHelper.getResource('Section')}</th>
                {
                    companies.map(company => <th key={company} className="name-step-background" style={{ width: 1000 }} colSpan={12}>{company}</th>)
                }
            </tr>
        );

        rows.push(
            <tr key="second-row-header">
                {
                    companies.map(company => {
                        const cols = [];
                        cols.push(<th className="name-company-background" key={`${company}-local`} colSpan={4}>{LangHelper.getResource('Local')}</th>);
                        cols.push(<th className="name-company-background" key={`${company}-expat`} colSpan={4}>{LangHelper.getResource('Expat')}</th>);
                        cols.push(<th className="name-step-background" key={`${company}-total`} colSpan={4}>{LangHelper.getResource('Total')}</th>);
                        return cols;
                    })
                }
            </tr>
        );

        rows.push(
            <tr key="third-row-header">
                {
                    companies.map(company => {
                        const cols = [];
                        cols.push(<th className="name-company-background" key={`${company}-local-${year - 1}-current`}>{`${year - 1} (Current)`}</th>);
                        cols.push(<th className="name-company-background" key={`${company}-local-${year - 1}-forecast`}>{`${year - 1} Forecast`}</th>);
                        cols.push(<th className="name-company-background" key={`${company}-local-${year}-additional`}>{`${year} Additional`}</th>);
                        cols.push(<th className="name-company-background" key={`${company}-local-${year}-forecast`}>{`${year} (Forecast)`}</th>);

                        cols.push(<th className="name-company-background" key={`${company}-expat-${year - 1}-current`}>{`${year - 1} (Current)`}</th>);
                        cols.push(<th className="name-company-background" key={`${company}-expat-${year - 1}-forecast`}>{`${year - 1} Forecast`}</th>);
                        cols.push(<th className="name-company-background" key={`${company}-expat-${year}-additional`}>{`${year} Additional`}</th>);
                        cols.push(<th className="name-company-background" key={`${company}-expat-${year}-forecast`}>{`${year} (Forecast)`}</th>);

                        cols.push(<th className="name-step-background" key={`${company}-total-${year - 1}-current`}>{`${year - 1} (Current)`}</th>);
                        cols.push(<th className="name-step-background" key={`${company}-total-${year - 1}-forecast`}>{`${year - 1} Forecast`}</th>);
                        cols.push(<th className="name-step-background" key={`${company}-total-${year}-additional`}>{`${year} Additional`}</th>);
                        cols.push(<th className="name-step-background" key={`${company}-total-${year}-forecast`}>{`${year} (Forecast)`}</th>);

                        return cols;
                    })
                }
            </tr>
        );

        return rows;
    };

    const getSection = (request, detaildepartment) => {
        const sections = [];
        request.details.map(detail => {
            if (isEqual(detail.departmentName, detaildepartment.departmentName)
                && sections.findIndex(section => isEqual(section, detail.sectionName)) === -1) {
                sections.push(detail.sectionName);
            }
        });
        return sections;
    };

    const getDepartment = request => {
        const departments = [];
        request.details.map(detail => {
            if (departments.findIndex(x => isEqual(x.departmentName, detail.departmentName)) === -1) {
                departments.push({
                    departmentName: detail.departmentName,
                    sections: getSection(request, detail)
                });
            }
        });
        return departments;
    };

    const buildBody = () => {
        unitTypes.map(unitType => {
            const { unitRow, rowSpanUnitType } = renderUnit(unitType);
            const totalHeadcountUnitType = renderTotalHeadcountUnitType(unitType);
            bodyRows.push(<tr key={unitType}><td className="bold unit fixed-body fixed-body-unittype" rowSpan={rowSpanUnitType}>{unitType}</td></tr>);
            bodyRows.push(unitRow);
            bodyRows.push(totalHeadcountUnitType);
        });
        bodyRows.unshift(renderGrandTotal());

        return bodyRows;
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
                        {
                            request.requestId > 0 &&
                            <a className="link-unitname" href={Site.resolveClientUrl(`ManpowerForecast/AnnualRequest/Detail/${request.requestId}`)}>{request.unitName}</a>
                        }
                        {
                            request.requestId <= 0 &&
                            <a>{request.unitName}</a>
                        }
                    </td>
                </tr>);
                unitRow.push(departmentRow);
            }
        });
        return { unitRow, rowSpanUnitType };
    };

    const renderDeparment = request => {
        const departments = getDepartment(request);
        let rowSpanUnit = 1;
        const departmentRow = [];

        departments.map(department => {
            const { sectionRow, rowSpanDeparment } = renderSection(department, request);
            const totalHeadcountDepartmentRow = renderTotalHeadcountDepartment(request, department);
            rowSpanUnit += rowSpanDeparment;
            departmentRow.push(
                <tr key={request.unitName + department.departmentName}>
                    <td className="bold fixed-body fixed-body fixed-body-department" rowSpan={rowSpanDeparment}>
                        {department.departmentName}
                    </td>
                </tr>);
            departmentRow.push(sectionRow);
            departmentRow.push(totalHeadcountDepartmentRow);
        });
        return { departmentRow, rowSpanUnit };
    };

    const renderSection = (department, request) => {
        const sectionRow = [];
        let rowSpanDeparment = 2;
        const year = periodSelected.year;
        const unitType = request.unitName.slice(0, 2);
        department.sections.map(section => {
            rowSpanDeparment++;
            const headcountRow = [];

            companies.map(company => {
                const key = `${unitType}-${request.unitName}-${department.departmentName}-${section}-${company}`;
                const detail = request.details.find(detail => detail.unitName === request.unitName
                    && isEqual(detail.departmentName, department.departmentName)
                    && isEqual(detail.sectionName, section) && company === detail.companyName);

                if (detail) {
                    headcountRow.push(<td key={`${key}-local-${year - 1}-current`}>{renderNumberOfHeadcount(detail.localCurrentHeadcount)}</td>);
                    headcountRow.push(<td key={`${key}-local-${year - 1}-forecast`}>{renderNumberOfHeadcount(detail.localApprovedHeadcount)}</td>);
                    headcountRow.push(<td key={`${key}-local-${year}-additional`}>{renderNumberOfHeadcount(detail.localAddition)}</td>);
                    headcountRow.push(<td key={`${key}-local-${year}-forecast`}>{renderNumberOfHeadcount(detail.localCurrentHeadcount + detail.localAddition)}</td>);

                    headcountRow.push(<td key={`${key}-expat-${year - 1}-current`}>{renderNumberOfHeadcount(detail.expatCurrentHeadcount)}</td>);
                    headcountRow.push(<td key={`${key}-expat-${year - 1}-forecast`}>{renderNumberOfHeadcount(detail.expatApprovedHeadcount)}</td>);
                    headcountRow.push(<td key={`${key}-expat-${year}-additional`}>{renderNumberOfHeadcount(detail.expatAddition)}</td>);
                    headcountRow.push(<td key={`${key}-expat-${year}-forecast`}>{renderNumberOfHeadcount(detail.expatCurrentHeadcount + detail.expatAddition)}</td>);

                    headcountRow.push(<td key={`${key}-total-${year - 1}-current`}>{renderNumberOfHeadcount(detail.totalCurrentHeadcount)}</td>);
                    headcountRow.push(<td key={`${key}-total-${year - 1}-forecast`}>{renderNumberOfHeadcount(detail.totalApprovedHeadcount)}</td>);
                    headcountRow.push(<td key={`${key}-total-${year}-additional`}>{renderNumberOfHeadcount(detail.totalAddition)}</td>);
                    headcountRow.push(<td key={`${key}-total-${year}-forecast`}>{renderNumberOfHeadcount(detail.totalCurrentHeadcount + detail.totalAddition)}</td>);
                } else {
                    headcountRow.push(<td key={`${key}-local-${year - 1}-current`}></td>);
                    headcountRow.push(<td key={`${key}-local-${year - 1}-forecast`}></td>);
                    headcountRow.push(<td key={`${key}-local-${year}-additional`}></td>);
                    headcountRow.push(<td key={`${key}-local-${year}-forecast`}></td>);

                    headcountRow.push(<td key={`${key}-expat-${year - 1}-current`}></td>);
                    headcountRow.push(<td key={`${key}-expat-${year - 1}-forecast`}></td>);
                    headcountRow.push(<td key={`${key}-expat-${year}-additional`}></td>);
                    headcountRow.push(<td key={`${key}-expat-${year}-forecast`}></td>);

                    headcountRow.push(<td key={`${key}-total-${year - 1}-current`}></td>);
                    headcountRow.push(<td key={`${key}-total-${year - 1}-forecast`}></td>);
                    headcountRow.push(<td key={`${key}-total-${year}-additional`}></td>);
                    headcountRow.push(<td key={`${key}-total-${year}-forecast`}></td>);
                }
            });

            sectionRow.push(
                <tr className="highlight-row" key={request.unitName + department.departmentName + section}>
                    <td className="text-left fixed-body fixed-body-section">{section}</td>
                    {headcountRow}
                </tr>
            );
        });
        return { sectionRow, rowSpanDeparment };
    };

    const calculateSum = (array, property) => {
        let sum = 0;
        const totalItems = array.length;
        for (let index = 0; index < totalItems; index++) {
            sum += array[index][property];
        }

        return sum;
    };

    const renderTotalHeadcountDepartment = (request, department) => {
        const cols = [];
        companies.map(company => {
            const key = `${request.unitName}-${department.departmentName}-totalHeadcount-${company}`;
            const details = request.details.filter(detail => detail.unitName === request.unitName
                && isEqual(detail.departmentName, department.departmentName)
                && company === detail.companyName);

            const totalLocalCurrentHeadcount = calculateSum(details, 'localCurrentHeadcount');
            const totalLocalApprovedHeadcount = calculateSum(details, 'localApprovedHeadcount');
            const totalLocalAddition = calculateSum(details, 'localAddition');

            cols.push(<td key={`${key}-local-${year - 1}-current`}>{renderNumberOfHeadcount(totalLocalCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-local-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalLocalApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-local-${year}-additional`}>{renderNumberOfHeadcount(totalLocalAddition)}</td>);
            cols.push(<td key={`${key}-local-${year}-forecast`}>{renderNumberOfHeadcount(totalLocalCurrentHeadcount + totalLocalAddition)}</td>);

            const totalExpatCurrentHeadcount = calculateSum(details, 'expatCurrentHeadcount');
            const totalExpatApprovedHeadcount = calculateSum(details, 'expatApprovedHeadcount');
            const totalExpatAddition = calculateSum(details, 'expatAddition');

            cols.push(<td key={`${key}-expat-${year - 1}-current`}>{renderNumberOfHeadcount(totalExpatCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-expat-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalExpatApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-expat-${year}-additional`}>{renderNumberOfHeadcount(totalExpatAddition)}</td>);
            cols.push(<td key={`${key}-expat-${year}-forecast`}>{renderNumberOfHeadcount(totalExpatCurrentHeadcount + totalExpatAddition)}</td>);

            cols.push(<td key={`${key}-total-${year - 1}-current`}>{renderNumberOfHeadcount(totalLocalCurrentHeadcount + totalExpatCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-total-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalLocalApprovedHeadcount + totalExpatApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-total-${year}-additional`}>{renderNumberOfHeadcount(totalLocalAddition + totalExpatAddition)}</td>);
            cols.push(<td key={`${key}-total-${year}-forecast`}>
                {renderNumberOfHeadcount(totalLocalCurrentHeadcount + totalExpatCurrentHeadcount + totalLocalAddition + totalExpatAddition)}
            </td>);

            if (totalDeptHeadcounts.indexOf(item => item.unitName === request.unitName
                && isEqual(item.departmentName, department.departmentName)
                && company === item.companyName) === -1) {
                totalDeptHeadcounts.push({
                    unitName: request.unitName,
                    departmentName: department.departmentName,
                    companyName: company,
                    localCurrentHeadcount: totalLocalCurrentHeadcount,
                    localApprovedHeadcount: totalLocalApprovedHeadcount,
                    localAddition: totalLocalAddition,
                    localForecastHeadcount: totalLocalCurrentHeadcount + totalLocalAddition,
                    expatCurrentHeadcount: totalExpatCurrentHeadcount,
                    expatApprovedHeadcount: totalExpatApprovedHeadcount,
                    expatAddition: totalExpatAddition,
                    expatForecastHeadcount: totalExpatCurrentHeadcount + totalExpatAddition
                });
            }
        });

        return (
            <tr className="total-department" key={`${request.unitName}-${department.departmentName}-totalHeadcount`}>
                <td className="text-left fixed-body fixed-body-total-department">Total Headcount</td>
                {cols}
            </tr>
        );
    };

    const renderTotalHeadcountUnitType = unitType => {
        const cols = [];
        companies.map(company => {
            const key = `${unitType}-totalHeadcount-${company}`;
            const units = totalDeptHeadcounts.filter(item => unitType === item.unitName.slice(0, 2) && item.companyName === company);

            const totalLocalCurrentHeadcount = calculateSum(units, 'localCurrentHeadcount');
            const totalLocalApprovedHeadcount = calculateSum(units, 'localApprovedHeadcount');
            const totalLocalAddition = calculateSum(units, 'localAddition');
            const totalLocalForecastHeadcount = calculateSum(units, 'localForecastHeadcount');

            cols.push(<td key={`${key}-local-${year - 1}-current`}>{renderNumberOfHeadcount(totalLocalCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-local-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalLocalApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-local-${year}-additional`}>{renderNumberOfHeadcount(totalLocalAddition)}</td>);
            cols.push(<td key={`${key}-local-${year}-forecast`}>{renderNumberOfHeadcount(totalLocalForecastHeadcount)}</td>);

            const totalExpatCurrentHeadcount = calculateSum(units, 'expatCurrentHeadcount');
            const totalExpatApprovedHeadcount = calculateSum(units, 'expatApprovedHeadcount');
            const totalExpatAddition = calculateSum(units, 'expatAddition');
            const totalExpatForecastHeadcount = calculateSum(units, 'expatForecastHeadcount');

            cols.push(<td key={`${key}-expat-${year - 1}-current`}>{renderNumberOfHeadcount(totalExpatCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-expat-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalExpatApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-expat-${year}-additional`}>{renderNumberOfHeadcount(totalExpatAddition)}</td>);
            cols.push(<td key={`${key}-expat-${year}-forecast`}>{renderNumberOfHeadcount(totalExpatForecastHeadcount)}</td>);

            cols.push(<td key={`${key}-total-${year - 1}-current`}>{renderNumberOfHeadcount(totalLocalCurrentHeadcount + totalExpatCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-total-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalLocalApprovedHeadcount + totalExpatApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-total-${year}-additional`}>{renderNumberOfHeadcount(totalLocalAddition + totalExpatAddition)}</td>);
            cols.push(<td key={`${key}-total-${year}-forecast`}>
                {renderNumberOfHeadcount(totalLocalForecastHeadcount + totalExpatForecastHeadcount)}
            </td>);
        });

        return (
            <tr className="total-unittype" key={`${unitType}-totalHeadcount`}>
                <td className="bold fixed-body fixed-body-total-unittype" colSpan={4}>Total Headcount ({unitType})</td>
                {cols}
            </tr>
        );
    };

    const renderGrandTotal = () => {
        const cols = [];
        companies.map(company => {
            const key = `grand-total-${company}`;
            const items = totalDeptHeadcounts.filter(item => item.companyName === company);

            const totalLocalCurrentHeadcount = calculateSum(items, 'localCurrentHeadcount');
            const totalLocalApprovedHeadcount = calculateSum(items, 'localApprovedHeadcount');
            const totalLocalAddition = calculateSum(items, 'localAddition');
            const totalLocalForecastHeadcount = calculateSum(items, 'localForecastHeadcount');

            cols.push(<td key={`${key}-local-${year - 1}-current`}>{renderNumberOfHeadcount(totalLocalCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-local-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalLocalApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-local-${year}-additional`}>{renderNumberOfHeadcount(totalLocalAddition)}</td>);
            cols.push(<td key={`${key}-local-${year}-forecast`}>{renderNumberOfHeadcount(totalLocalForecastHeadcount)}</td>);

            const totalExpatCurrentHeadcount = calculateSum(items, 'expatCurrentHeadcount');
            const totalExpatApprovedHeadcount = calculateSum(items, 'expatApprovedHeadcount');
            const totalExpatAddition = calculateSum(items, 'expatAddition');
            const totalExpatForecastHeadcount = calculateSum(items, 'expatForecastHeadcount');

            cols.push(<td key={`${key}-expat-${year - 1}-current`}>{renderNumberOfHeadcount(totalExpatCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-expat-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalExpatApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-expat-${year}-additional`}>{renderNumberOfHeadcount(totalExpatAddition)}</td>);
            cols.push(<td key={`${key}-expat-${year}-forecast`}>{renderNumberOfHeadcount(totalExpatForecastHeadcount)}</td>);

            cols.push(<td key={`${key}-total-${year - 1}-current`}>{renderNumberOfHeadcount(totalLocalCurrentHeadcount + totalExpatCurrentHeadcount)}</td>);
            cols.push(<td key={`${key}-total-${year - 1}-forecast`}>{renderNumberOfHeadcount(totalLocalApprovedHeadcount + totalExpatApprovedHeadcount)}</td>);
            cols.push(<td key={`${key}-total-${year}-additional`}>{renderNumberOfHeadcount(totalLocalAddition + totalExpatAddition)}</td>);
            cols.push(<td key={`${key}-total-${year}-forecast`}>
                {renderNumberOfHeadcount(totalLocalForecastHeadcount + totalExpatForecastHeadcount)}
            </td>);
        });
        return (
            <tr className="total-unittype" key="grand-total">
                <td className="bold fixed-body fixed-body-total-unittype" colSpan={4}>Grand Total ({unitTypes.join('+')})</td>
                {cols}
            </tr>
        );
    };

    if (periodSelected !== null) {
        return (
            <div data-testid='table-summary' className="table-summary member-summary">
                <table className="table">
                    <thead>
                        {buildHeader()}
                    </thead>
                    <tbody>
                        {
                            requests.length === 0 &&
                            <tr key={'body'} className="no-data">
                                <td colSpan={16}>{LangHelper.getResource('ThereIsNoData')}</td>
                            </tr>
                        }
                        {requests.length > 0 && buildBody()}
                    </tbody>
                </table>
            </div>
        );
    } else {
        return <></>;
    }
};

export default DataGrid;