import React, { useEffect, useState } from 'react';

import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import LangHelper from '@jsroot/common/langHelper';
import { ManpowerFlow } from '@jsroot/common/constants';

const Result = ({ units }) => {
    const [flows, setFlows] = useState([]);

    useEffect(() => {
        const getFlows = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Manpower/Flow/GetFlows');
            await DataService.getDataAsync(url).then(response => {
                setFlows(response.data);
            }).then(() => Site.unblockUI());
        };

        getFlows();
    }, []);

    const renderFlowItem = flow => {
        return (
            <div className={`flow-item mb-2 ${!flow.isActive || flow.isDeleted ? 'danger' : ''}`}>
                <div className="personal-info">
                    <div><span className="font-weight-bold">{LangHelper.getResource('SiteName')}</span>: <span className="font-italic">{flow.siteName}</span></div>
                    <div><span className="font-weight-bold">{LangHelper.getResource('Code')}</span>: <span className="font-italic">{flow.code}</span></div>
                    <div><span className="font-weight-bold">{LangHelper.getResource('Nickname')}</span>: <span className="font-italic">{flow.nickname}</span></div>
                    <div><span className="font-weight-bold">{LangHelper.getResource('FullName')}</span>: <span className="font-italic">{flow.fullName}</span></div>
                </div>
            </div>
        );
    };

    return (
        <table className="table table-bordered">
            <thead>
                <tr>
                    <th className="unit">{LangHelper.getResource('Unit')}</th>
                    <th className="unit-head">{LangHelper.getResource('UnitHead')}</th>
                    <th className="verifier">{LangHelper.getResource('Verifier')}</th>
                    <th className="approver">{LangHelper.getResource('Approver')}</th>
                    <th className="hr">{LangHelper.getResource('HR')}</th>
                </tr>
            </thead>
            <tbody>
                {
                    units.map(item => {
                        const unitHeads = flows.filter(x => x.unitName === item.name && x.processId === ManpowerFlow.UnitHead);
                        const verifiers = flows.filter(x => x.unitName === item.name && x.processId === ManpowerFlow.Verifier);
                        const approvers = flows.filter(x => x.unitName === item.name && x.processId === ManpowerFlow.Approver);
                        const hrs = flows.filter(x => x.unitName === item.name && x.processId === ManpowerFlow.HR);

                        return (
                            <tr key={`${item.name}`}>
                                <td>{item.name}</td>
                                <td>
                                    {unitHeads.map(unithead => renderFlowItem(unithead))}
                                </td>
                                <td>
                                    {verifiers.map(verifier => renderFlowItem(verifier))}
                                </td>
                                <td>
                                    {approvers.map(approver => renderFlowItem(approver))}
                                </td>
                                <td>
                                    {hrs.map(hr => renderFlowItem(hr))}
                                </td>
                            </tr>
                        );
                    })
                }
            </tbody>
        </table>
    );
};

export default Result;