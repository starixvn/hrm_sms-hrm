import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Form } from 'react-bootstrap';
import { ActionCreators } from '../actions';
import { Table, HeaderRow, HeaderCell, Row, Cell } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';
import DateTimeService from '@jsroot/common/DateTimeService';

export class DataGrid extends Component {
    static defaultProps = {
        functions: []
    };

    constructor(props) {
        super(props);
    }

    handleUpdateStatus = item => {
        this.props.setFunctionSelected(item);
        this.props.setUpdateStatusModalDialog({ show: true });
    };

    handleUpdate = item => {
        this.props.setFunctionSelected(item);
        this.props.setUpdateModalDialog({ show: true });
    };

    buildGridHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={150}>{LangHelper.getResource('Name')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Order')}</HeaderCell>
                <HeaderCell colWidth={12}>{LangHelper.getResource('Policy')}</HeaderCell>
                <HeaderCell colWidth={22}>{LangHelper.getResource('ModifiedDate')}</HeaderCell>
                <HeaderCell colWidth={20}>{LangHelper.getResource('ModifiedBy')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Active')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildGridBody = functionsInfo => {
        if (functionsInfo.length > 0) {
            const hierarchyFunctions = functionsInfo.filter(x=>x.parentId === 0).map(func => {
                const copyFunc = {...func};
                const child = this.GetChildren(func.functionId);
                const childSortOrder = child.sort((first, second) => first.displayOrder - second.displayOrder);
                copyFunc.children = childSortOrder;
                return copyFunc;
            });
            return hierarchyFunctions.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} className="no-data"><Cell colSpan="7">{LangHelper.getResource('ThereIsNoData')}</Cell></Row>];
    };

    GetChildren = parentId =>{
        const func = this.props.functions;
        return  func.filter(x=>x.parentId === parentId).map(child => {
            const copyChild = {...child};
            copyChild.children = this.GetChildren(child.functionId);
            return copyChild;
        });
    }

    renderRow = (item, index) => {
        const rows = [];
        let levelChild = 0;
        const classEvenOdd = index % 2 === 0 ? 'even-row' : 'odd-row';
        const policy = this.renderPolicy(item.policy);
        const formatDate = item.modifiedDate !== null ? DateTimeService.toLongDateTimeFormat(item.modifiedDate) : '';
        const parents = (
            <Row key={item.functionId} className={classEvenOdd}>
                <Cell className={`first-level ${item.children.length > 0 ? 'has-child' : ''}`}>
                        <span className="fa fa-minus-circle pr-2" aria-hidden="true" />
                        <span>{item.functionName}</span>
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
        );
        rows.push(parents);
        if(item.children.length > 0){
            levelChild ++;
            const childRow = item.children.map(item => this.renderChildRow(item, levelChild, classEvenOdd));
            rows.push(childRow);
        }

        return rows;
    }

    renderChildRow = (item, levelChild, classEvenOdd) => {
        const rows = [];
        const styleChild = {
            paddingLeft: this.calculatePadding(levelChild),
            border: "none"
        };
        const formatDate = item.modifiedDate !== null ? DateTimeService.toLongDateTimeFormat(item.modifiedDate) : '';
        const policy = this.renderPolicy(item.policy);
        const child = (
            <Row key={item.functionId} className={classEvenOdd}>
                <Cell style={styleChild} className='child-class'>
                    <span className='pr-2'>└──</span>
                    { item.children.length > 0 ? <span className="fa fa-minus-circle pr-2" aria-hidden="true" /> : <span />}
                    <span>{item.functionName}</span>
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
        );
        rows.push(child);
        if(item.children.length > 0){
            levelChild ++;
            const childRow = item.children.map(item => this.renderChildRow(item, levelChild, classEvenOdd));
            rows.push(childRow);
        }

        return rows;
    };

    renderPolicy = policy =>{
        let policyName = '';
        switch(policy) {
            case 0:
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

    calculatePadding  = levelChild => {
        let levelPadding = 0;
        switch(levelChild) {
            case 1:
                levelPadding = 5;
                break;
            case 2:
                levelPadding = 40;
                break;
            case 3:
                levelPadding = 75;
                break;
            case 4:
                levelPadding = 110;
                break;
            case 5:
                levelPadding = 145;  
                break; 
            default:
                levelPadding = 180;  
                break; 
          }
        return levelPadding;
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