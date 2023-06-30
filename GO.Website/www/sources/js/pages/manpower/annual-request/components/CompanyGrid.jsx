import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';
import RowItem from './RowItem';
import CompanyTotalGrid from './CompanyTotalGrid';

const CompanyGrid = ({ isDetail, period, units, companies, annualRequestFormDetails }) => {
    const renderSectionNoneData = (itemUnit, itemDepartment, itemSection) => {
        const bodyJobLevel = [];
        annualRequestFormDetails.map(itemDetail => {
            if (itemDetail.unitName === itemUnit.unitName && itemDetail.departmentName === itemDepartment.departmentName &&
                itemDetail.sectionName === itemSection) {
                bodyJobLevel.push(<RowItem key={`${itemUnit.unitName}-${itemDepartment.departmentName}-${itemSection}-${itemDetail.jobLevel}`}
                        isRenderData={false} isRenderTotal={false}/>);
            }
        });
        return bodyJobLevel;
    };

    const renderDepartmentNoneData = (itemUnit, itemDepartment) => {
        const bodySection = [];
        itemDepartment.sections.map(itemSection => {
            const bodyJobLevel = renderSectionNoneData(itemUnit, itemDepartment, itemSection);
            bodySection.push(bodyJobLevel);
            bodySection.push(<RowItem key={`columnTotalSection-${itemUnit.unitName}-${itemDepartment.departmentName}-${itemSection}`}
                    isRenderData={false} isRenderTotal={false}/>);
        });
        return bodySection;
    };

    const renderUnitNoneData = itemUnit => {
        const body = [];
        itemUnit.departments.map(itemDepartment => {
            const bodySection = renderDepartmentNoneData(itemUnit, itemDepartment);
            body.push(bodySection);
            body.push(<RowItem key={`columnTotalDepartment-${itemUnit.unitName}-${itemDepartment.departmentName}`} isRenderData={false} isRenderTotal={false}/>);
        });
        return body;
    };

    const renderJoblevel = (itemUnit, itemDepartment, itemSection, company) => {
        const bodyJobLevel = [];
        let totalSection = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0,
            expatAddition: 0, localAddition: 0, totalAddition: 0,
            expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        annualRequestFormDetails.map(itemDetail => {
            if (itemDetail.unitName === itemUnit.unitName && itemDetail.departmentName === itemDepartment.departmentName &&
                itemDetail.sectionName === itemSection && itemDetail.companyName === company) {
                bodyJobLevel.push(
                    <RowItem key={`${itemUnit.unitName}-${itemDepartment.departmentName}-${itemSection}-${itemDetail.jobLevel}`}
                        isDetail={isDetail} itemDetail={itemDetail} isRenderData={true} isRenderTotal={false} />
                );

                totalSection = {
                    expatCurrentHeadcount: totalSection.expatCurrentHeadcount + itemDetail.expatCurrentHeadcount,
                    localCurrentHeadcount: totalSection.localCurrentHeadcount + itemDetail.localCurrentHeadcount,
                    totalCurrentHeadcount: totalSection.totalCurrentHeadcount + itemDetail.totalCurrentHeadcount,
                    expatApprovedHeadcount: totalSection.expatApprovedHeadcount + itemDetail.expatApprovedHeadcount,
                    localApprovedHeadcount: totalSection.localApprovedHeadcount + itemDetail.localApprovedHeadcount,
                    totalApprovedHeadcount: totalSection.totalApprovedHeadcount + itemDetail.totalApprovedHeadcount,
                    expatAddition: totalSection.expatAddition + itemDetail.expatAddition,
                    localAddition: totalSection.localAddition + itemDetail.localAddition,
                    totalAddition: totalSection.totalAddition + itemDetail.totalAddition,
                    expatForecastHeadcount: totalSection.expatForecastHeadcount + itemDetail.expatAddition + itemDetail.expatApprovedHeadcount,
                    localForecastHeadcount: totalSection.localForecastHeadcount + itemDetail.localAddition + itemDetail.localApprovedHeadcount,
                    totalForecastHeadcount: totalSection.totalForecastHeadcount + itemDetail.expatAddition + itemDetail.expatApprovedHeadcount
                        + itemDetail.localAddition + itemDetail.localApprovedHeadcount
                };
            }
        });
        return { totalSection, bodyJobLevel };
    };

    const renderSection = (itemUnit, itemDepartment, company) => {
        const bodySection = [];
        let totalDepartment = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0, expatAddition: 0,
            localAddition: 0, totalAddition: 0, expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        itemDepartment.sections.map(itemSection => {
            if (isExistSection(company, itemSection)) {
                const { totalSection, bodyJobLevel } = renderJoblevel(itemUnit, itemDepartment, itemSection, company);
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
                bodySection.push( <RowItem key={`columnTotalSection-${itemUnit.unitName}-${itemDepartment.departmentName}-${itemSection}`}
                        total={totalSection} isRenderData={false} isRenderTotal={true}/> );
            }
            else {
                bodySection.push(renderSectionNoneData(itemUnit, itemDepartment, itemSection));
                bodySection.push(<RowItem key={`columnTotalSection-${itemUnit.unitName}-${itemDepartment.departmentName}-${itemSection}`}
                        isRenderData={false} isRenderTotal={false}/>);
            }
        });
        return { totalDepartment, bodySection };
    };

    const renderDepartment = (itemUnit, company) => {
        const body = [];
        itemUnit.departments.map(itemDepartment => {
            if (isExistDepartment(company, itemDepartment)) {
                const { totalDepartment, bodySection } = renderSection(itemUnit, itemDepartment, company);
                body.push(bodySection);
                body.push(<RowItem key={`columnTotalDepartment-${itemUnit.unitName}-${itemDepartment.departmentName}`}
                    total={totalDepartment} isRenderData={false} isRenderTotal={true} />);
            }
            else {
                body.push(renderDepartmentNoneData(itemUnit, itemDepartment));
                body.push(<RowItem key={`columnTotalDepartment-${itemUnit.unitName}-${itemDepartment.departmentName}`}
                    isRenderData={false} isRenderTotal={false} />);
            }
        });
        return body;
    };

    const isExistCompany = (company, itemUnit) => annualRequestFormDetails.find(x => x.unitName === itemUnit.unitName
                                                                                && x.companyName === company) != null ? true : false;

    const isExistDepartment = (company, itemDepartment) => annualRequestFormDetails.find(x => x.departmentName === itemDepartment.departmentName
                                                                                && x.companyName === company) != null ? true : false;

    const isExistSection = (company, itemSection) => annualRequestFormDetails.find(x => x.sectionName === itemSection
                                                                                && x.companyName === company) != null ? true : false;

    const renderBodyTable = company => {
        const body = [];
        units.map(itemUnit => {
            if (isExistCompany(company, itemUnit)) {
                body.push(renderDepartment(itemUnit, company));
            }
            else {
                body.push(renderUnitNoneData(itemUnit));
            }
        });
        return body;
    };

    const renderTable = () => {
        const tables = [];
        companies.map(company => {
            tables.push(
                <table key={`tablecompany-${company}`} className={`table table-bordered table-company company-${company}`}>
                    <thead>
                        <tr><th colSpan="12" className="name">{company}</th></tr>
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
                    <tbody>{renderBodyTable(company)}</tbody>
                </table>
            );
        });
        if (companies.length > 1) {
            tables.push(<CompanyTotalGrid key="renderCompanyTotalGrid" />);
        }
        return tables;
    };
    return (
        <>{renderTable()}</>
    );
};

const mapStateToProps = state => {
    const { period, units, companies, annualRequestFormDetails } = state;
    return { period, units, companies, annualRequestFormDetails };
};

export default connect(mapStateToProps, null)(CompanyGrid);