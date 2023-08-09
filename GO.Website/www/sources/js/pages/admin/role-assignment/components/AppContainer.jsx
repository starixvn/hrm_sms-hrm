import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import Filter from './Filter';
import DataGrid from './DataGrid';
import AssignRole from './AssignRole';

import { ActionCreators } from '../actions';

const AppContainer = ({ updateUserRoleModalDialog, getRoles }) => {
    useEffect(() => {
        getRoles();
    }, []);

    return (
        <>
            <Filter />
            <DataGrid />
            {
                updateUserRoleModalDialog.show &&
                <AssignRole />
            }
        </>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { updateUserRoleModalDialog } = state;
    return { updateUserRoleModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);