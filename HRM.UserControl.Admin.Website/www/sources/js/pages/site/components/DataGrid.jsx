import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Form } from 'react-bootstrap';
import { ActionCreators } from '../actions';
import { Table, HeaderRow, HeaderCell, Row, Cell } from '@jsroot/components/react-grid/index';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

export class DataGrid extends Component {
    static defaultProps = {
        sites: []
    };

    constructor(props) {
        super(props);
    }

    handleUpdateStatus = item => {
        this.props.setSelectedSite(item);
        this.props.setUpdateStatusModalDialog({ show: true });
    };

    handleUpdate = item => {
        this.props.getSiteById(item.siteId);
        this.props.setUpdateModalDialog({ show: true });
    };

    buildGridHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={10}>{LangHelper.getResource('ID')}</HeaderCell>
                <HeaderCell colWidth={20}>{LangHelper.getResource('Name')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Users')}</HeaderCell>
                <HeaderCell colWidth={20}>{LangHelper.getResource('URL')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Active')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildGridBody = sites => {
        if (sites.length > 0) {
            return sites.map((item, index) => this.renderRow(item));
        }

        return [<Row key={`body${1}`} className="no-data"><Cell colSpan="6">{LangHelper.getResource('ThereIsNoData')}</Cell></Row>];
    };

    renderRow = item => (
        <Row key={item.siteId}>
            <Cell className="text-center">{item.siteId}</Cell>
            <Cell>{item.siteName}</Cell>
            <Cell className="text-center">{item.numOfUser}</Cell>
            <Cell>{item.siteURL}</Cell>
            <Cell className="text-center">
                <Form.Check
                    type='checkbox'
                    checked={item.isActive}
                    onChange={() => this.handleUpdateStatus(item)}
                />
            </Cell>
            <Cell className="text-center">
                <span className="rounded-circle fa fa-pencil icon-edit"
                    onClick={() => this.handleUpdate(item)}
                />
            </Cell>
        </Row>
    );

    render() {
        return (
            <Table
                minWidth={720}
                maxWidth={document.getElementById('app-container').clientWidth}
                autoWidth
                header={this.buildGridHeader()}
                body={this.buildGridBody(this.props.sites)}
                containerPadding={0}
                adjustedHeight={10}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { sites } = state;
    return { sites };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);