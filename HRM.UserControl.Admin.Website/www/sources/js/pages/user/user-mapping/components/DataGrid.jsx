import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import RowItem from './RowItem';
import LangHelper from '@jsroot/common/langHelper';

export class DataGrid extends Component {
    buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={20}>{LangHelper.getResource('Site')}</HeaderCell>
                <HeaderCell colWidth={50}>{LangHelper.getResource('User')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildBody = () => {
        const totalItems = this.props.userSites.length;
        if (totalItems > 0) {
            return this.props.userSites.map((item, index) => <RowItem key={index} user={item} index={index + 1} />);
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={10}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    render() {
        return (
            <Table
                width={720}
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