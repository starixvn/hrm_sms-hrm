import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';
import RowItem from './RowItem';
import CompanyTotalGrid from './CompanyTotalGrid';

const CompanyGrid = ({ isDetail, period, departments, companies, requestDetails, unitSelected }) => {
    const renderSectionNoneData = (itemDepartment, itemSection) => {
        const bodyJobLevel = [];
        requestDetails.map(itemDetail => {
            if (itemDetail.unitName === unitSelected.value && itemDetail.departmentName === itemDepartment.departmentName &&
                itemDetail.sectionName === itemSection) {
                bodyJobLevel.push(<RowItem key={`${unitSelected.value}-${itemDepartment.departmentName}-${itemSection}-${itemDetail.jobLevel}`}
                        isRenderData={false} isRenderTotal={false}/>);
            }
        });
        return bodyJobLevel;
    };

    const renderDepartmentNoneData = itemDepartment => {
        const bodySection = [];
        itemDepartment.sections.map(itemSection => {
            const bodyJobLevel = renderSectionNoneData(itemDepartment, itemSection);
            bodySection.push(bodyJobLevel);
            bodySection.push(<RowItem key={`columnTotalSection-${unitSelected.value}-${itemDepartment.departmentName}-${itemSection}`}
                    isRenderData={false} isRenderTotal={false}/>);
        });
        return bodySection;
    };

    const renderUnitNoneData = () => {
        const body = [];
        departments.map(itemDepartment => {
            const bodySection = renderDepartmentNoneData(itemDepartment);
            body.push(bodySection);
            body.push(<RowItem key={`columnTotalDepartment-${unitSelected.value}-${itemDepartment.departmentName}`} isRenderData={false} isRenderTotal={false}/>);
        });
        return body;
    };

    const renderJoblevel = (itemDepartment, itemSection, company) => {
        const bodyJobLevel = [];
        let totalSection = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0,
            expatAddition: 0, localAddition: 0, totalAddition: 0,
            expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        requestDetails.map(itemDetail => {
            if (itemDetail.unitName === unitSelected.value && itemDetail.departmentName === itemDepartment.departmentName &&
                itemDetail.sectionName === itemSection && itemDetail.companyName === company) {
                bodyJobLevel.push(
                    <RowItem key={`${unitSelected.value}-${itemDepartment.departmentName}-${itemSection}-${itemDetail.jobLevel}`}
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

    const renderSection = (itemDepartment, company) => {
        const bodySection = [];
        let totalDepartment = {
            expatCurrentHeadcount: 0, localCurrentHeadcount: 0, totalCurrentHeadcount: 0,
            expatApprovedHeadcount: 0, localApprovedHeadcount: 0, totalApprovedHeadcount: 0, expatAddition: 0,
            localAddition: 0, totalAddition: 0, expatForecastHeadcount: 0, localForecastHeadcount: 0, totalForecastHeadcount: 0
        };
        itemDepartment.sections.map(itemSection => {
            if (isExistSection(company, itemSection)) {
                const { totalSection, bodyJobLevel } = renderJoblevel(itemDepartment, itemSection, company);
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
                bodySection.push(<RowItem key={`columnTotalSection-${unitSelected.value}-${itemDepartment.departmentName}-${itemSection}`}
                        total={totalSection} isRenderData={false} isRenderTotal={true}/> );
            }
            else {
                bodySection.push(renderSectionNoneData(itemDepartment, itemSection));
                bodySection.push(<RowItem key={`columnTotalSection-${unitSelected.value}-${itemDepartment.departmentName}-${itemSection}`}
                        isRenderData={false} isRenderTotal={false}/>);
            }
        });
        return { totalDepartment, bodySection };
    };

    const renderDepartment = company => {
        const body = [];
        departments.map(itemDepartment => {
            if (isExistDepartment(company, itemDepartment)) {
                const { totalDepartment, bodySection } = renderSection(itemDepartment, company);
                body.push(bodySection);
                body.push(<RowItem key={`columnTotalDepartment-${unitSelected.value}-${itemDepartment.departmentName}`}
                    total={totalDepartment} isRenderData={false} isRenderTotal={true} />);
            }
            else {
                body.push(renderDepartmentNoneData(itemDepartment));
                body.push(<RowItem key={`columnTotalDepartment-${unitSelected.value}-${itemDepartment.departmentName}`}
                    isRenderData={false} isRenderTotal={false} />);
            }
        });
        return body;
    };

    const isExistCompany = company => requestDetails.find(x => x.unitName === unitSelected.value
        && x.companyName === company) != null ? true : false;

    const isExistDepartment = (company, itemDepartment) => requestDetails.find(x => x.departmentName === itemDepartment.departmentName
        && x.companyName === company) != null ? true : false;

    const isExistSection = (company, itemSection) => requestDetails.find(x => x.sectionName === itemSection
        && x.companyName === company) != null ? true : false;

    const renderBodyTable = company => {
        const body = [];
        if (isExistCompany(company)) {
            body.push(renderDepartment(company));
        } else {
            body.push(renderUnitNoneData());
        }

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
    const { period, departments, companies, requestDetails, unitSelected } = state;
    return { period, departments, companies, requestDetails, unitSelected };
};

export default connect(mapStateToProps, null)(CompanyGrid);