import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions';
import { Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import DateTimeService from '@jsroot/common/dateTimeService';
import LangHelper from '@jsroot/common/langHelper';

export class DataGrid extends Component {
    handleOpenUpdateUserSiteStatusModalDialog = site => {
        this.props.setUpdateUserSiteStatusModalDialog({ show: true, site });
    };

    handleOpenUpdateUserSiteRoleModalDialog = site => {
        this.props.setUpdateUserSiteRoleModalDialog({ show: true, site });
    };

    buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Site')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Roles')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('LastLoginDate')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Active')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildBody = () => {
        const totalItems = this.props.userSites.length;
        if (totalItems > 0) {
            return this.props.userSites.map((item, index) => this.renderRow(item, index + 1));
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={10}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    renderRow = (site, index) => (
        <Row key={`row-${index}`}>
            <Cell className="text-center">{index}</Cell>
            <Cell>{site.siteName}</Cell>
            <Cell>{site.roles.map(item => item.name).join(', ')}</Cell>
            <Cell className="text-center">{site.lastLoginDate ? DateTimeService.toLongDateTimeFormat(site.lastLoginDate) : ''}</Cell>
            <Cell className="text-center">
                <Form.Check
                    type='checkbox'
                    checked={site.isActive}
                    onChange={() => this.handleOpenUpdateUserSiteStatusModalDialog(site)}
                />
            </Cell>
            <Cell className="text-center">
                <OverlayTrigger placement="auto" overlay={<Tooltip id="tooltip-assign-role">{LangHelper.getResource('AssignRole')}</Tooltip>}>
                    <span className="rounded-circle fa fa-pencil icon-edit"
                        onClick={() => this.handleOpenUpdateUserSiteRoleModalDialog(site)}
                    />
                </OverlayTrigger>
            </Cell>
        </Row>
    );

    render() {
        return (
            <Table
                width={960}
                autoWidth={false}
                header={this.buildHeader()}
                body={this.buildBody()}
                containerPadding={0}
                adjustedHeight={10}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { userSites } = state;
    return { userSites };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);