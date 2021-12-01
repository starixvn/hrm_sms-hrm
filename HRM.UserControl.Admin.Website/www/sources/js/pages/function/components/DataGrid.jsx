import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Form } from 'react-bootstrap';
import { ActionCreators } from '../actions';
import { Table, HeaderRow, HeaderCell, Row, Cell } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';
import DateTimeService from '@jsroot/common/dateTimeService';

export class DataGrid extends Component {
    static defaultProps = {
        functions: []
    };

    constructor(props) {
        super(props);
    }

    handleUpdateStatus = item => {
        this.props.setUpdateStatusModalDialog({ show: true, item: item });
    };

    handleUpdate = item => {
        this.props.setUpdateModalDialog({ show: true, item: item});
    };

    buildGridHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={150}>{LangHelper.getResource('Name')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Order')}</HeaderCell>
                <HeaderCell colWidth={12}>{LangHelper.getResource('Policy')}</HeaderCell>
                <HeaderCell colWidth={22}>{LangHelper.getResource('ModifiedDate')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('ModifiedBy')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Active')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildGridBody = functions => {
        if (functions.length > 0) {
            const rows = [];
            for (let i = 0; i < functions.length; i++) {
                rows.push(...this.renderRow(functions[i]));
            }
            return rows;
        }

        return [<Row key={`body${1}`} className="no-data"><Cell colSpan="7">{LangHelper.getResource('ThereIsNoData')}</Cell></Row>];
    };

    renderRow = item => {
        const trees = [];
        for (let i = 0; i < item.level; i++) {
            if (i === item.level - 1) {
                trees.push(<span key={i}>&#9492;&#9472;&#9472;</span>);
            } else {
                trees.push(<span key={i} className="invisible">&#9492;&#9472;&#9472;</span>);
            }
        }
        const policy = this.renderPolicy(item.policy);
        const formatDate = item.modifiedDate !== null ? DateTimeService.toLongDateTimeFormat(item.modifiedDate) : '';
        const rows = [
            <Row key={`row-${item.functionId}`}>
                <Cell className={`${item.childs && item.childs.length > 0 && item.level === 0 ? 'border-bottom-0' : ''} ${item.level > 0 ? 'border-top-0 border-bottom-0' : ''}`}>
                    {trees.map(item => item)}
                    <span> {item.functionName}</span>
                </Cell>
                <Cell className="text-center">{item.displayOrder}</Cell>
                <Cell className={`text-center ${policy}`}><span />{policy}</Cell>
                <Cell className="text-center">{formatDate}</Cell>
                <Cell className="text-center">{item.modifiedBy}</Cell>
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
        ];
        if (item.childs && item.childs.length > 0) {
            const totalChilds = item.childs.length;
            for (let i = 0; i < totalChilds; i++) {
                rows.push(...this.renderRow(item.childs[i]));
            }
        }

        return rows;
    }
    
    renderPolicy = policy =>{
        let policyName = '';
        switch(policy) {
            case null:
                policyName = 'All';
                break;
            case 1:
                policyName = 'Read';
                break;
            case 2:
                policyName = 'Host';
                break; 
            default:
                policyName = '';
                break;  
        }
        return policyName;    
    }   

    render() {
        return (
            <Table
                minWidth={720}
                maxWidth={document.getElementById('app-container').clientWidth}
                autoWidth
                header={this.buildGridHeader()}
                body={this.buildGridBody(this.props.functions)}
                containerPadding={0}
                adjustedHeight={10}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { functions } = state;
    return { functions };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);