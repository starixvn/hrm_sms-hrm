import React from 'react';
import { connect } from 'react-redux';

import RowItem from './RowItem';

import LangHelper from '@jsroot/common/langHelper';
import { isEqual } from '@jsroot/common/string.funcs';

const CompanyGrid = ({ period, departments, requestDetails, jobLevels, unitSelected }) => {
    const renderJoblevel = (deptItem, sentionName) => {
        const jobLevelRows = [];
        let totalSection = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0,
            expatAddition: 0, localAddition: 0, totalAddition: 0,
            expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        jobLevels.map(jobLevel => {
            let totalJoblevel = {
                expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
                expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0,
                expatAddition: 0, localAddition: 0, totalAddition: 0,
                expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
            };
            requestDetails.map(item => {
                if (item.unitName === unitSelected.value && isEqual(item.departmentName, deptItem.departmentName) &&
                    isEqual(item.sectionName, sentionName) && item.jobLevel === jobLevel) {
                    totalJoblevel = {
                        expatCurrentHeadcount: totalJoblevel.expatCurrentHeadcount + item.expatCurrentHeadcount,
                        localCurrentHeadcount: totalJoblevel.localCurrentHeadcount + item.localCurrentHeadcount,
                        totalCurrentHeadcount: totalJoblevel.totalCurrentHeadcount + item.totalCurrentHeadcount,
                        expatApprovedHeadcount: totalJoblevel.expatApprovedHeadcount + item.expatApprovedHeadcount,
                        localApprovedHeadcount: totalJoblevel.localApprovedHeadcount + item.localApprovedHeadcount,
                        totalApprovedHeadcount: totalJoblevel.totalApprovedHeadcount + item.totalApprovedHeadcount,
                        expatAddition: totalJoblevel.expatAddition + item.expatAddition,
                        localAddition: totalJoblevel.localAddition + item.localAddition,
                        totalAddition: totalJoblevel.totalAddition + item.totalAddition,
                        expatForecastHeadcount: totalJoblevel.expatForecastHeadcount + item.expatAddition + item.expatCurrentHeadcount,
                        localForecastHeadcount: totalJoblevel.localForecastHeadcount + item.localAddition + item.localCurrentHeadcount,
                        totalForecastHeadcount: totalJoblevel.totalForecastHeadcount + item.expatAddition + item.expatCurrentHeadcount
                            + item.localAddition + item.localCurrentHeadcount
                    };
                }
            });
            totalSection = {
                expatCurrentHeadcount: totalSection.expatCurrentHeadcount + totalJoblevel.expatCurrentHeadcount,
                localCurrentHeadcount: totalSection.localCurrentHeadcount + totalJoblevel.localCurrentHeadcount,
                totalCurrentHeadcount: totalSection.totalCurrentHeadcount + totalJoblevel.totalCurrentHeadcount,
                expatApprovedHeadcount: totalSection.expatApprovedHeadcount + totalJoblevel.expatApprovedHeadcount,
                localApprovedHeadcount: totalSection.localApprovedHeadcount + totalJoblevel.localApprovedHeadcount,
                totalApprovedHeadcount: totalSection.totalApprovedHeadcount + totalJoblevel.totalApprovedHeadcount,
                expatAddition: totalSection.expatAddition + totalJoblevel.expatAddition,
                localAddition: totalSection.localAddition + totalJoblevel.localAddition,
                totalAddition: totalSection.totalAddition + totalJoblevel.totalAddition,
                expatForecastHeadcount: totalSection.expatForecastHeadcount + totalJoblevel.expatForecastHeadcount,
                localForecastHeadcount: totalSection.localForecastHeadcount + totalJoblevel.localForecastHeadcount,
                totalForecastHeadcount: totalSection.totalForecastHeadcount + totalJoblevel.totalForecastHeadcount
            };
            jobLevelRows.push(
                <RowItem key={`${unitSelected.value}-${deptItem.departmentName}-${sentionName}-${jobLevel}`}
                    total={totalJoblevel} isRenderData={false} isRenderTotal={true} isTableTotal={true} />
            );
        });
        return { totalSection, jobLevelRows };
    };

    const renderSection = deptItem => {
        const sectionRows = [];
        let totalDepartment = {
            expatCurrentHeadcount: 0,
            localCurrentHeadcount: 0,
            totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0,
            localApprovedHeadcount: 0,
            totalApprovedHeadcount: 0,
            expatAddition: 0,
            localAddition: 0,
            totalAddition: 0,
            expatForecastHeadcount: 0,
            localForecastHeadcount: 0,
            totalForecastHeadcount: 0
        };
        deptItem.sections.map(sentionName => {
            const { totalSection, jobLevelRows } = renderJoblevel(deptItem, sentionName);
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
            sectionRows.push(
                <RowItem key={`col-total-section-${unitSelected.value}-${deptItem.departmentName}-${sentionName}`}
                    total={totalSection} isRenderData={false} isRenderTotal={true} />
            );
        });
        return { totalDepartment, sectionRows };
    };
    const renderBodyTable = () => {
        const rows = [];
        departments.map(deptItem => {
            const { totalDepartment, sectionRows } = renderSection(deptItem);
            rows.push(sectionRows);
            rows.push(
                <RowItem key={`col-total-dept-${unitSelected.value}-${deptItem.departmentName}`}
                    total={totalDepartment} isRenderData={false} isRenderTotal={true} />
            );
        });
        return rows;
    };

    return (
        <table className="table table-bordered table-company company-Total">
            <thead>
                <tr>
                    <th colSpan="12" className="name">{LangHelper.getResource('Total')}</th>
                </tr>
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
            <tbody>
                {renderBodyTable()}
            </tbody>
        </table>
    );
};

const mapStateToProps = state => {
    const { period, departments, requestDetails, jobLevels, unitSelected } = state;
    return { period, departments, requestDetails, jobLevels, unitSelected };
};

export default connect(mapStateToProps, null)(CompanyGrid);