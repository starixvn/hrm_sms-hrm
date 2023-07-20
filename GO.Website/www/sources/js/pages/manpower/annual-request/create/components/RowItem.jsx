import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../../action';
import { ManpowerFlow, AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';

const RowItem = ({ isDetail, itemDetail, total, isRenderData, isRenderTotal, addRequestDetailsItem,
    isTableTotal, request, logDetails, setLogDetailModalDialog }) => {
    const MaxAdditionalInput = 100;

    const checkZero = number => number !== 0 ? number : '';

    const checkMaxInput = number => number > MaxAdditionalInput ? MaxAdditionalInput : number;

    const handleAddRequestDetailsItem = (value, columnName) => {
        const param = {
            unitName: itemDetail.unitName,
            departmentName: itemDetail.departmentName,
            sectionName: itemDetail.sectionName,
            jobLevel: itemDetail.jobLevel,
            companyName: itemDetail.companyName,
            columnName: columnName,
            value: (value === '' || value < 0) ? 0 : checkMaxInput(value)
        };
        addRequestDetailsItem(param);
    };

    const renderTotalDetail = () => {
        return (
            <tr>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.expatCurrentHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.localCurrentHeadcount)}</td>
                <td className="highlight">{checkZero(total.totalCurrentHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.expatApprovedHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.localApprovedHeadcount)}</td>
                <td className="highlight">{checkZero(total.totalApprovedHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.expatAddition)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.localAddition)}</td>
                <td className="highlight">{checkZero(total.totalAddition)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.expatForecastHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.localForecastHeadcount)}</td>
                <td className="highlight">{checkZero(total.totalForecastHeadcount)}</td>
            </tr>
        );
    };

    const renderDetailNone = () => {
        return (
            <tr>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
        );
    };

    const isSubmit = () => (request.isEdit && request.currentProcessId === ManpowerFlow.UnitHead) ? true : false;
    const isVerify = () => (request.isEdit && request.currentProcessId === ManpowerFlow.Verifier &&
        request.statusId === AnnualManpowerForecastRequestStatus.Pending) ? true : false;
    const isApprove = () => (request.isEdit && request.currentProcessId === ManpowerFlow.Approver &&
        request.statusId === AnnualManpowerForecastRequestStatus.Verified) ? true : false;

    const handleShowLogModalDialog = () => {
        const logs = logDetails.filter(log => log.companyName === itemDetail.companyName
            && log.unitName === itemDetail.unitName && log.departmentName === itemDetail.departmentName
            && log.sectionName === itemDetail.sectionName && log.jobLevel === itemDetail.jobLevel);

        setLogDetailModalDialog({
            show: true,
            requestDetail: itemDetail,
            logs
        });
    };

    const renderDetail = () => {
        const logs = logDetails.filter(log => log.companyName === itemDetail.companyName
            && log.unitName === itemDetail.unitName && log.departmentName === itemDetail.departmentName
            && log.sectionName === itemDetail.sectionName && log.jobLevel === itemDetail.jobLevel);

        return (
            <tr>
                <td>{checkZero(itemDetail.expatCurrentHeadcount)}</td>
                <td>{checkZero(itemDetail.localCurrentHeadcount)}</td>
                <td className="highlight">{checkZero(itemDetail.totalCurrentHeadcount)}</td>
                <td>{checkZero(itemDetail.expatApprovedHeadcount)}</td>
                <td>{checkZero(itemDetail.localApprovedHeadcount)}</td>
                <td className="highlight">{checkZero(itemDetail.totalApprovedHeadcount)}</td>
                <td>
                    {
                        isDetail && (!request.isEdit || (!isSubmit() && !isVerify() && !isApprove())) && checkZero(itemDetail.expatAddition)
                    }
                    {
                        (!isDetail || (isDetail && (isSubmit() || isVerify() || isApprove()))) &&
                        <input className="input-add" type="number" min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddRequestDetailsItem(parseInt(e.target.value, 10), 'expatAddition')}
                            value={checkZero(itemDetail.expatAddition)}></input>
                    }
                </td>
                <td>
                    {
                        isDetail && (!request.isEdit || (!isSubmit() && !isVerify() && !isApprove())) && checkZero(itemDetail.localAddition)
                    }
                    {
                        (!isDetail || (isDetail && (isSubmit() || isVerify() || isApprove()))) &&
                        <input className="input-add" type="number"
                            min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddRequestDetailsItem(parseInt(e.target.value, 10), 'localAddition')}
                            value={checkZero(itemDetail.localAddition)}></input>
                    }
                </td>
                <td className="highlight">
                    <div className="company-total-add">
                        {checkZero(itemDetail.totalAddition)}
                        {!isTableTotal && logs.length > 1 && <span className="fa fa-info-circle" onClick={handleShowLogModalDialog} />}
                    </div>
                </td>
                <td>{checkZero(itemDetail.expatAddition + itemDetail.expatApprovedHeadcount)}</td>
                <td>{checkZero(itemDetail.localAddition + itemDetail.localApprovedHeadcount)}</td>
                <td className="highlight">{checkZero(itemDetail.expatAddition + itemDetail.expatApprovedHeadcount +
                    itemDetail.localAddition + itemDetail.localApprovedHeadcount)}</td>
            </tr>
        );
    };

    return (
        <>
            {
                isRenderData && !isRenderTotal && renderDetail()
            }
            {
                !isRenderData && !isRenderTotal && renderDetailNone()
            }
            {
                isRenderTotal && renderTotalDetail()
            }
        </>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { request, logDetails, logDetailModalDialog } = state;
    return { request, logDetails, logDetailModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(RowItem);