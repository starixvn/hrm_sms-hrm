import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions';
import UserInfo from './UserInfo';
import DataGrid from './DataGrid';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.getUserMappingByUserId(window.selectedUser.userId);
    }

    render() {
        return (
            <>
                <UserInfo />
                <DataGrid />
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

export default connect(null, mapDispatchToProps)(AppContainer);