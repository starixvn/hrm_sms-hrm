import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../action';

const RowItem = ({ itemDetail, total, isRenderData, isRenderTotal, addAnnualRequestFormDetailsItem }) => {
    const checkZero = number => number !== 0 ? number : '';

    const handleAddAnnualRequestFormDetailsItem = (value, columnName) => {
        const param = {
            unitName: itemDetail.unitName,
            departmentName: itemDetail.departmentName,
            sectionName: itemDetail.sectionName,
            jobLevel: itemDetail.jobLevel,
            companyName: itemDetail.companyName,
            columnName: columnName,
            value: value
        };
        addAnnualRequestFormDetailsItem({ param });
    };

    const renderTotalDetail = () => {
        return (
            <tr>
                <td>{checkZero(total.expatCurrentHeadcount)}</td>
                <td>{checkZero(total.localCurrentHeadcount)}</td>
                <td className="data-total">{checkZero(total.totalCurrentHeadcount)}</td>
                <td>{checkZero(total.expatApprovedHeadcount)}</td>
                <td>{checkZero(total.localApprovedHeadcount)}</td>
                <td className="data-total">{checkZero(total.totalApprovedHeadcount)}</td>
                <td>{checkZero(total.expatAddition)}</td>
                <td>{checkZero(total.localAddition)}</td>
                <td className="data-total">{checkZero(total.totalAddition)}</td>
                <td>{checkZero(total.expatForecastHeadcount)}</td>
                <td>{checkZero(total.localForecastHeadcount)}</td>
                <td className="data-total">{checkZero(total.totalForecastHeadcount)}</td>
            </tr>
        );
    };

    const renderDetailNone = () => {
        return (
            <tr>
                <td></td>
                <td></td>
                <td className="data-total"></td>
                <td></td>
                <td></td>
                <td className="data-total"></td>
                <td></td>
                <td></td>
                <td className="data-total"></td>
                <td></td>
                <td></td>
                <td className="data-total"></td>
            </tr>
        );
    };

    const renderDetail = () => {
        return (
            <tr>
                <td>{checkZero(itemDetail.expatCurrentHeadcount)}</td>
                <td>{checkZero(itemDetail.localCurrentHeadcount)}</td>
                <td className="data-total">{checkZero(itemDetail.totalCurrentHeadcount)}</td>
                <td>{checkZero(itemDetail.expatApprovedHeadcount)}</td>
                <td>{checkZero(itemDetail.localApprovedHeadcount)}</td>
                <td className="data-total">{checkZero(itemDetail.totalApprovedHeadcount)}</td>
                <td>
                    <input className="input-add" type="number"
                        onChange={e => handleAddAnnualRequestFormDetailsItem(e.target.value, 'expatAddition')}
                        value={itemDetail.expatAddition}></input>
                </td>
                <td>
                    <input className="input-add" type="number"
                        onChange={e => handleAddAnnualRequestFormDetailsItem(e.target.value, 'localAddition')}
                        value={itemDetail.localAddition}></input>
                </td>
                <td className="data-total">{checkZero(itemDetail.expatAddition + itemDetail.localAddition)}</td>
                <td>{checkZero(itemDetail.expatCurrentHeadcount + itemDetail.expatApprovedHeadcount)}</td>
                <td>{checkZero(itemDetail.localCurrentHeadcount + itemDetail.localApprovedHeadcount)}</td>
                <td className="data-total">{checkZero(itemDetail.expatCurrentHeadcount + itemDetail.expatApprovedHeadcount +
                    itemDetail.localCurrentHeadcount + itemDetail.localApprovedHeadcount)}</td>
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