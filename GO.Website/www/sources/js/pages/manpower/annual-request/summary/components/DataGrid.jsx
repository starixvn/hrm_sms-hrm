import React from 'react';

import { Cell, Row } from '@jsroot/components/react-grid/index';

import LangHelper from '@jsroot/common/langHelper';

const DataGrid = ({ requests, companies, unitTypes, periodSelected }) => {
    const maxStep = 4;
    const checkZero = number => number !== 0 ? number : '';
    const renderFirstRowHeader = () => {
        const year = periodSelected.year;
        return (
            <tr key="first-row-header">
                <th className="name-step-background hide-border" rowSpan={3} colSpan={2}>{LangHelper.getResource('Unit')}</th>
                <th className="name-step-background hide-border" rowSpan={3}>{LangHelper.getResource('Deparment')}</th>
                <th className="name-step-background section hide-border" rowSpan={3}>{LangHelper.getResource('Section')}</th>
                <th className="name-step-background" colSpan={3 * companies.length + 1}>{year + LangHelper.getResource(' Manpower Headcount (Current)')}</th>
                <th className="name-step-background" colSpan={3 * companies.length + 1}>{year + LangHelper.getResource(' Approved Forecast Headcount')}</th>
                <th className="name-step-background" colSpan={3 * companies.length + 1}>{LangHelper.getResource('Additional Headcount')}</th>
                <th className="name-step-background" colSpan={3 * companies.length + 1}>{(year + 1) + LangHelper.getResource(' Manpower Headcount (Forecast)')}</th>
            </tr>);
    }

    const renderSecondRowHeader = () => {
        const headerHeadcount = [];
        for (let step = 0; step < maxStep; step++) {
            companies.map((item, index) => {
                if ((index + 1) === companies.length) {
                    headerHeadcount.push(<th className="name-company-background" key={item + step} colSpan={3}>{item}</th>,
                        <th className="name-step-background grand-total hide-border" key={'Grand Total' + item + step} rowSpan={2}>{LangHelper.getResource('Grand Total')}</th>);
                }
                else {
                    headerHeadcount.push(<th className="name-company-background" key={item + step} colSpan={3}>{item}</th>);
                }
            });
        }
        return headerHeadcount;
    }

    const renderThirdRowHeader = () => {
        const headerDataHeadcount = [];
        for (let step = 0; step < maxStep; step++) {
            companies.map((item, index) => {
                headerDataHeadcount.push(<th className="name-company-background" key={item + step + 'Local'} >{LangHelper.getResource('Local')}</th>,
                    <th className="name-company-background" key={item + step + 'Expat'}>{LangHelper.getResource('Expat')}</th>,
                    <th className="name-company-background" key={item + step + 'Total'}>{LangHelper.getResource('Total')}</th>)
            });
        }
        return headerDataHeadcount;
    }

    const buildHeader = () => {
        const row = [];
        row.push(renderFirstRowHeader())
        row.push(<tr key="second-row-header">
            {renderSecondRowHeader()}
        </tr>);
        row.push(<tr key="third-row-header">
            {renderThirdRowHeader()}
        </tr>);
        return row;
    };

    const getSection = (request, detaildepartment)  => {
        const sections = [];
        request.details.map(detail => {
            if (detail.departmentName === detaildepartment.departmentName
                && sections.findIndex(section => section === detail.sectionName) === -1) {
                sections.push(detail.sectionName);
            }
        });
        return sections;
    }

    const getDepartment = request => {
        const departments = [];
        request.details.map(detail => {
            if (departments.findIndex(x => x.departmentName === detail.departmentName) === -1) {
                departments.push({
                    departmentName: detail.departmentName,
                    sections: getSection(request, detail)
                });
            }
        });
        return departments;
    };

    const renderCurrentHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let checkNone = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                    grandtotal += detail.totalCurrentHeadcount;
                    checkNone++;
                    row.push(
                        <td>{checkZero(detail.localCurrentHeadcount)}</td>,
                        <td>{checkZero(detail.expatCurrentHeadcount)}</td>,
                        <td>{checkZero(detail.totalCurrentHeadcount)}</td>);
                }
            });
            if (checkNone === 0) {
                row.push(
                    <td></td>,
                    <td></td>,
                    <td></td>);
            }
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderApprovedForecastHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let checkNone = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                    grandtotal += detail.totalApprovedHeadcount;
                    checkNone++;
                    row.push(<td>{checkZero(detail.localApprovedHeadcount)}</td>,
                        <td>{checkZero(detail.expatApprovedHeadcount)}</td>,
                        <td>{checkZero(detail.totalApprovedHeadcount)}</td>)
                }
            });
            if (checkNone === 0) {
                row.push(
                    <td></td>,
                    <td></td>,
                    <td></td>);
            }
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderAdditionalHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let checkNone = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                    grandtotal += detail.totalAddition;
                    checkNone++;
                    row.push(<td>{checkZero(detail.localAddition)}</td>,
                        <td>{checkZero(detail.expatAddition)}</td>,
                        <td>{checkZero(detail.totalAddition)}</td>)
                }
            });
            if (checkNone === 0) {
                row.push(
                    <td></td>,
                    <td></td>,
                    <td></td>);
            }
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderForecastHeadcount = (request, deparment, section) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let checkNone = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    detail.sectionName === section && company === detail.companyName) {
                    grandtotal += detail.totalApprovedHeadcount + detail.totalAddition;
                    checkNone++;
                    row.push(<td>{checkZero(detail.localApprovedHeadcount + detail.localAddition)}</td>,
                        <td>{checkZero(detail.expatApprovedHeadcount + detail.expatAddition)}</td>,
                        <td>{checkZero(detail.totalApprovedHeadcount + detail.totalAddition)}</td>)
                }
            });
            if (checkNone === 0) {
                row.push(
                    <td></td>,
                    <td></td>,
                    <td></td>);
            }
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
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
                    <td className="text-left">{section}</td>
                    {headcountRow}
                </tr>
            );
        });
        return {sectionRow, rowSpanDeparment};
    };

    const renderTotalCurrentHeadcountDepartment = (request, deparment) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    company === detail.companyName) {
                    local += detail.localCurrentHeadcount;
                    expat += detail.expatCurrentHeadcount;
                }
            });
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderTotalApprovedForecastHeadcountDepartment = (request, deparment) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    company === detail.companyName) {
                    local += detail.localApprovedHeadcount;
                    expat += detail.expatApprovedHeadcount;
                }
            });
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderTotalAdditionalHeadcountDepartment = (request, deparment) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    company === detail.companyName) {
                    local += detail.localAddition;
                    expat += detail.expatAddition;
                }
            });
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderTotalForecastHeadcountDepartment = (request, deparment) => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            let local = 0;
            let expat = 0;
            request.details.map(detail => {
                if (detail.unitName === request.unitName && detail.departmentName === deparment.departmentName &&
                    company === detail.companyName) {
                    local += detail.localApprovedHeadcount + detail.localAddition;
                    expat += detail.expatApprovedHeadcount + detail.expatAddition;
                }
            });
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderTotalHeadcountDepartment = (request, deparment) => {
        const totalHeadcountDepartmentRow = [];
        totalHeadcountDepartmentRow.push(renderTotalCurrentHeadcountDepartment(request, deparment));
        totalHeadcountDepartmentRow.push(renderTotalApprovedForecastHeadcountDepartment(request, deparment));
        totalHeadcountDepartmentRow.push(renderTotalAdditionalHeadcountDepartment(request, deparment));
        totalHeadcountDepartmentRow.push(renderTotalForecastHeadcountDepartment(request, deparment));
        return <tr className="total-department" key={'Total Headcount' + request.unitName + deparment.departmentName}>
            <td className="text-left">Total Headcount</td>
            {totalHeadcountDepartmentRow}
        </tr>;
    }

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
    }

    const renderTotalCurrentHeadcountUnitType = unitType => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalCurrentHeadcountUnitType(company, unitType);
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
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
    }

    const renderTotalApprovedForecastHeadcountUnitType = unitType => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalApprovedHeadcountUnitType(company, unitType);
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
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
    }

    const renderTotalAdditionalHeadcountUnitType = unitType => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalAdditionalHeadcountUnitType(company, unitType);
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
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
    }


    const renderTotalForecastHeadcountUnitType = unitType => {
        const row = [];
        let grandtotal = 0;
        companies.map(company => {
            const { local, expat } = calculaTotalForecastHeadcountUnitType(company, unitType);
            row.push(
                <td>{checkZero(local)}</td>,
                <td>{checkZero(expat)}</td>,
                <td>{checkZero(local + expat)}</td>);
            grandtotal += local + expat;
        });
        row.push(<td>{checkZero(grandtotal)}</td>)
        return row;
    };

    const renderTotalHeadcountUnitType = unitType => {
        const totalHeadcountRow = [];
        totalHeadcountRow.push(renderTotalCurrentHeadcountUnitType(unitType));
        totalHeadcountRow.push(renderTotalApprovedForecastHeadcountUnitType(unitType));
        totalHeadcountRow.push(renderTotalAdditionalHeadcountUnitType(unitType));
        totalHeadcountRow.push(renderTotalForecastHeadcountUnitType(unitType));
        return <tr className="total-unittype" key={'Total Headcount ' + unitType}>
            <td className="bold" colSpan={4}>Total Headcount ( {unitType} )</td>
            {totalHeadcountRow}
        </tr>;
    }

    const renderDeparment = request => {
        const deparments = getDepartment(request);
        let rowSpanUnit = 1;
        const departmentRow = [];

        deparments.map(deparment => {
            const { sectionRow, rowSpanDeparment } = renderSection(deparment, request)
            const totalHeadcountDepartmentRow = renderTotalHeadcountDepartment(request, deparment);
            rowSpanUnit += rowSpanDeparment;
            departmentRow.push(<tr key={request.unitName + deparment.departmentName}><td className="bold" rowSpan={rowSpanDeparment}>{deparment.departmentName}</td></tr>);
            departmentRow.push(sectionRow);
            departmentRow.push(totalHeadcountDepartmentRow);
        });
        return {departmentRow, rowSpanUnit}; 
    };

    const renderUnit = unitType =>{
        let rowSpanUnitType = 1;
        const unitRow = [];
        requests.map(request => {
            if (unitType === request.unitName.slice(0, 2)) {
                const {departmentRow, rowSpanUnit} = renderDeparment(request);
                rowSpanUnitType += rowSpanUnit;
                unitRow.push(<tr key={request.unitName}><td className="bold unit" rowSpan={rowSpanUnit}>{request.unitName}</td></tr>);
                unitRow.push(departmentRow);
            }               
        });
    return {unitRow, rowSpanUnitType};
    };

    const buildBody = () => {
        const row = [];
        unitTypes.map(unitType => {
            const {unitRow, rowSpanUnitType} = renderUnit(unitType);
            const totalHeadcountUnitType = renderTotalHeadcountUnitType(unitType);
            row.push(<tr key={unitType}><td className="bold unit" rowSpan={rowSpanUnitType}>{unitType}</td></tr>);
            row.push(unitRow);
            row.push(totalHeadcountUnitType);
        });
        return row;
    };

    return (
        <>
            {
                requests.length > 0 &&
                <div className="table-summary">
                    <table className="table">
                        <thead>
                            {buildHeader()}
                        </thead>
                        <tbody>{buildBody()}</tbody>
                    </table>
                </div>
            }
        </>
    );
};

export default DataGrid;