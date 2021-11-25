import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Form } from 'react-bootstrap';
import { ActionCreators } from '../../actions';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';

export class DataGrid extends Component {
    buildHeader = () => {
        return [
            <HeaderRow>
                <HeaderCell colWidth={20} rowSpan={2}>{LangHelper.getResource('Function')}</HeaderCell>
                <HeaderCell colWidth={10} colSpan={2}>{LangHelper.getResource('Permission')}</HeaderCell>
            </HeaderRow>,
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Read')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Host')}</HeaderCell>
            </HeaderRow>
        ];
    };

    buildBody = () => {
        const { functions, permissions } = this.props.userPermission;
        const totalItems = functions.length;
        if (totalItems > 0) {
            const rows = [];
            for (let i = 0; i < totalItems; i++) {
                rows.push(...this.renderRow(permissions, functions[i]));
            }

            return rows;
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={3}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    renderRow = (permissions, func) => {
        const permission = permissions.find(item => item.functionId === func.functionId);
        const trees = [];
        for (let i = 0; i < func.level; i++) {
            if (i === func.level - 1) {
                trees.push(<span key={i}>&#9492;&#9472;&#9472;</span>);
            } else {
                trees.push(<span key={i} className="invisible">&#9492;&#9472;&#9472;</span>);
            }
        }

        const rows = [
            <Row key={`row-${func.functionId}`}>
                <Cell className={`${func.childs && func.childs.length > 0 && func.level === 0 ? 'border-bottom-0' : ''} ${func.level > 0 ? 'border-top-0 border-bottom-0' : ''}`}>
                    {trees.map(item => item)}
                    <span> {func.functionName}</span>
                </Cell>
                <Cell className="text-center">
                    {
                        (func.policy === null || func.policy === 1) &&
                        <Form.Check type='checkbox' disabled checked={permission && permission.isRead} />
                    }
                </Cell>
                <Cell className="text-center">
                    {
                        (func.policy === null || func.policy === 2) &&
                        <Form.Check type='checkbox' disabled checked={permission && permission.isHost} />
                    }
                </Cell>
            </Row>
        ];

        if (func.childs && func.childs.length > 0) {
            const totalChilds = func.childs.length;
            for (let i = 0; i < totalChilds; i++) {
                rows.push(...this.renderRow(permissions, func.childs[i]));
            }
        }

        return rows;
    };

    render() {
        if (this.props.userPermission === null) {
            return null;
        }

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
    const { userPermission } = state;
    return { userPermission };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);