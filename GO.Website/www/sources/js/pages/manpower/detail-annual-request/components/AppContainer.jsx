import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';

import UnitGrid from '../../annual-request/components/UnitGrid';
import CompanyGrid from '../../annual-request/components/CompanyGrid';

import { ActionCreators } from '../../annual-request/action';

const AppContainer = ({ getAnnualRequestById }) => {
    useEffect(() => {
        if (window.requestId != null) {
            getAnnualRequestById(window.requestId);
        }
    }, []);
    return (
        <div>
            <div className="request-form height-detail">
                <div className="manpower-forecast row">
                    <div className="col-unit">
                        <UnitGrid />
                    </div>
                    <div className="col-company">
                        <CompanyGrid isDetail={ true } />
                    </div>
                </div>
            </div>
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAnnualRequestById } = state;
    return { getAnnualRequestById };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);