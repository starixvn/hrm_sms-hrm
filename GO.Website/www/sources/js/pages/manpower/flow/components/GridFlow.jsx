import React from 'react';
import { connect } from 'react-redux';

import FlowItem from './FlowItem';
import LangHelper from '@jsroot/common/langHelper';
import { ManpowerFlow } from '@jsroot/common/constants';

const GridFlow = ({ selectedUnit, flows }) => {
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
                            <FlowItem flows={flows} processId={ManpowerFlow.UnitHead} />
                        }
                    </td>
                    <td>
                        {
                            selectedUnit &&
                            <FlowItem flows={flows} processId={ManpowerFlow.Verifier} />
                        }
                    </td>
                    <td>
                        {
                            selectedUnit &&
                            <FlowItem flows={flows} processId={ManpowerFlow.Approver} />
                        }
                    </td>
                    <td>
                        {
                            selectedUnit &&
                            <FlowItem flows={flows} processId={ManpowerFlow.HR} />
                        }
                    </td>
                </tr>
            </tbody>
        </table>
    );
};

const mapStateToProps = state => {
    const { selectedUnit, flows } = state;
    return { selectedUnit, flows };
};

export default connect(mapStateToProps, null)(GridFlow);