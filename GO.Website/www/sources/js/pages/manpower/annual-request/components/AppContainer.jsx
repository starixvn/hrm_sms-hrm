import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import UnitGrid from './UnitGrid';
import CompanyGrid from './CompanyGrid';

import { ActionCreators } from '../action';

const AppContainer = ({ period, getDataForCreateAnnualRequest }) => {
    useEffect(() => {
        getDataForCreateAnnualRequest();
    }, []);
    return (
        <>
            {   period != null &&
                <div className="manpower-forecast row">
                    <div className="col-unit">
                        <UnitGrid />
                    </div>
                    <div className="col-company">
                        <CompanyGrid/>
                    </div>
                </div>
            }
            {
                period === null &&
                <div className="manpower-forecast row">
                    hết hạn làm request rồi nhé
                </div>
            }
        </>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { period, getDataForCreateAnnualRequest } = state;
    return { period, getDataForCreateAnnualRequest };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);