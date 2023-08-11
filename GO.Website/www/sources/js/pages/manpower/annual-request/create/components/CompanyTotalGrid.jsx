import React from 'react';
import { connect } from 'react-redux';

import RowItem from './RowItem';

import LangHelper from '@jsroot/common/langHelper';
import { Equal } from '@jsroot/common/string.funcs';

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
            requestDetails.map(itemDetail => {
                if (itemDetail.unitName === unitSelected.value && Equal(itemDetail.departmentName, deptItem.departmentName) &&
                    Equal(itemDetail.sectionName, sentionName) && itemDetail.jobLevel === jobLevel) {
                    totalJoblevel = {
                        expatCurrentHeadcount: totalJoblevel.expatCurrentHeadcount + itemDetail.expatCurrentHeadcount,
                        localCurrentHeadcount: totalJoblevel.localCurrentHeadcount + itemDetail.localCurrentHeadcount,
                        totalCurrentHeadcount: totalJoblevel.totalCurrentHeadcount + itemDetail.totalCurrentHeadcount,
                        expatApprovedHeadcount: totalJoblevel.expatApprovedHeadcount + itemDetail.expatApprovedHeadcount,
                        localApprovedHeadcount: totalJoblevel.localApprovedHeadcount + itemDetail.localApprovedHeadcount,
                        totalApprovedHeadcount: totalJoblevel.totalApprovedHeadcount + itemDetail.totalApprovedHeadcount,
                        expatAddition: totalJoblevel.expatAddition + itemDetail.expatAddition,
                        localAddition: totalJoblevel.localAddition + itemDetail.localAddition,
                        totalAddition: totalJoblevel.totalAddition + itemDetail.totalAddition,
                        expatForecastHeadcount: totalJoblevel.expatForecastHeadcount + itemDetail.expatAddition + itemDetail.expatApprovedHeadcount,
                        localForecastHeadcount: totalJoblevel.localForecastHeadcount + itemDetail.localAddition + itemDetail.localApprovedHeadcount,
                        totalForecastHeadcount: totalJoblevel.totalForecastHeadcount + itemDetail.expatAddition + itemDetail.expatApprovedHeadcount
                            + itemDetail.localAddition + itemDetail.localApprovedHeadcount
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