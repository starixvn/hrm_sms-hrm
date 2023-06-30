import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';

import UnitGrid from './UnitGrid';
import Filter from './Filter';
import CompanyGrid from './CompanyGrid';

import { ActionCreators } from '../action';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = ({ period, annualRequestFormDetails, getDataForCreateAnnualRequest, createAnnualRequest, unitSelected, setUnitSelected }) => {
    useEffect(() => {
        if (window.units.length > 1) {
            setUnitSelected({ value: window.units[0], label: window.units[0] });
            getDataForCreateAnnualRequest(window.units[0]);
        }
    }, []);
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
    return (
        <>
            {
                //period != null &&
                <div>
                    {
                        window.units.length > 1 &&
                        <Filter />
                    }
                    <div className={`request-form ${window.units.length > 1 ? 'height-filter' : 'height-none-filter'}`}>
                        <div className="manpower-forecast row">
                            
                            <div className="col-unit">
                                <UnitGrid />
                            </div>
                            <div className="col-company">
                                <CompanyGrid />
                            </div>
                        </div>
                    </div>
                    <Button variant="primary" size="sm" className="mr-3 btn--submit" onClick={handleSubmit}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                </div>
            }
            {
                //period == null &&
                //<div className="breadcrumb inValidPeriod">
                //    <span className="breadcrumb-item active inValidPeriod-item">{LangHelper.getResource('InValidPeriod')}</span>
                //</div>
            }
        </>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { period, annualRequestFormDetails, getDataForCreateAnnualRequest, unitSelected, setUnitSelected } = state;
    return { period, annualRequestFormDetails, getDataForCreateAnnualRequest, unitSelected, setUnitSelected };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);