import React from 'react';
import { Button } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toast } from 'react-toastify';

import { ActionCreators } from '../actions';

import FlowItem from './FlowItem';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import { ManpowerFlow } from '@jsroot/common/constants';

const FlowSetting = ({
    units, selectedUnit, flows, showDeleteModalDialog, setUnits, setSelectedUnit,
    setFlows, setShowDeleteModalDialog, updateFlow, deleteFlow
}) => {
    const handleShowDeleteModalDialog = () => {
        setShowDeleteModalDialog(true);
    };

    const validateFlow = () => {
        let isValid = true;
        if (flows.filter(x => x.processId === ManpowerFlow.UnitHead).length === 0
            || flows.filter(x => x.processId === ManpowerFlow.Verifier).length === 0
            || flows.filter(x => x.processId === ManpowerFlow.Approver).length === 0
            || flows.filter(x => x.processId === ManpowerFlow.HR).length === 0) {
            isValid = false;
        }

        return isValid;
    };

    const handleUpdateFlow = async () => {
        const isValid = validateFlow();
        if (!isValid) {
            toast.error('All steps is required.');
        } else {
            Site.blockUI();
            const params = {
                unitName: selectedUnit.name,
                flows
            };
            await updateFlow( params).then(response => {
                if (response.data.isSuccessful) {
                    toast.success('Update successfully!');

                    if (!selectedUnit.hasFlow) {
                        const tempUnits = [...units];
                        const index = tempUnits.findIndex(x => x.name === selectedUnit.name);
                        tempUnits[index] = { ...tempUnits[index], hasFlow: true };

                        setSelectedUnit({ ...selectedUnit, hasFlow: true });
                        setUnits(tempUnits);
                    }
                } else {
                    toast.error(response.data.message);
                }
            }).then(() => Site.unblockUI());
        }
    };

    const handleCloseDeleteModalDialog = () => {
        setShowDeleteModalDialog(false);
    };

    const handleDeleteFlow = async () => {
        Site.blockUI();
        const params = { unitName: selectedUnit.name };
        await deleteFlow(params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Delete successfully!');

                const tempUnits = [...units];
                const index = tempUnits.findIndex(x => x.name === selectedUnit.name);
                tempUnits[index] = { ...tempUnits[index], hasFlow: false };

                setSelectedUnit({ ...selectedUnit, hasFlow: false });
                setUnits(tempUnits);
                setFlows([]);
                handleCloseDeleteModalDialog();
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

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
            {
                selectedUnit &&
                <div className="button-group">
                    <Button type="button" variant="danger" size="sm"
                        className="mr-3" disabled={flows.length === 0 || !selectedUnit.hasFlow} onClick={handleShowDeleteModalDialog}
                    >
                        {LangHelper.getResource('Delete')}
                    </Button>
                    <Button type="button" variant="primary" size="sm" onClick={handleUpdateFlow}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                </div>
            }
            {
                showDeleteModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to delete flow of <strong>{selectedUnit.name}</strong>?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={handleDeleteFlow}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={handleCloseDeleteModalDialog}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={handleCloseDeleteModalDialog}
                />
            }
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { units, selectedUnit, flows, showDeleteModalDialog } = state;
    return { units, selectedUnit, flows, showDeleteModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(FlowSetting);