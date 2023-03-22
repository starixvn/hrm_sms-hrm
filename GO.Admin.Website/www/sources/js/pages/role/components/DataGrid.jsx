import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Form } from 'react-bootstrap';
import { ActionCreators } from '../actions';
import { Table, HeaderRow, HeaderCell, Row, Cell } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';

export class DataGrid extends Component {
    static defaultProps = {
        roles: []
    };

    constructor(props) {
        super(props);
    }

    handleUpdateStatus = item => {
        this.props.setUpdateStatusModalDialog({ show: true, role: item });
    };

    handleUpdate = item => {
        this.props.setUpdateModalDialog({ show: true, role: item });
    };

    buildGridHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('ID')}</HeaderCell>
                <HeaderCell colWidth={20}>{LangHelper.getResource('Name')}</HeaderCell>
                <HeaderCell colWidth={20}>{LangHelper.getResource('Site')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Active')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Default')}</HeaderCell>
                {
                    window.hasHostPermissionOnRole &&
                    <HeaderCell colWidth={8}>{LangHelper.getResource('Action')}</HeaderCell>
                }
            </HeaderRow>
        );
    };

    buildGridBody = () => {
        if (this.props.roles.length > 0) {
            return this.props.roles.map((item, index) => this.renderRow(item, index + 1));
        }

        return [<Row key={`body${1}`} className="no-data"><Cell colSpan="5">{LangHelper.getResource('ThereIsNoData')}</Cell></Row>];
    };

    renderRow = (item, index) => (
        <Row key={item.roleId}>
            <Cell className="text-center">{index}</Cell>
            <Cell>{item.name}</Cell>
            <Cell>{item.siteName}</Cell>
            <Cell className="text-center">
                <Form.Check
                    type='checkbox'
                    checked={item.isActive}
                    disabled={!window.hasHostPermissionOnRole}
                    onChange={() => this.handleUpdateStatus(item)}
                />
            </Cell>
            <Cell className="text-center">
                <Form.Check
                    type='checkbox' disabled
                    checked={item.isDefault}
                />
            </Cell>
            {
                window.hasHostPermissionOnRole &&
                <Cell className="text-center">
                    <span className="rounded-circle fa fa-pencil icon-edit"
                        onClick={() => this.handleUpdate(item)}
                    />
                </Cell>
            }
        </Row>
    );

    render() {
        return (
            <Table
                width={960}
                autoWidth={false}
                header={this.buildGridHeader()}
                body={this.buildGridBody()}
                containerPadding={0}
                adjustedHeight={10}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { roles } = state;
    return { roles };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);