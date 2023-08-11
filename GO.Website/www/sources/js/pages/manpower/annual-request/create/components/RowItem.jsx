import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../../action';
import { Equal } from '@jsroot/common/string.funcs';
import { ManpowerFlow, AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';
import { renderNumberOfHeadcount } from '../../../common.funcs';

const RowItem = ({ isDetail, itemDetail, total, isRenderData, isRenderTotal, addRequestDetailsItem,
    isTableTotal, request, logDetails, setLogDetailModalDialog }) => {
    const MaxAdditionalInput = 100;

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

    const isSubmit = () => (request.isEdit && request.currentProcessId === ManpowerFlow.UnitHead) ? true : false;
    const isVerify = () => (request.isEdit && request.currentProcessId === ManpowerFlow.Verifier &&
        request.statusId === AnnualManpowerForecastRequestStatus.Pending) ? true : false;
    const isApprove = () => (request.isEdit && request.currentProcessId === ManpowerFlow.Approver &&
        request.statusId === AnnualManpowerForecastRequestStatus.Verified) ? true : false;

    const handleShowLogModalDialog = () => {
        const logs = logDetails.filter(log => log.companyName === itemDetail.companyName
            && log.unitName === itemDetail.unitName && Equal(log.departmentName, itemDetail.departmentName)
            && Equal(log.sectionName, itemDetail.sectionName) && log.jobLevel === itemDetail.jobLevel);

        setLogDetailModalDialog({
            show: true,
            requestDetail: itemDetail,
            logs
        });
    };

    const renderDetail = () => {
        const logs = logDetails.filter(log => log.companyName === itemDetail.companyName
            && log.unitName === itemDetail.unitName && Equal(log.departmentName, itemDetail.departmentName)
            && Equal(log.sectionName, itemDetail.sectionName) && log.jobLevel === itemDetail.jobLevel);

        return (
            <tr>
                <td>{renderNumberOfHeadcount(itemDetail.expatCurrentHeadcount)}</td>
                <td>{renderNumberOfHeadcount(itemDetail.localCurrentHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(itemDetail.totalCurrentHeadcount)}</td>
                <td>{renderNumberOfHeadcount(itemDetail.expatApprovedHeadcount)}</td>
                <td>{renderNumberOfHeadcount(itemDetail.localApprovedHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(itemDetail.totalApprovedHeadcount)}</td>
                <td>
                    {
                        isDetail && (!request.isEdit || (!isSubmit() && !isVerify() && !isApprove())) && renderNumberOfHeadcount(itemDetail.expatAddition)
                    }
                    {
                        (!isDetail || (isDetail && (isSubmit() || isVerify() || isApprove()))) &&
                        <input className="input-add" type="number" min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddRequestDetailsItem(parseInt(e.target.value, 10), 'expatAddition')}
                            value={renderNumberOfHeadcount(itemDetail.expatAddition)}
                        />
                    }
                </td>
                <td>
                    {
                        isDetail && (!request.isEdit || (!isSubmit() && !isVerify() && !isApprove())) && renderNumberOfHeadcount(itemDetail.localAddition)
                    }
                    {
                        (!isDetail || (isDetail && (isSubmit() || isVerify() || isApprove()))) &&
                        <input className="input-add" type="number"
                            min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddRequestDetailsItem(parseInt(e.target.value, 10), 'localAddition')}
                            value={renderNumberOfHeadcount(itemDetail.localAddition)}
                        />
                    }
                </td>
                <td className="highlight">
                    <div className="company-total-add">
                        {renderNumberOfHeadcount(itemDetail.totalAddition)}
                        {!isTableTotal && logs.length > 1 && <span className="fa fa-info-circle" onClick={handleShowLogModalDialog} />}
                    </div>
                </td>
                <td>{renderNumberOfHeadcount(itemDetail.expatAddition + itemDetail.expatApprovedHeadcount)}</td>
                <td>{renderNumberOfHeadcount(itemDetail.localAddition + itemDetail.localApprovedHeadcount)}</td>
                <td className="highlight">{renderNumberOfHeadcount(itemDetail.expatAddition + itemDetail.expatApprovedHeadcount +
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