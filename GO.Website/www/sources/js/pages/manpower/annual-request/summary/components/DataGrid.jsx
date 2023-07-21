import React from 'react';

import { Cell, Row } from '@jsroot/components/react-grid/index';

import LangHelper from '@jsroot/common/langHelper';

const DataGrid = ({ requests, companies, unitTypes, periodSelected }) => {
    const maxStep = 4;
    const renderFirstHeader = () => {
        const year = periodSelected.year;
        return (
            <tr>
                <th colWidth={3} rowSpan={3} colSpan={2}>{LangHelper.getResource('Unit')}</th>
                <th colWidth={3} rowSpan={3}>{LangHelper.getResource('Deparment')}</th>
                <th colWidth={3} rowSpan={3}>{LangHelper.getResource('Section')}</th>
                <th colWidth={3} colSpan={3 * companies.length + 1}>{year + LangHelper.getResource(' Manpower Headcount (Current)')}</th>
                <th colWidth={3} colSpan={3 * companies.length + 1}>{year + LangHelper.getResource(' Approved Forecast Headcount')}</th>
                <th colWidth={3} colSpan={3 * companies.length + 1}>{LangHelper.getResource('Additional Headcount')}</th>
                <th colWidth={3} colSpan={3 * companies.length + 1}>{(year + 1) + LangHelper.getResource(' Manpower Headcount (Forecast)')}</th>
            </tr>);
    }

    const renderSecondHeader = () => {
        const headerHeadcount = [];
        for (let step = 0; step < maxStep; step++) {
            companies.map((item, index) => {
                if ((index + 1) === companies.length) {
                    headerHeadcount.push(<th colWidth={3} colSpan={3}>{item}</th>,
                        <th colWidth={3} rowSpan={2}>{LangHelper.getResource('Grand Total')}</th>);
                }
                else {
                    headerHeadcount.push(<th colWidth={3} colSpan={3}>{item}</th>);
                }
            });
        }
        return headerHeadcount;
    }

    const renderThirdHeader = () => {
        const headerDataHeadcount = [];
        for (let step = 0; step < maxStep; step++) {
            companies.map((item, index) => {
                headerDataHeadcount.push(<th colWidth={3}>{LangHelper.getResource('Local')}</th>,
                    <th colWidth={3}>{LangHelper.getResource('Expat')}</th>,
                    <th colWidth={3}>{LangHelper.getResource('Total')}</th>)
            });
        }
        return headerDataHeadcount;
    }

    const buildHeader = () => {
        const row = [];
        row.push(renderFirstHeader())
        row.push(<tr>
            {renderSecondHeader()}
        </tr>);
        row.push(<tr>
            {renderThirdHeader()}
        </tr>);
        return row;
    };

    const getDepartment = request => {
        const departments = [];
        request.details.map(item => {
            if (departments.findIndex(x => x.departmentName === item.departmentName) === -1) {
                const sections = [];
                request.details.map(detail => {
                    if (detail.departmentName === item.departmentName
                        && sections.findIndex(section => section === detail.sectionName) === -1) {
                        sections.push(detail.sectionName);
                    }
                });
                departments.push({
                    departmentName: item.departmentName,
                    sections: sections
                });
            }
        });
        return departments;
    };

    const renderCurrentHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                    grandtotal += detail.totalCurrentHeadcount;
                    row.push(
                        <td>{detail.localCurrentHeadcount}</td>,
                        <td>{detail.expatCurrentHeadcount}</td>,
                        <td>{detail.totalCurrentHeadcount}</td>)
                }
            });
        });
        row.push(<td>{grandtotal}</td>)
        return row;
    };

    const renderApprovedForecastHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                        grandtotal += detail.totalApprovedHeadcount;
                        row.push(<td>{detail.localApprovedHeadcount}</td>,
                            <td>{detail.expatApprovedHeadcount}</td>,
                            <td>{detail.totalApprovedHeadcount}</td>)
                }
            });
        });
        row.push(<td>{grandtotal}</td>)
        return row;
    };

    const renderAdditionalHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                        grandtotal += detail.totalAddition;
                        row.push(<td>{detail.localAddition}</td>,
                            <td>{detail.expatAddition}</td>,
                            <td>{detail.totalAddition}</td>)
                }
            });
        });
        row.push(<td>{grandtotal}</td>)
        return row;
    };

    const renderForecastHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                        grandtotal += detail.totalApprovedHeadcount + detail.totalAddition;
                        row.push(<td>{detail.localApprovedHeadcount + detail.localAddition}</td>,
                            <td>{detail.expatApprovedHeadcount + detail.expatAddition}</td>,
                            <td>{detail.totalApprovedHeadcount + detail.totalAddition}</td>)
                }
            });
        });
        row.push(<td>{grandtotal}</td>)
        return row;
    };

    const renderSection = (deparment, request) => {
        const sectionRow = [];
        let rowSpanDeparment = 1;
        deparment.sections.map(section => {
            rowSpanDeparment++;
            const headcountRow = [];
            headcountRow.push(renderCurrentHeadcount(request, deparment, section));
            headcountRow.push(renderApprovedForecastHeadcount(request, deparment, section));
            headcountRow.push(renderAdditionalHeadcount(request, deparment, section));
            headcountRow.push(renderForecastHeadcount(request, deparment, section));
            sectionRow.push(
                <tr>
                    <td>{section}</td>
                    {headcountRow}
                </tr>
            );
        });
        return {sectionRow, rowSpanDeparment};
    };

    const renderDeparment = request => {
        const deparments = getDepartment(request);
        let rowSpanUnit = 3;
        const departmentRow = [];

        deparments.map(deparment => {
            const { sectionRow, rowSpanDeparment } = renderSection(deparment, request)
            rowSpanUnit += (rowSpanDeparment - 1) ;
            departmentRow.push(<tr><td rowSpan={rowSpanDeparment}>{deparment.departmentName}</td></tr>);
            departmentRow.push(sectionRow);
        });
        return {departmentRow, rowSpanUnit}; 
    };

    const renderUnit = unitType =>{
        let rowSpanUnitType = 5;
        const unitRow = [];
        requests.map(request => {
            if (unitType === request.unitName.slice(0, 2)) {
                const {departmentRow, rowSpanUnit} = renderDeparment(request);
                rowSpanUnitType += rowSpanUnit;
                unitRow.push(<tr><td rowSpan={rowSpanUnit}>{request.unitName}</td></tr>);
                unitRow.push(departmentRow);
            }               
        });
    return {unitRow, rowSpanUnitType};
    };

    const buildBody = () => {
        const row = [];
        unitTypes.map(unitType => {
            const {unitRow, rowSpanUnitType} = renderUnit(unitType);
            row.push(<tr><td rowSpan={rowSpanUnitType}>{unitType}</td></tr>);
            row.push(unitRow);
        });
        return row;
    };

    return (
        <>
            {
                requests.length > 0 &&
                <table className="table">
                    <thead>
                        {buildHeader()}
                    </thead>
                    <tbody>{buildBody()}</tbody>
                </table>
            }
        </>
    );
};

export default DataGrid;