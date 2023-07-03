import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';
import RowItem from './RowItem';

const CompanyGrid = ({ period, departments, annualRequestFormDetails, jobLevels, unitSelected }) => {
    const renderJoblevel = (itemDepartment, itemSection) => {
        const bodyJobLevel = [];
        let totalSection = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0,
            expatAddition: 0, localAddition: 0, totalAddition: 0,
            expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        jobLevels.map(itemJobLevel => {
            let totalJoblevel = {
                expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
                expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0,
                expatAddition: 0, localAddition: 0, totalAddition: 0,
                expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
            };
            annualRequestFormDetails.map(itemDetail => {
                if (itemDetail.unitName === unitSelected.value && itemDetail.departmentName === itemDepartment.departmentName &&
                    itemDetail.sectionName === itemSection && itemDetail.jobLevel === itemJobLevel) {
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
            bodyJobLevel.push(
                <RowItem key={`${unitSelected.value}-${itemDepartment.departmentName}-${itemSection}-${itemJobLevel}`}
                    total={totalJoblevel} isRenderData={false} isRenderTotal={true} isTableTotal={true} />
            );
        });
        return { totalSection, bodyJobLevel };
    };

    const renderSection = itemDepartment => {
        const bodySection = [];
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
        itemDepartment.sections.map(itemSection => {
            const { totalSection, bodyJobLevel } = renderJoblevel(itemDepartment, itemSection);
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

            bodySection.push(bodyJobLevel);
            bodySection.push(
                <RowItem key={`columnTotalSection-${unitSelected.value}-${itemDepartment.departmentName}-${itemSection}`}
                    total={totalSection} isRenderData={false} isRenderTotal={true} />
            );
        });
        return { totalDepartment, bodySection };
    };
    const renderBodyTable = () => {
        const body = [];
        departments.map(itemDepartment => {
            const { totalDepartment, bodySection } = renderSection(itemDepartment);
            body.push(bodySection);
            body.push(
                <RowItem key={`columnTotalDepartment-${unitSelected.value}-${itemDepartment.departmentName}`}
                    total={totalDepartment} isRenderData={false} isRenderTotal={true} />
            );
        });
        return body;
    };

    const renderTable = () => {
        const tables = [];
        tables.push(
            <table key={`tabletotalcompany`} className="table table-bordered table-company company-Total">
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
                        <th className="highlight">{LangHelper.getResource('Total')}</th>
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
        return tables;
    };
    return (
        <>
            {renderTable()}
        </>
    );
};

const mapStateToProps = state => {
    const { period, departments, annualRequestFormDetails, jobLevels, unitSelected } = state;
    return { period, departments, annualRequestFormDetails, jobLevels, unitSelected };
};

export default connect(mapStateToProps, null)(CompanyGrid);