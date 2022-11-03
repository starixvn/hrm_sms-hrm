import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Autocomplete from 'react-autocomplete';
import { Form } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { ActionCreators } from '../../actions';
import { Cell, Row } from '@jsroot/components/react-grid/index';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

export class RowItem extends Component {
    constructor(props) {
        super(props);

        this.state = {
            user: { ...this.props.user },
            searchkey: '',
            userSites: [],
            isEditing: false
        };
    }

    handleOnChange = (event, value) => {
        this.setState({ searchkey: value }, () => {
            if (value.length > 1) {
                const params = {
                    keyword: value,
                    siteId: this.state.user.siteId,
                    statusId: null
                };

                this.props.searchUserMapping(params);
            } else {
                this.props.setUsers([]);
            }
        });
    };

    handleOnSelect = id => {
        const userId = parseInt(id, 10);
        const index = this.props.users.findIndex(item => item.userId === userId);
        const user = { ... this.state.user, ...this.props.users[index] };

        Site.blockUI();
        const params = {
            userId: userId,
            siteId: user.siteId,
            newUserId: window.selectedUser.userId
        };
        this.props.updateUserMapping(params).then(response => {
            Site.unblockUI();
            if (response.data.isSuccessful) {
                this.setState({ user });
            } else {
                toast.error(response.data.message);
            }
        });
    };

    handleRenderMenu = (items, value, style) => <div className="autocomplete-items" style={{ ...style }} children={items} />;

    handleRenderItem = (item, isHighlighted) => {
        return (
            <div key={item.userId} className="autocomplete-item"
                style={{ backgroundColor: isHighlighted ? '#eee' : '#fff' }}>
                <div><strong>{LangHelper.getResource('Username')}</strong>: {item.username}</div>
                <div><strong>{LangHelper.getResource('Code')}</strong>: {item.code}</div>
                <div><strong>{LangHelper.getResource('Nickname')}</strong>: {item.nickname}</div>
                <div><strong>{LangHelper.getResource('FullName')}</strong>: {item.fullName}</div>
            </div>
        );
    };

    handleOnMenuVisibilityChange = isOpen => {
        if (!isOpen) {
            this.props.setUsers([]);
            this.setState({ searchkey: '' });
        }
    };

    render() {
        const user = this.state.user;
        let items = [];
        if (this.props.users.length > 0) {
            items = this.props.users;
        }

        return (
            <Row>
                <Cell className="text-center">{this.props.index}</Cell>
                <Cell>{user.siteName}</Cell>
                <Cell>
                    {
                        window.hasHostPermissionOnUser &&
                        <Autocomplete wrapperProps={{ 'className': 'autocomplete-container' }}
                            getItemValue={item => item.userId.toString()}
                            value={this.state.searchkey}
                            items={items}
                            onChange={this.handleOnChange}
                            onSelect={this.handleOnSelect}
                            renderMenu={this.handleRenderMenu}
                            renderItem={this.handleRenderItem}
                            renderInput={props => <Form.Control {...props} type="text" size="sm"
                                name="keyword" id="keyword"
                                className="shadow-none" autoComplete="off"
                                placeholder="Search user"
                            />}
                            onMenuVisibilityChange={this.handleOnMenuVisibilityChange}
                        />
                    }
                    <div>- <strong>{LangHelper.getResource('Username')}</strong>: {user.username}</div>
                    <div>- <strong>{LangHelper.getResource('Code')}</strong>: {user.code}</div>
                    <div>- <strong>{LangHelper.getResource('Nickname')}</strong>: {user.nickname}</div>
                    <div>- <strong>{LangHelper.getResource('FullName')}</strong>: {user.fullName}</div>
                </Cell>
            </Row>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { users } = state;
    return { users };
};

export default connect(mapStateToProps, mapDispatchToProps)(RowItem);