import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import UnitList from './UnitList';
import FlowSetting from './FlowSetting';

import { ActionCreators } from '../actions';

const AppContainer = ({ getUnits }) => {
    useEffect(() => {
        getUnits();
    }, []);

    return (
        <div className="manpower-flow">
            <UnitList />
            <FlowSetting />
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

export default connect(null, mapDispatchToProps)(AppContainer);