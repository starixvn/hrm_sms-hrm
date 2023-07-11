import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../../action';

import { ManpowerFlow } from '@jsroot/common/constants';

const RowItem = ({ isDetail, itemDetail, total, isRenderData, isRenderTotal, addRequestDetailsItem, isTableTotal, request }) => {
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
        addRequestDetailsItem({ param });
    };

    const renderTotalDetail = () => {
        return (
            <tr>
                <td className={`${isTableTotal? '' : 'highlight'}`}>{checkZero(total.expatCurrentHeadcount)}</td>
                <td className={`${isTableTotal? '' : 'highlight'}`}>{checkZero(total.localCurrentHeadcount)}</td>
                <td className="highlight">{checkZero(total.totalCurrentHeadcount)}</td>
                <td className={`${isTableTotal? '' : 'highlight'}`}>{checkZero(total.expatApprovedHeadcount)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.localApprovedHeadcount)}</td>
                <td className="highlight">{checkZero(total.totalApprovedHeadcount)}</td>
                <td className={`${isTableTotal? '' : 'highlight'}`}>{checkZero(total.expatAddition)}</td>
                <td className={`${isTableTotal ? '' : 'highlight'}`}>{checkZero(total.localAddition)}</td>
                <td className="highlight">{checkZero(total.totalAddition)}</td>
                <td className={`${isTableTotal? '' : 'highlight'}`}>{checkZero(total.expatForecastHeadcount)}</td>
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

    const renderDetail = () => {
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
                        isDetail && (!request.isEdit || request.currentProcessId !== ManpowerFlow.UnitHead) && (itemDetail.expatAddition === 0 ? '' : itemDetail.expatAddition)
                    }
                    {
                        (!isDetail || (isDetail && request.isEdit && request.currentProcessId === ManpowerFlow.UnitHead)) && 
                        <input className="input-add" type="number" min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddRequestDetailsItem(parseInt(e.target.value, 10), 'expatAddition')}
                            value={checkZero(itemDetail.expatAddition)}></input>
                    }
                </td>
                <td>
                    {
                        isDetail && !request.isEdit && (itemDetail.localAddition === 0 ? '' : itemDetail.localAddition)
                    }
                    {
                        (!isDetail || (isDetail && request.isEdit && request.currentProcessId === ManpowerFlow.UnitHead)) && 
                        <input className="input-add" type="number"
                            min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddRequestDetailsItem(parseInt(e.target.value, 10), 'localAddition')}
                            value={checkZero(itemDetail.localAddition)}></input>
                    }
                </td>
                <td className="highlight">{checkZero(itemDetail.totalAddition)}</td>
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
    const { request } = state;
    return { request };
};

export default connect(mapStateToProps, mapDispatchToProps)(RowItem);