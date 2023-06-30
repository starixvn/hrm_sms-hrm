import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../action';

const RowItem = ({ isDetail, itemDetail, total, isRenderData, isRenderTotal, addAnnualRequestFormDetailsItem, isTableTotal }) => {
    const MaxAdditionalInput = 100;
    const checkZero = number => number !== 0 ? number : '';
    const checkMaxInput = number => number > MaxAdditionalInput ? MaxAdditionalInput : number;
    const handleAddAnnualRequestFormDetailsItem = (value, columnName) => {
        const param = {
            unitName: itemDetail.unitName,
            departmentName: itemDetail.departmentName,
            sectionName: itemDetail.sectionName,
            jobLevel: itemDetail.jobLevel,
            companyName: itemDetail.companyName,
            columnName: columnName,
            value: (value === '' || value < 0) ? 0 : checkMaxInput(value)
        };
        addAnnualRequestFormDetailsItem({ param });
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
                        isDetail && (itemDetail.expatAddition === 0 ? '' : itemDetail.expatAddition)
                    }
                    {
                        !isDetail && 
                        <input className="input-add" type="number" min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddAnnualRequestFormDetailsItem(parseInt(e.target.value, 10), 'expatAddition')}
                            value={itemDetail.expatAddition === 0 ? '' : itemDetail.expatAddition}></input>
                    }
                </td>
                <td>
                    {
                        isDetail && (itemDetail.localAddition === 0 ? '' : itemDetail.localAddition)
                    }
                    {
                        !isDetail &&
                        <input className="input-add" type="number"
                            min={0} max={MaxAdditionalInput}
                            onChange={e => handleAddAnnualRequestFormDetailsItem(parseInt(e.target.value, 10), 'localAddition')}
                            value={itemDetail.localAddition === 0 ? '' : itemDetail.localAddition}></input>
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

export default connect(null, mapDispatchToProps)(RowItem);