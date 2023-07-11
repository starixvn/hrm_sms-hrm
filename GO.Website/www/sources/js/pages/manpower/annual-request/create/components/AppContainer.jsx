import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';

import UnitGrid from './UnitGrid';
import Filter from './Filter';
import CompanyGrid from './CompanyGrid';

import { ActionCreators } from '../../action';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = ({ period, requestDetails, getDataForCreateAnnualRequest, createAnnualRequest, unitSelected, setUnitSelected }) => {
    useEffect(() => {
        if (window.units.length > 1) {
            setUnitSelected({ value: window.units[0], label: window.units[0] });
            getDataForCreateAnnualRequest(window.units[0]);
        }
    }, []);

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
            MethodName: 'Create '
        };
        createAnnualRequest(param);
    };

    if (period == null) {
        return (
            <div className="breadcrumb inValidPeriod">
                <span className="breadcrumb-item active inValidPeriod-item">{LangHelper.getResource('InValidPeriod')}</span>
            </div>
        );
    }

    return (
        <div>
            {
                window.units.length > 1 &&
                <Filter />
            }
            <div className={`request-form ${window.units.length > 1 ? 'has-filter' : ''}`}>
                <div className="manpower-forecast row">
                    <div className="col-unit">
                        <UnitGrid />
                    </div>
                    <div className="col-company">
                        <CompanyGrid />
                    </div>
                </div>
            </div>
            <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={handleSubmit}>
                {LangHelper.getResource('Submit')}
            </Button>
            <Button variant="primary" size="sm" className="mr-3 mt-3" onClick={() => history.back()} >
                {LangHelper.getResource('Back')}
            </Button>
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { period, requestDetails, getDataForCreateAnnualRequest, unitSelected, setUnitSelected } = state;
    return { period, requestDetails, getDataForCreateAnnualRequest, unitSelected, setUnitSelected };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);