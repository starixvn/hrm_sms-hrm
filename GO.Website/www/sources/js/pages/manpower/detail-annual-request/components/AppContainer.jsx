import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';

import UnitGrid from '../../annual-request/components/UnitGrid';
import CompanyGrid from '../../annual-request/components/CompanyGrid';
import { ActionCreators } from '../../annual-request/action';

import { ManpowerFlow, AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = ({ getAnnualRequestById,
    request,
    annualRequestFormDetails,
    period,
    unitSelected,
    createAnnualRequest,
    updateStatus,
    setRequest }) => {

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
        switch (request.statusId) {
            case AnnualManpowerForecastRequestStatus.Pending:
                return 'Pending';
            case AnnualManpowerForecastRequestStatus.Verified:
                return 'Verified';
            case AnnualManpowerForecastRequestStatus.Approved:
                return 'Approved';
        }
    }
    const handleSubmit = () => {
        const details = [];
        annualRequestFormDetails.map(itemDetail => {
            if (itemDetail.expatAddition !== 0 || itemDetail.localAddition !== 0) {
                details.push(itemDetail);
            }
        });
        const param = {
            PeriodId: period.periodId,
            Details: details,
            UnitName: unitSelected.value

        };
        createAnnualRequest(param);
    };

    const handleUpdateStatus = () => {
        const param = {
            RequestId: request.requestId,
            StatusId: request.statusId + 1,
        };
        updateStatus(param);
        handleRequest(request.statusId + 1);
    };

    const handleResetStatus = () => {
        const param = {
            RequestId: request.requestId,
            StatusId: request.statusId - 1,
        };
        updateStatus(param);
        handleRequest(request.statusId - 1);
    };

    const handleRequest = statusId => {
        setRequest({
            requestId: request.requestId,
            currentProcessId: request.currentProcessId,
            verifierId: request.verifierId,
            approverId: request.approverId,
            creator: request.creator,
            statusId: statusId,
            isEdit: request.isEdit
        });
    };
    return (
        <div>
            <div className="filter">
                <span className="status">{LangHelper.getResource('Status')}</span>
                <span className="bold">{getStatus()}</span>
            </div> 
            <div className="request-form height-filter">
                <div className="manpower-forecast row">
                    <div className="col-unit">
                        <UnitGrid />
                    </div>
                    <div className="col-company">
                        <CompanyGrid isDetail={ true } />
                    </div>
                </div>
            </div>
            {
                isSubmit() &&
                <Button variant="primary" size="sm" className="mr-3 btn--submit" onClick={handleSubmit}>
                    {LangHelper.getResource('Submit')}
                </Button>
            }
            {
                idVerify() &&
                <Button variant="primary" size="sm" className="mr-3 btn--submit" onClick={handleUpdateStatus}>
                    {LangHelper.getResource('Verifier')}
                </Button>
            }
            {
                isApprove() &&
                <Button variant="primary" size="sm" className="mr-3 btn--submit" onClick={handleUpdateStatus}>
                    {LangHelper.getResource('Approver')}
                </Button>
            }
            {
                isReset() &&
                <Button variant="primary" size="sm" className="mr-3 btn--submit" onClick={handleResetStatus} >
                    {LangHelper.getResource('Reset')}
                </Button>
            }
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAnnualRequestById, request, annualRequestFormDetails, period, unitSelected, createAnnualRequest, updateStatus, setRequest } = state;
    return { getAnnualRequestById, request, annualRequestFormDetails, period, unitSelected, createAnnualRequest, updateStatus, setRequest };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);