import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';

import UnitGrid from './UnitGrid';
import CompanyGrid from './CompanyGrid';

import { ActionCreators } from '../action';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = ({ period, annualRequestFormDetails, getDataForCreateAnnualRequest, createAnnualRequest }) => {
    useEffect(() => {
        getDataForCreateAnnualRequest();
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
            Details: details
        };
        createAnnualRequest(param);
    };
    return (
        <>
            {
                period != null &&
                <div>
                    <div className="request-form">
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
                period != null &&
                <div className="manpower-forecast row">
                    hết hạn làm request rồi nhé
                </div>
            }
        </>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { period, annualRequestFormDetails, getDataForCreateAnnualRequest } = state;
    return { period, annualRequestFormDetails, getDataForCreateAnnualRequest };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);