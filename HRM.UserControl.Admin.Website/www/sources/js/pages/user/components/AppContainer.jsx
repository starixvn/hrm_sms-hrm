import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Filter from './Filter';
import DataGrid from './DataGrid';
import CreateUser from './Create';
import ResetPassword from './ResetPassword';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const { createModalDialog, resetPasswordModalDialog } = this.props;
        return (
            <>
                <Filter />
                <DataGrid />
                {
                    createModalDialog.show &&
                    <CreateUser />
                }
                {
                    resetPasswordModalDialog.show &&
                    <ResetPassword />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { createModalDialog, resetPasswordModalDialog } = state;
    return { createModalDialog, resetPasswordModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);