import React from 'react';
import { Button } from 'react-bootstrap';
import FlowItem from './FlowItem';
import LangHelper from '@jsroot/common/langHelper';
import { ManpowerFlow } from '@jsroot/common/constants';

const FlowSetting = ({ selectedUnit, flows, onAddFlowItem, onRemoveFlowItem, onSubmit }) => {
    return (
        <div className="flow-setting">
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
                    <tr>
                        <td>
                            {
                                selectedUnit &&
                                <div className="flow-item">
                                    <i className="avatar fa fa-users" />
                                    <div className="personal-info">
                                        <span className="font-weight-bold">{selectedUnit.name}</span>
                                    </div>
                                </div>
                            }
                        </td>
                        <td>
                            {
                                selectedUnit &&
                                <FlowItem flows={flows} processId={ManpowerFlow.UnitHead}
                                    onAddFlowItem={onAddFlowItem} onRemoveFlowItem={onRemoveFlowItem}
                                />
                            }
                        </td>
                        <td>
                            {
                                selectedUnit &&
                                <FlowItem flows={flows} processId={ManpowerFlow.Verifier}
                                    onAddFlowItem={onAddFlowItem} onRemoveFlowItem={onRemoveFlowItem}
                                />
                            }
                        </td>
                        <td>
                            {
                                selectedUnit &&
                                <FlowItem flows={flows} processId={ManpowerFlow.Approver}
                                    onAddFlowItem={onAddFlowItem} onRemoveFlowItem={onRemoveFlowItem}
                                />
                            }
                        </td>
                        <td>
                            {
                                selectedUnit &&
                                <FlowItem flows={flows} processId={ManpowerFlow.HR}
                                    onAddFlowItem={onAddFlowItem} onRemoveFlowItem={onRemoveFlowItem}
                                />
                            }
                        </td>
                    </tr>
                </tbody>
            </table>
            {
                selectedUnit &&
                <div className="button-group">
                    <Button type="button" variant="danger" size="sm"
                        className="mr-3" disabled={flows.length === 0}
                    >
                        {LangHelper.getResource('Delete')}
                    </Button>
                    <Button type="button" variant="primary" size="sm" onClick={onSubmit}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                </div>
            }
        </div>
    );
};

export default FlowSetting;