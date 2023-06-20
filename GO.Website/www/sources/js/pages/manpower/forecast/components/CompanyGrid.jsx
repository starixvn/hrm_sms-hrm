import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';

const CompanyGrid = ({ compamy, companies }) => {
    const renderProcessSetting = () => {
        const cells = [];
        for (let i = 0; i < 37; i++) {
            cells.push(
                <tr>
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

        return cells;
    };
    let companyCurrent = false;
    companies.map(itemCompany => {
        if (itemCompany.companyName === compamy) {
            companyCurrent = true;
        }
    });
    const currentYear = new Date().getFullYear();
    return (
        <>
            {
                companyCurrent &&
                <table className="table table-bordered table-company">
                    <thead>
                        <tr>
                            <th colSpan="12">{compamy}</th>
                        </tr>
                        <tr>
                                <th colSpan="3">{LangHelper.getResource('JobLevel') + currentYear}</th>
                                <th colSpan="3">{LangHelper.getResource('ApprovedForecastHeadcount') + currentYear}</th>
                                <th colSpan="3">{LangHelper.getResource('Additional')}</th>
                                <th colSpan="3">{LangHelper.getResource('ForecastHeadcount') + currentYear +1}</th>
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
                        {renderProcessSetting()}
                    </tbody>
                </table>
            }
        </>
    );
};

const mapStateToProps = state => {
    const { companies } = state;
    return { companies };
};

export default connect(mapStateToProps, null)(CompanyGrid);