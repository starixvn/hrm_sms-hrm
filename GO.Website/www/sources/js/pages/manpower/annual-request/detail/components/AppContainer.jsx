import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';

import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import UnitGrid from '../../create/components/UnitGrid';
import CompanyGrid from '../../create/components/CompanyGrid';
import LogModalDialog from './LogModalDialog';

import { ActionCreators } from '../../action';
import { ManpowerFlow, AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';
import { getAnnualRequestStatus } from '../../../common.funcs';

const AppContainer = ({ getAnnualRequestById,
    request,
    requestDetails,
    period,
    unitSelected,
    createAnnualRequest,
    updateStatus,
    setRequest,
    setSubmitModalDialog,
    setverifyApproveModalDialog,
    setResetModalDialog,
    submitModalDialog,
    verifyApproveModalDialog,
    resetModalDialog,
    updateAnnualRequest,
    logDetailModalDialog
}) => {
    useEffect(() => {
        if (window.requestId != null) {
            getAnnualRequestById(window.requestId);
        }
    }, []);

    const isSubmit = () => request.isEdit && request.currentProcessId === ManpowerFlow.UnitHead;
    const isVerify = () => request.isEdit && request.currentProcessId === ManpowerFlow.Verifier &&
        request.statusId === AnnualManpowerForecastRequestStatus.Pending;
    const isApprove = () => request.isEdit && request.currentProcessId === ManpowerFlow.Approver &&
        request.statusId === AnnualManpowerForecastRequestStatus.Verified;
    const isReset = () => request.isEdit &&
        ((request.currentProcessId === ManpowerFlow.Approver && AnnualManpowerForecastRequestStatus.Approved === request.statusId) ||
            (request.currentProcessId === ManpowerFlow.Verifier && AnnualManpowerForecastRequestStatus.Verified === request.statusId));

    const handleSubmit = () => {
        const details = [];
        requestDetails.map(itemDetail => {
            if (itemDetail.expatAddition !== 0 || itemDetail.localAddition !== 0) {
                details.push(itemDetail);
            }
        });
        const param = {
            periodId: period.periodId,
            details: details,
            unitName: unitSelected.value,
            methodName: 'Update '
        };
        createAnnualRequest(param);
        setSubmitModalDialog(false);
    };

    const handleUpdateRequest = () => {
        const details = [];
        requestDetails.map(itemDetail => {
            if (itemDetail.expatAddition !== 0 || itemDetail.localAddition !== 0) {
                details.push(itemDetail);
            }
        });
        const param = {
            requestId: request.requestId,
            details: details,
            statusId: request.statusId + 1,
            methodName: 'Update '
        };

        updateAnnualRequest(param);
        handleRequest(request.statusId + 1);
        setverifyApproveModalDialog(false);
    };

    const handleResetStatus = () => {
        const param = {
            requestId: request.requestId,
            statusId: request.statusId - 1,
            methodName: 'Reset '
        };
        updateStatus(param);
        handleRequest(request.statusId - 1);
        setResetModalDialog(false);
    };

    const handleRequest = statusId => {
        setRequest({
            requestId: request.requestId,
            currentProcessId: request.currentProcessId,
            statusId: statusId,
            isEdit: request.isEdit
        });
    };

    if (request === null) {
        return null;
    }

    return (
        <>
            <div className="filter">
                <span className="status">{LangHelper.getResource('Status') + ': '}</span>
                <span className="bold">{getAnnualRequestStatus(request.statusId)}</span>
            </div>
            <div className="request-form has-filter">
                <div className="manpower-forecast row">
                    <div className="col-unit">
                        <UnitGrid />
                    </div>
                    <div className="col-company">
                        <CompanyGrid isDetail={true} />
                    </div>
                </div>
            </div>
            {
                isSubmit() &&
                <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => setSubmitModalDialog(true)}>
                    {LangHelper.getResource('Submit')}
                </Button>
            }
            {
                isVerify() &&
                <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => setverifyApproveModalDialog(true)}>
                    {LangHelper.getResource('Verify')}
                </Button>
            }
            {
                isApprove() &&
                <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => setverifyApproveModalDialog(true)}>
                    {LangHelper.getResource('Approve')}
                </Button>
            }
            {
                isReset() &&
                <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => setResetModalDialog(true)} >
                    {LangHelper.getResource('Reset')}
                </Button>
            }
            {
                submitModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to <strong>submit</strong> this request?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok" onClick={handleSubmit}>
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel" onClick={() => setSubmitModalDialog(false)}>
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={() => setSubmitModalDialog(false)}
                />
            }
            {
                verifyApproveModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to <strong>{getRequestStatus(request.statusId + 1)}</strong> this request?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok" onClick={handleUpdateRequest}>
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel" onClick={() => setverifyApproveModalDialog(false)}>
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={() => setverifyApproveModalDialog(false)}
                />
            }
            {
                resetModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to <strong>reset</strong> this request?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok" onClick={handleResetStatus}>
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel" onClick={() => setResetModalDialog(false)}>
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={() => setResetModalDialog(false)}
                />
            }
            {
                logDetailModalDialog.show &&
                <LogModalDialog />
            }
            <Button variant="secondary" size="sm" className="mr-3 mt-3" onClick={() => history.back()} >
                {LangHelper.getResource('Back')}
            </Button>
        </>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAnnualRequestById,
        request,
        requestDetails,
        period,
        unitSelected,
        createAnnualRequest,
        updateStatus,
        setRequest,
        setSubmitModalDialog,
        setverifyApproveModalDialog,
        setResetModalDialog,
        submitModalDialog,
        verifyApproveModalDialog,
        resetModalDialog,
        updateAnnualRequest,
        logDetailModalDialog
    } = state;
    return {
        getAnnualRequestById,
        request,
        requestDetails,
        period,
        unitSelected,
        createAnnualRequest,
        updateStatus,
        setRequest,
        setSubmitModalDialog,
        setverifyApproveModalDialog,
        setResetModalDialog,
        submitModalDialog,
        verifyApproveModalDialog,
        resetModalDialog,
        updateAnnualRequest,
        logDetailModalDialog
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);