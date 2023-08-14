import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../../action';
import { isEqual } from '@jsroot/common/string.funcs';
import { ManpowerFlow, AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';
import { renderNumberOfHeadcount } from '../../../common.funcs';

const RowItem = ({ isDetail, requestDetail, total, isRenderData, isRenderTotal, addRequestDetailsItem,
    isTableTotal, request, logDetails, setLogDetailModalDialog }) => {
    const MaxAdditionalInput = 100;

    const checkMaxInput = number => number > MaxAdditionalInput ? MaxAdditionalInput : number;

    const handleAddRequestDetailsItem = (value, columnName) => {
        const param = {
            unitName: requestDetail.unitName,
            departmentName: requestDetail.departmentName,
            sectionName: requestDetail.sectionName,
            jobLevel: requestDetail.jobLevel,
            companyName: requestDetail.companyName,
            columnName: columnName,
            value: (value === '' || value < 0) ? 0 : checkMaxInput(value)
        };
        addRequestDetailsItem(param);
    };

    const renderTotalDetail = () => {
        return (
            <tr>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.expatCurrentHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.localCurrentHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(total.totalCurrentHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.expatApprovedHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.localApprovedHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(total.totalApprovedHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.expatAddition)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.localAddition)}</td>
                <td className="highlight">{renderNumberOfHeadcount(total.totalAddition)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.expatForecastHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{renderNumberOfHeadcount(total.localForecastHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(total.totalForecastHeadcount)}</td>
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

    const isSubmit = () => request.isEdit && request.currentProcessId === ManpowerFlow.UnitHead;
    const isVerify = () => request.isEdit && request.currentProcessId === ManpowerFlow.Verifier &&
        request.statusId === AnnualManpowerForecastRequestStatus.Pending;
    const isApprove = () => request.isEdit && request.currentProcessId === ManpowerFlow.Approver &&
        request.statusId === AnnualManpowerForecastRequestStatus.Verified;

    const getLogs = () => logDetails.filter(log => log.companyName === requestDetail.companyName
        && log.unitName === requestDetail.unitName && isEqual(log.departmentName, requestDetail.departmentName)
        && isEqual(log.sectionName, requestDetail.sectionName) && log.jobLevel === requestDetail.jobLevel);

    const handleShowLogModalDialog = () => {
        setLogDetailModalDialog({
            show: true,
            requestDetail,
            logs: getLogs()
        });
    };

    const renderAddition = (addition, fieldName) => {
        const inputValue = renderNumberOfHeadcount(addition);

        const inputField = <input className="input-add" type="number" min={0} max={MaxAdditionalInput}
            onChange={e => handleAddRequestDetailsItem(parseInt(e.target.value, 10), fieldName)}
            value={inputValue}
        />;

        if (isDetail) {
            if ((!request.isEdit || (!isSubmit() && !isVerify() && !isApprove()))) {
                return <span>{inputValue}</span>;
            }

            if ((isSubmit() || isVerify() || isApprove())) {
                return inputField;
            }
        } else {
            return inputField;
        }

        return <span />;
    };

    const renderDetail = () => {
        const logs = getLogs();

        return (
            <tr>
                <td>{renderNumberOfHeadcount(requestDetail.expatCurrentHeadcount)}</td>
                <td>{renderNumberOfHeadcount(requestDetail.localCurrentHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(requestDetail.totalCurrentHeadcount)}</td>
                <td>{renderNumberOfHeadcount(requestDetail.expatApprovedHeadcount)}</td>
                <td>{renderNumberOfHeadcount(requestDetail.localApprovedHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(requestDetail.totalApprovedHeadcount)}</td>
                <td>{renderAddition(requestDetail.expatAddition, 'expatAddition')}</td>
                <td>{renderAddition(requestDetail.localAddition, 'localAddition')}</td>
                <td className="highlight">
                    <div className="company-total-add">
                        {renderNumberOfHeadcount(requestDetail.totalAddition)}
                        {!isTableTotal && logs.length > 1 && <span className="fa fa-info-circle" onClick={handleShowLogModalDialog} />}
                    </div>
                </td>
                <td>{renderNumberOfHeadcount(requestDetail.expatAddition + requestDetail.expatApprovedHeadcount)}</td>
                <td>{renderNumberOfHeadcount(requestDetail.localAddition + requestDetail.localApprovedHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(requestDetail.expatAddition + requestDetail.expatApprovedHeadcount +
                    requestDetail.localAddition + requestDetail.localApprovedHeadcount)}</td>
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