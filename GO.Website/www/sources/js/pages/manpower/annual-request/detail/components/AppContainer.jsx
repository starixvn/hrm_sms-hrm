import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';

import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import UnitGrid from '../../create/components/UnitGrid';
import CompanyGrid from '../../create/components/CompanyGrid';
import { ActionCreators } from '../../action';

import { ManpowerFlow, AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = ({ getAnnualRequestById,
    request,
    requestDetails,
    period,
    unitSelected,
    createAnnualRequest,
    updateStatus,
    setRequest,
    setSubmitModalDialog,
    setVerifyModalDialog,
    setApproveModalDialog,
    setResetModalDialog,
    submitModalDialog,
    verifyModalDialog,
    approveModalDialog,
    resetModalDialog}) => {
    useEffect(() => {
        if (window.requestId != null) {
            getAnnualRequestById(window.requestId);
        }
    }, []);

    const isSubmit = () => (request.isEdit && request.currentProcessId === ManpowerFlow.UnitHead) ? true : false;
    const idVerify = () => (request.isEdit && request.currentProcessId === ManpowerFlow.Verifier &&
        AnnualManpowerForecastRequestStatus.Pending === request.statusId) ? true : false;
    const isApprove = () => (request.isEdit && request.currentProcessId === ManpowerFlow.Approver &&
        AnnualManpowerForecastRequestStatus.Verified === request.statusId) ? true : false;
    const isReset = () => (request.isEdit &&
        ((request.currentProcessId === ManpowerFlow.Approver && AnnualManpowerForecastRequestStatus.Approved === request.statusId) ||
            (request.currentProcessId === ManpowerFlow.Verifier && AnnualManpowerForecastRequestStatus.Verified === request.statusId))) ? true : false;

    const getStatus = () => {
        let status = '';
        switch (request.statusId) {
            case AnnualManpowerForecastRequestStatus.Pending:
                status = LangHelper.getResource('Pending');
                break;
            case AnnualManpowerForecastRequestStatus.Verified:
                status = LangHelper.getResource('Verified');
                break;
            case AnnualManpowerForecastRequestStatus.Approved:
                status = LangHelper.getResource('Approved');
                break;
        }
        return status;
    };
    const handleSubmit = () => {
        const details = [];
        requestDetails.map(itemDetail => {
            if (itemDetail.expatAddition !== 0 || itemDetail.localAddition !== 0) {
                details.push(itemDetail);
            }
        });
        const param = {
            PeriodId: period.periodId,
            Details: details,
            UnitName: unitSelected.value,
            MethodName: 'Update '
        };
        createAnnualRequest(param);
        setSubmitModalDialog(false);
    };

    const handleUpdateStatus = () => {
        const param = {
            RequestId: request.requestId,
            StatusId: request.statusId + 1,
            MethodName: 'Update '
        };
        updateStatus(param);
        handleRequest(request.statusId + 1);
        setVerifyModalDialog(false);
        setApproveModalDialog(false);
    };

    const handleResetStatus = () => {
        const param = {
            RequestId: request.requestId,
            StatusId: request.statusId - 1,
            MethodName: 'Reset '
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

    return (
        <div>
            <div className="filter">
                <span className="status">{LangHelper.getResource('Status') + ': '}</span>
                <span className="bold">{getStatus()}</span>
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
                idVerify() &&
                <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => setVerifyModalDialog(true)}>
                    {LangHelper.getResource('Verify')}
                </Button>
            }
            {
                isApprove() &&
                <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => setApproveModalDialog(true)}>
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
                        <Button variant="primary" size="sm" key="ok"
                            onClick={handleSubmit}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={() => setSubmitModalDialog(false)}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={() => setSubmitModalDialog(false)}
                />
            }
            {
                verifyModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to <strong>verify</strong> this request?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={handleUpdateStatus}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={() => setVerifyModalDialog(false)}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={() => setVerifyModalDialog(false)}
                />
            }
            {
                approveModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to <strong>approve</strong> this request?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={handleResetStatus}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={() => setApproveModalDialog(false)}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={() => setApproveModalDialog(false)}
                />
            }
            {
                resetModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to <strong>reset</strong> this request?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={handleResetStatus}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={() => setResetModalDialog(false)}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={() => setResetModalDialog(false)}
                />
            }
            <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => history.back()} >
                {LangHelper.getResource('Back')}
            </Button>
        </div>
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
        setVerifyModalDialog,
        setApproveModalDialog,
        setResetModalDialog,
        submitModalDialog,
        verifyModalDialog,
        approveModalDialog,
        resetModalDialog    } = state;
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
        setVerifyModalDialog,
        setApproveModalDialog,
        setResetModalDialog,
        submitModalDialog,
        verifyModalDialog,
        approveModalDialog,
        resetModalDialog    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);