import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';
import RowItem from './RowItem';
import { Equal } from '@jsroot/common/string.funcs';
import CompanyTotalGrid from './CompanyTotalGrid';

const CompanyGrid = ({ isDetail, period, departments, companies, requestDetails, unitSelected }) => {
    const renderSectionNoneData = (deptItem, sentionName) => {
        const jobLevelRows = [];
        requestDetails.map(item => {
            if (item.unitName === unitSelected.value && Equal(item.departmentName, deptItem.departmentName) && Equal(item.sectionName, sentionName)) {
                jobLevelRows.push(<RowItem key={`${unitSelected.value}-${deptItem.departmentName}-${sentionName}-${item.jobLevel}`}
                        isRenderData={false} isRenderTotal={false}/>);
            }
        });
        return jobLevelRows;
    };

    const renderDepartmentNoneData = deptItem => {
        const sectionRows = [];
        deptItem.sections.map(sentionName => {
            const jobLevelRows = renderSectionNoneData(deptItem, sentionName);
            sectionRows.push(jobLevelRows);
            sectionRows.push(<RowItem key={`col-total-section-${unitSelected.value}-${deptItem.departmentName}-${sentionName}`}
                    isRenderData={false} isRenderTotal={false}/>);
        });
        return sectionRows;
    };

    const renderUnitNoneData = () => {
        const rows = [];
        departments.map(deptItem => {
            const sectionRows = renderDepartmentNoneData(deptItem);
            rows.push(sectionRows);
            rows.push(<RowItem key={`col-total-dept-${unitSelected.value}-${deptItem.departmentName}`} isRenderData={false} isRenderTotal={false}/>);
        });
        return rows;
    };

    const renderJoblevel = (deptItem, sentionName, companyName) => {
        const jobLevelRows = [];
        let totalSection = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0,
            expatAddition: 0, localAddition: 0, totalAddition: 0,
            expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        requestDetails.map(item => {
            if (item.unitName === unitSelected.value && Equal(item.departmentName, deptItem.departmentName) &&
                Equal(item.sectionName, sentionName) && item.companyName === companyName) {
                jobLevelRows.push(
                    <RowItem key={`${unitSelected.value}-${deptItem.departmentName}-${sentionName}-${item.jobLevel}`}
                        isDetail={isDetail} itemDetail={item} isRenderData={true} isRenderTotal={false} />
                );

                totalSection = {
                    expatCurrentHeadcount: totalSection.expatCurrentHeadcount + item.expatCurrentHeadcount,
                    localCurrentHeadcount: totalSection.localCurrentHeadcount + item.localCurrentHeadcount,
                    totalCurrentHeadcount: totalSection.totalCurrentHeadcount + item.totalCurrentHeadcount,
                    expatApprovedHeadcount: totalSection.expatApprovedHeadcount + item.expatApprovedHeadcount,
                    localApprovedHeadcount: totalSection.localApprovedHeadcount + item.localApprovedHeadcount,
                    totalApprovedHeadcount: totalSection.totalApprovedHeadcount + item.totalApprovedHeadcount,
                    expatAddition: totalSection.expatAddition + item.expatAddition,
                    localAddition: totalSection.localAddition + item.localAddition,
                    totalAddition: totalSection.totalAddition + item.totalAddition,
                    expatForecastHeadcount: totalSection.expatForecastHeadcount + item.expatAddition + item.expatApprovedHeadcount,
                    localForecastHeadcount: totalSection.localForecastHeadcount + item.localAddition + item.localApprovedHeadcount,
                    totalForecastHeadcount: totalSection.totalForecastHeadcount + item.expatAddition + item.expatApprovedHeadcount
                        + item.localAddition + item.localApprovedHeadcount
                };
            }
        });
        return { totalSection, jobLevelRows };
    };

    const renderSection = (deptItem, companyName) => {
        const sectionRows = [];
        let totalDepartment = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0, expatAddition: 0,
            localAddition: 0, totalAddition: 0, expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        deptItem.sections.map(sentionName => {
            if (isExistSection(companyName, sentionName)) {
                const { totalSection, jobLevelRows } = renderJoblevel(deptItem, sentionName, companyName);
                totalDepartment = {
                    expatCurrentHeadcount: totalDepartment.expatCurrentHeadcount + totalSection.expatCurrentHeadcount,
                    localCurrentHeadcount: totalDepartment.localCurrentHeadcount + totalSection.localCurrentHeadcount,
                    totalCurrentHeadcount: totalDepartment.totalCurrentHeadcount + totalSection.totalCurrentHeadcount,
                    expatApprovedHeadcount: totalDepartment.expatApprovedHeadcount + totalSection.expatApprovedHeadcount,
                    localApprovedHeadcount: totalDepartment.localApprovedHeadcount + totalSection.localApprovedHeadcount,
                    totalApprovedHeadcount: totalDepartment.totalApprovedHeadcount + totalSection.totalApprovedHeadcount,
                    expatAddition: totalDepartment.expatAddition + totalSection.expatAddition,
                    localAddition: totalDepartment.localAddition + totalSection.localAddition,
                    totalAddition: totalDepartment.totalAddition + totalSection.totalAddition,
                    expatForecastHeadcount: totalDepartment.expatForecastHeadcount + totalSection.expatForecastHeadcount,
                    localForecastHeadcount: totalDepartment.localForecastHeadcount + totalSection.localForecastHeadcount,
                    totalForecastHeadcount: totalDepartment.totalForecastHeadcount + totalSection.totalForecastHeadcount
                };

                sectionRows.push(jobLevelRows);
                sectionRows.push(<RowItem key={`col-total-section-${unitSelected.value}-${deptItem.departmentName}-${sentionName}`}
                        total={totalSection} isRenderData={false} isRenderTotal={true}/> );
            } else {
                sectionRows.push(renderSectionNoneData(deptItem, sentionName));
                sectionRows.push(<RowItem key={`col-total-section-${unitSelected.value}-${deptItem.departmentName}-${sentionName}`}
                        isRenderData={false} isRenderTotal={false}/>);
            }
        });
        return { totalDepartment, sectionRows };
    };

    const renderDepartment = companyName => {
        const rows = [];
        departments.map(deptItem => {
            if (isExistDepartment(companyName, deptItem)) {
                const { totalDepartment, sectionRows } = renderSection(deptItem, companyName);
                rows.push(sectionRows);
                rows.push(<RowItem key={`col-total-dept-${unitSelected.value}-${deptItem.departmentName}`}
                    total={totalDepartment} isRenderData={false} isRenderTotal={true} />);
            } else {
                rows.push(renderDepartmentNoneData(deptItem));
                rows.push(<RowItem key={`col-total-dept-${unitSelected.value}-${deptItem.departmentName}`}
                    isRenderData={false} isRenderTotal={false} />);
            }
        });
        return rows;
    };

    const isExistCompany = companyName => requestDetails.find(x => x.unitName === unitSelected.value && x.companyName === companyName) != null;

    const isExistDepartment = (companyName, deptItem) => requestDetails.find(x => Equal(x.departmentName, deptItem.departmentName) && x.companyName === companyName) != null;

    const isExistSection = (companyName, sentionName) => requestDetails.find(x => Equal(x.sectionName, sentionName) && x.companyName === companyName) != null;

    const renderBodyTable = companyName => {
        const rows = [];
        if (isExistCompany(companyName)) {
            rows.push(renderDepartment(companyName));
        } else {
            rows.push(renderUnitNoneData());
        }

        return rows;
    };

    const renderTable = () => {
        const tables = [];
        companies.map(companyName => {
            tables.push(
                <table key={`table-company-${companyName}`} className={`table table-bordered table-company company-${companyName}`}>
                    <thead>
                        <tr><th colSpan="12" className="name">{companyName}</th></tr>
                        <tr>
                            <th colSpan="3" className="highlight">{LangHelper.getResource('CurrentHeadcount') + (period.year - 1)}</th>
                            <th colSpan="3" className="highlight">{LangHelper.getResource('ApprovedForecastHeadcount') + (period.year - 1)}</th>
                            <th colSpan="3" className="highlight">{LangHelper.getResource('Additional')}</th>
                            <th colSpan="3" className="highlight">{LangHelper.getResource('ForecastHeadcount') + period.year}</th>
                        </tr>
                        <tr>
                            <th className="highlight">{LangHelper.getResource('Expat')}</th>
                            <th className="highlight">{LangHelper.getResource('Local')}</th>
                            <th className="highlight">{LangHelper.getResource('Total')}</th>
                            <th className="highlight">{LangHelper.getResource('Expat')}</th>
                            <th className="highlight">{LangHelper.getResource('Local')}</th>
                            <th className="highlight">{LangHelper.getResource('Total')}</th>
                            <th className="highlight">{LangHelper.getResource('Expat')}</th>
                            <th className="highlight">{LangHelper.getResource('Local')}</th>
                            <th className="highlight">{LangHelper.getResource('Total')}</th>
                            <th className="highlight">{LangHelper.getResource('Expat')}</th>
                            <th className="highlight">{LangHelper.getResource('Local')}</th>
                            <th className="highlight">{LangHelper.getResource('Total')}</th>
                        </tr>
                    </thead>
                    <tbody>{renderBodyTable(companyName)}</tbody>
                </table>
            );
        });

        if (companies.length > 1) {
            tables.push(<CompanyTotalGrid key="company-total-grid" />);
        }

        return tables;
    };

    return (
        <>{renderTable()}</>
    );
};

const mapStateToProps = state => {
    const { period, departments, companies, requestDetails, unitSelected } = state;
    return { period, departments, companies, requestDetails, unitSelected };
};

export default connect(mapStateToProps, null)(CompanyGrid);