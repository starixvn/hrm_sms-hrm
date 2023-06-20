import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import UnitGrid from './UnitGrid';
import CompanyGrid from './CompanyGrid';

import { ActionCreators } from '../action';

const AppContainer = ({ period }) => {
    const { fromDate, toDate } = period;
    const currentDate = new Date();
    const idValid = currentDate >= fromDate && currentDate <= toDate;
    return (
        <>
            {  idValid &&
                <div className="manpower-forecast row">
                    <div className="col-unit">
                        <UnitGrid />
                    </div>
                    <div className="col-company">
                        <CompanyGrid compamy={'CTI'} />
                    </div>
                    <div className="col-company">
                        <CompanyGrid compamy={'Lexfin'} />
                    </div>
                    <div className="col-company">
                        <CompanyGrid compamy={'SMS'} />
                    </div>
                    <div className="col-company">
                        <CompanyGrid compamy={'Hong Ma'} />
                    </div>
                    <div className="col-company">
                        <CompanyGrid compamy={'Nexcel'} />
                    </div>
                    <div className="col-company">
                        <CompanyGrid compamy={'Total'} />
                    </div>
                </div>
            }
            {
                !idValid &&
                <div className="manpower-forecast row">
                    hết hạn làm request rồi nhé
                </div>
            }
        </>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { period } = state;
    return { period };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);