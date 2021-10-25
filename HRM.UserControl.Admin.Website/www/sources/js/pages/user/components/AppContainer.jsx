import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Filter from './Filter';
import DataGrid from './DataGrid';
import CreateUser from './Create';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const { createModalDialog } = this.props;
        return (
            <>
                <Filter />
                <DataGrid />
                {
                    createModalDialog.show &&
                    <CreateUser />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { createModalDialog } = state;
    return { createModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);