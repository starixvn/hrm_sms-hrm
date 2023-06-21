import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';

const CompanyGrid = ({ units, companies, annualRequestFormDetails }) => {
    const renderBodyTable = company => {
        const body = [];
        units.map(itemUnit => {
            itemUnit.departments.map(itemDepartment => {
                itemDepartment.sections.map(itemSection => {
                    itemSection.jobLevels.map(itemJobLevel => {
                        annualRequestFormDetails.map(itemDetail => {
                            if (itemDetail.unitName === itemUnit.unitName &&
                                itemDetail.departmentName === itemDepartment.departmentName &&
                                itemDetail.sectionName === itemSection.sectionName &&
                                itemDetail.jobLevel === itemJobLevel) {
                                body.push(
                                    <tr key={`${itemUnit.unitName}-${itemDepartment.departmentName}-${itemSection.sectionName}-${itemJobLevel}`}>
                                        <td></td>
                                        <td></td>
                                        <td className="data-total"></td>
                                        <td></td>
                                        <td></td>
                                        <td className="data-total"></td>
                                        <td></td>
                                        <td></td>
                                        <td className="data-total"></td>
                                        <td></td>
                                        <td></td>
                                        <td className="data-total"></td>
                                    </tr>
                                );
                            }
                        });
                    });
                });
            });
        });
        return body;
    };

    const renderTable = () => {
        const tables = [];
        const currentYear = new Date().getFullYear();
        const nextYear = currentYear + 1;
        companies.map(company => {
            tables.push(
                <table key={`tablecompany-${company}`} className="table table-bordered table-company">
                    <thead>
                        <tr>
                            <th colSpan="12">CTI</th>
                        </tr>
                        <tr>
                            <th colSpan="3">{LangHelper.getResource('CurrentHeadcount') + currentYear}</th>
                            <th colSpan="3">{LangHelper.getResource('ApprovedForecastHeadcount') + currentYear}</th>
                            <th colSpan="3">{LangHelper.getResource('Additional')}</th>
                            <th colSpan="3">{LangHelper.getResource('ForecastHeadcount') + nextYear}</th>
                        </tr>
                        <tr>
                            <th>{LangHelper.getResource('Expat')}</th>
                            <th>{LangHelper.getResource('Expat')}</th>
                            <th>{LangHelper.getResource('Local')}</th>
                            <th>{LangHelper.getResource('Total')}</th>
                            <th>{LangHelper.getResource('Local')}</th>
                            <th>{LangHelper.getResource('Total')}</th>
                            <th>{LangHelper.getResource('Expat')} </th>
                            <th>{LangHelper.getResource('Local')}</th>
                            <th>{LangHelper.getResource('Total')}</th>
                            <th>{LangHelper.getResource('Expat')} </th>
                            <th>{LangHelper.getResource('Local')}</th>
                            <th>{LangHelper.getResource('Total')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {renderBodyTable(company)}
                    </tbody>
                </table>
            );
        });
        return tables;
    };
    return (
        <>
            {renderTable()}
        </>
    );
};

const mapStateToProps = state => {
    const { units, companies, annualRequestFormDetails } = state;
    return { units, companies, annualRequestFormDetails };
};

export default connect(mapStateToProps, null)(CompanyGrid);