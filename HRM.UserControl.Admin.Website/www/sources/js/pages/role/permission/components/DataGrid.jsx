import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Form } from 'react-bootstrap';
import { ActionCreators } from '../../actions';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';
import { Policy } from '@jsroot/common/constants';

export class DataGrid extends Component {
    buildHeader = () => {
        return [
            <HeaderRow key="header1">
                <HeaderCell colWidth={20} rowSpan={2}>{LangHelper.getResource('Function')}</HeaderCell>
                <HeaderCell colWidth={10} colSpan={2}>{LangHelper.getResource('Permission')}</HeaderCell>
            </HeaderRow>,
            <HeaderRow key="header2">
                <HeaderCell colWidth={5}>{LangHelper.getResource('Read')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Host')}</HeaderCell>
            </HeaderRow>
        ];
    };

    buildBody = () => {
        const { hierarchyFunctions, permissions } = this.props.rolePermission;
        const totalItems = hierarchyFunctions.length;
        if (totalItems > 0) {
            const rows = [];
            for (let i = 0; i < totalItems; i++) {
                rows.push(...this.renderRow(permissions, hierarchyFunctions[i]));
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
                        (func.policy === null || func.policy === Policy.Read) &&
                        <Form.Check type='checkbox' name="funcRead"
                            disabled={!window.hasHostPermissionOnRole}
                            checked={permission !== undefined && permission.isRead}
                            onChange={event => this.handleChangePermission(func, event)}
                        />
                    }
                </Cell>
                <Cell className="text-center">
                    {
                        (func.policy === null || func.policy === Policy.Host) &&
                        <Form.Check type='checkbox' name="funcHost"
                            checked={permission !== undefined && permission.isHost}
                            disabled={!window.hasHostPermissionOnRole}
                            onChange={event => this.handleChangePermission(func, event)}
                        />
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

    handleChangePermission = (func, event) => {
        if (!window.hasHostPermissionOnRole) {
            return;
        }

        const { name, checked } = event.target;
        const { functionId, parentId } = func;
        let { functions, hierarchyFunctions, permissions } = this.props.rolePermission;
        permissions = [...permissions];
        if (permissions.findIndex(item => item.functionId === functionId) === - 1) {
            permissions.push({ ...func, isRead: false, isHost: false });
        }

        this.updatePermission(functions, permissions, functionId, name, checked);
        if (parentId) {
            this.updateParents(functions, permissions, parentId, name, checked);
        }

        this.updateChilds(functions, permissions, functionId, name, checked);
        this.props.setRolePermission({ functions, hierarchyFunctions, permissions });
    };

    updateParents = (functions, permissions, parentId, target, checked) => {
        let isChecked = checked;
        const pindex = functions.findIndex(item => item.functionId === parentId);
        if (permissions.findIndex(item => item.functionId === parentId) === - 1) {
            permissions.push({ ...functions[pindex], isRead: false, isHost: false });
        }

        if (!checked) {
            const childs = functions.filter(child => child.parentId === parentId
                && permissions.filter(item => item.functionId === child.functionId
                    && ((target === 'funcRead' && item.isRead) || (target === 'funcHost' && item.isHost))).length > 0);
            isChecked = childs.length === 0 ? false : true;
        }

        this.updatePermission(functions, permissions, functions[pindex].functionId, target, isChecked);

        if (functions[pindex].parentId) {
            this.updateParents(functions, permissions, functions[pindex].parentId, target, checked);
        }
    };

    updateChilds = (functions, permissions, functionId, target, checked) => {
        const childs = functions.filter(item => item.parentId === functionId);
        const totalItems = childs.length;
        for (let i = 0; i < totalItems; i++) {
            if (permissions.findIndex(item => item.functionId === childs[i].functionId) === - 1) {
                permissions.push({ ...childs[i], isRead: false, isHost: false });
            }

            this.updatePermission(functions, permissions, childs[i].functionId, target, checked);
            this.updateChilds(functions, permissions, childs[i].functionId, target, checked);
        }
    };

    updatePermission = (functions, permissions, functionId, target, checked) => {
        const findex = functions.findIndex(item => item.functionId === functionId);
        const index = permissions.findIndex(item => item.functionId === functionId);
        if (checked) {
            permissions[index] = {
                ...permissions[index],
                isRead: target === 'funcRead' || target === 'funcHost',
                isHost: target === 'funcHost'
            };
            return;
        }

        if (target === 'funcRead' || functions[findex].policy === Policy.Host) {
            permissions[index] = { ...permissions[index], isRead: false, isHost: false };
            return;
        }

        if (functions[findex].policy === null) {
            permissions[index] = { ...permissions[index], isRead: true, isHost: false };
            return;
        }
    };

    render() {
        const { rolePermission } = this.props;
        if (rolePermission === null) {
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
                shouldResetScrollPosition={false}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { rolePermission } = state;
    return { rolePermission };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);