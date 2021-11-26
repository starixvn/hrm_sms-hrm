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
        menus: []
    };

    constructor(props) {
        super(props);
    }

    handleUpdateStatus = item => {
        this.props.setMenuSelected(item);
        this.props.setUpdateStatusModalDialog({ show: true });
    };

    handleUpdate = item => {
        this.props.setMenuSelected(item);
        this.props.setUpdateModalDialog({ show: true });
    };

    buildGridHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={35}>{LangHelper.getResource('Name')}</HeaderCell>
                <HeaderCell colWidth={30}>{LangHelper.getResource('URL')}</HeaderCell>
                <HeaderCell colWidth={30}>{LangHelper.getResource('Function')}</HeaderCell>
                <HeaderCell colWidth={12}>{LangHelper.getResource('Order')}</HeaderCell>
                <HeaderCell colWidth={20}>{LangHelper.getResource('ModifiedDate')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('ModifiedBy')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Active')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildGridBody = menus => {
        if (menus.length > 0) {
            const menusInfoSort = menus.filter(x=>x.parentId === null).sort((first, second) => first.displayOrder - second.displayOrder);
            const hierarchyMenus = menusInfoSort.map(menu => {
                const copyFunc = {...menu};
                const child = this.GetChildren(menu.menuItemId);
                const childSortOrder = child.sort((first, second) => first.displayOrder - second.displayOrder);
                copyFunc.children = childSortOrder;
                return copyFunc;
            });
            return hierarchyMenus.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} className="no-data"><Cell colSpan="8">{LangHelper.getResource('ThereIsNoData')}</Cell></Row>];
    };

    GetChildren = parentId =>{
        return this.props.menus.filter(x=>x.parentId === parentId).map(child => {
            const copyChild = {...child};
            copyChild.children = this.GetChildren(child.menuItemId);
            return copyChild;
        });
    }

    renderRow = (item, index) => {
        const rows = [];
        let levelChild = 0;
        const classEvenOdd = index % 2 === 0 ? 'even-row' : 'odd-row';
        const formatDate = item.modifiedDate !== null ? DateTimeService.toLongDateTimeFormat(item.modifiedDate) : '';
        const parents = (
            <Row key={item.menuItemId} className={classEvenOdd}>
                <Cell className={`first-level ${item.children.length > 0 ? 'has-child' : ''}`}>
                        <span className="fa fa-minus-circle pr-2" aria-hidden="true" />
                        <span>{item.menuItemName}</span>
                </Cell>
                <Cell >{item.uRL}</Cell>
                <Cell >{item.functionName}</Cell>
                <Cell className="text-center">{item.displayOrder}</Cell>
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
        const child = (
            <Row key={item.menuItemId} className={classEvenOdd}>
                <Cell style={styleChild} className='child-class'>
                    <span className='pr-2'>└──</span>
                    { item.children.length > 0 ? <span className="fa fa-minus-circle pr-2" aria-hidden="true" /> : <span />}
                    <span>{item.menuItemName}</span>
                </Cell>
                <Cell >{item.url}</Cell>
                <Cell >{item.functionName}</Cell>
                <Cell className="text-center">{item.displayOrder}</Cell>
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
                body={this.buildGridBody(this.props.menus)}
                containerPadding={0}
                adjustedHeight={10}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { menus } = state;
    return { menus };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);