import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../../action';
import DateTimeService from '@jsroot/common/DateTimeService';
import LangHelper from '@jsroot/common/langHelper';

const LogModalDialog = ({ logDetailModalDialog, setLogDetailModalDialog }) => {
    const { requestDetail, logs } = logDetailModalDialog;

    const handleHideModalDialog = () => {
        setLogDetailModalDialog({
            show: false,
            requestDetail: null,
            logs: []
        });
    };

    return (
        <Modal show backdrop="static" className="view-log-modal-dialog" centered onHide={handleHideModalDialog}>
            <Modal.Header closeButton>
                <Modal.Title as="div">View Log</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="info">
                    <div>{LangHelper.getResource('Company')}: <span className="font-weight-bold">{requestDetail.companyName}</span></div>
                    <div>{LangHelper.getResource('Department')}: <span className="font-weight-bold">{requestDetail.departmentName}</span></div>
                    <div>{LangHelper.getResource('Section')}: <span className="font-weight-bold">{requestDetail.sectionName}</span></div>
                    <div>{LangHelper.getResource('JobLevel')}: <span className="font-weight-bold">{requestDetail.jobLevel}</span></div>
                </div>
                <table className="table">
                    <thead>
                        <tr>
                            <th rowSpan="2">Log Date</th>
                            <th rowSpan="2">{LangHelper.getResource('Actor')}</th>
                            <th colSpan="2">{LangHelper.getResource('Additional')}</th>
                        </tr>
                        <tr>
                            <th>{LangHelper.getResource('Expat')}</th>
                            <th>{LangHelper.getResource('Local')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            logs.map(item => {
                                const modifiedDate = DateTimeService.toLongDateTimeFormat(DateTimeService.toClientTimezone(item.modifiedDate));
                                return (
                                    <tr key={`log-${modifiedDate}`}>
                                        <td className="text-center">{modifiedDate}</td>
                                        <td>{item.modifiedBy ? `${item.modifiedBy.fullName} (${item.modifiedBy.nickname})` : ''}</td>
                                        <td className="text-center">{item.expatAddition > 0 ? item.expatAddition : ''}</td>
                                        <td className="text-center">{item.localAddition > 0 ? item.localAddition : ''}</td>
                                    </tr>
                                );
                            })
                        }
                    </tbody>
                </table>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" size="sm" onClick={handleHideModalDialog}>{LangHelper.getResource('Close')}</Button>
            </Modal.Footer>
        </Modal>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { period, requestDetails, getDataForCreateAnnualRequest, unitSelected, setUnitSelected, logDetailModalDialog } = state;
    return { period, requestDetails, getDataForCreateAnnualRequest, unitSelected, setUnitSelected, logDetailModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(LogModalDialog);