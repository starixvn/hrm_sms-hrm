import React, { useEffect, useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import Select from 'react-select';

import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import LangHelper from '@jsroot/common/langHelper';
import { ManpowerFlow, AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';

const Filter = ({ periods, onSubmit, requests, updateStatus, isHost }) => {
    const [period, setPeriod] = useState(null);
    const [verifyApproveModalDialog, setVerifyApproveModalDialog] = useState(false);
    const [resetModalDialog, setResetModalDialog] = useState(false);
    const [unitType, setUnitType] = useState({ value: 'BU, PU', label: 'PU - BU' });

    const getActionDialog = () => {
        var action = '';
        if (requests.length > 0 && requests[0].currentProcessId === ManpowerFlow.Verifier
            && requests.findIndex(x => x.statusId === AnnualManpowerForecastRequestStatus.Pending) !== -1) {
            action = LangHelper.getResource('Verify');
        }
        if (requests.length > 0 && requests[0].currentProcessId === ManpowerFlow.Approver
            && requests.findIndex(x => x.statusId === AnnualManpowerForecastRequestStatus.Verified) !== -1) {
            action = LangHelper.getResource('Approve');
        }
        return action;
    }

    const isVerify = () => requests.length > 0 && requests[0].currentProcessId === ManpowerFlow.Verifier
        && requests.findIndex(x => x.statusId === AnnualManpowerForecastRequestStatus.Pending) !== -1 ? true : false;
    const isApprove = () => requests.length > 0 && requests[0].currentProcessId === ManpowerFlow.Approver
        && requests.findIndex(x => x.statusId === AnnualManpowerForecastRequestStatus.Verified) !== -1 ? true : false;

    const isReset = () => requests.length > 0 && !isVerify() && !isApprove() ? true : false;

    useEffect(() => {
        if (periods.length > 0) {
            setPeriod({ value: periods[0].periodId, label: periods[0].name });

            const params = {
                periodId: periods[0].periodId,
                unitType: unitType.value
            };
            onSubmit(params);
        }
    }, [periods]);

    const getParams = () => {
        return {
            periodId: period ? period.value : 0,
            unitType: unitType.value
        };
    };

    const handleSubmit = event => {
        event.preventDefault();

        if (periods.length > 0) {
            const params = getParams();
            onSubmit(params);
        }
    };

    const periodOptions = periods.map(item => {
        return {
            value: item.periodId,
            label: item.name
        };
    });

    const unitTypeOptions = [
        { value: 'BU, PU', label: 'PU - BU' },
        { value: 'SU', label: 'SU' }
    ];

    const getRequestIds = () => {
        let requestIds = "";
        requests.map((request, index) => {
            if (requests.length === index + 1) {
                requestIds += `${request.requestId}`;
            }
            else {
                requestIds += `${request.requestId}, `;
            }
        });
        return requestIds;
    }

    const handleUpdateRequest = () => {
        const param = {
            requestId: getRequestIds(),
            statusId: requests[0].currentProcessId,
            methodName: 'Update '
        };
        updateStatus(param);
        setVerifyApproveModalDialog(false);
        const params = getParams();
        onSubmit(params);
    };

    const handleResetStatus = () => {
        const param = {
            requestId: getRequestIds(),
            statusId: requests[0].currentProcessId - 1,
            methodName: 'Update '
        };
        updateStatus(param);
        setResetModalDialog(false);
        const params = getParams();
        onSubmit(params);
    };

    return (
        <Form className="form-filter" onSubmit={handleSubmit}>
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Period')}</Form.Label>
                    <Select className={`react-select-container`} classNamePrefix="react-select"
                        options={periodOptions} value={period} isSearchable={false}
                        onChange={value => setPeriod(value)}
                    />
                </Form.Group>
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('UnitType')}</Form.Label>
                    <Select className={`react-select-container`}
                        classNamePrefix="react-select" isSearchable={false}
                        options={unitTypeOptions} value={unitType}
                        onChange={value => setUnitType(value)}
                    />
                </Form.Group>
                <Form.Group className="form-group-width">
                    <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={handleSubmit}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                </Form.Group>
                {
                    isHost && isVerify() &&
                    <Form.Group>
                        <Button variant="primary" size="sm" className="mr-3" onClick={() => setVerifyApproveModalDialog(true)}>
                            {LangHelper.getResource('Verify')}
                        </Button>
                    </Form.Group>
                }
                {
                    isHost && isApprove() &&
                    <Form.Group>
                        <Button variant="primary" size="sm" className="mr-3" onClick={() => setVerifyApproveModalDialog(true)}>
                            {LangHelper.getResource('Approve')}
                        </Button>
                    </Form.Group>
                }
                {
                    isHost &&  isReset() &&
                    <Form.Group>
                        <Button variant="primary" size="sm" className="mr-3" onClick={() => setResetModalDialog(true)}>
                            {LangHelper.getResource('Reset')}
                        </Button>
                    </Form.Group>
                }
                {
                    verifyApproveModalDialog &&
                    <SweetModalDialog
                        title={LangHelper.getResource('Confirmation')}
                        contents={<div>Are you sure you want to <strong>{getActionDialog()}</strong> all request?</div>}
                        buttons={[
                            <Button variant="primary" size="sm" key="ok"
                                onClick={handleUpdateRequest}
                            >
                                {LangHelper.getResource('OK')}
                            </Button>,
                            <Button variant="secondary" size="sm" key="cancel"
                                onClick={() => setVerifyApproveModalDialog(false)}
                            >
                                {LangHelper.getResource('Cancel')}
                            </Button>
                        ]}
                        onClose={() => setVerifyApproveModalDialog(false)}
                    />
                }
                {
                    resetModalDialog &&
                    <SweetModalDialog
                        title={LangHelper.getResource('Confirmation')}
                        contents={<div>Are you sure you want to <strong>reset</strong> all request?</div>}
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
            </Form.Row>
        </Form>
    );
};

export default Filter;