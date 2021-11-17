import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';
import { ActionCreators } from '../actions';
import LangHelper from '@jsroot/common/langHelper';
import { DefaultPageSize } from '@jsroot/common/constants';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            username: '',
            status: { value: 1, label: 'Active' },
            site: null
        };
    }

    componentDidMount() {
        const params = this.getParams();
        this.props.setGetAllUserParams(params);
        this.props.getAllUser(params);
    }

    getParams = () => {
        return {
            username: this.state.username,
            statusId: this.state.status.value,
            pageIndex: 1,
            pageSize: DefaultPageSize
        };
    };

    handleSubmit = event => {
        event.preventDefault();
        const params = this.getParams();
        this.props.setGetAllUserParams(params);
        this.props.getAllUser(params);
    };

    handleOpenCreateModalDialog = () => {
        this.props.setCreateModalDialog({ show: true });
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    render() {
        const { username, status, site } = this.state;
        const statusOptions = [{ value: null, label: 'All' },
            { value: 1, label: 'Active' },
            { value: 2, label: 'Inactive' },
            { value: 3, label: 'Deleted' }];
        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Username')}</Form.Label>
                        <Form.Control type="text" size="sm" value={username}
                            name="username" id="username"
                            className="shadow-none" autoComplete="off"
                            onChange={event => this.handleChange('username', event.target.value)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Site')}</Form.Label>
                        <Select className={`react-select-container`} classNamePrefix="react-select"
                            options={[]} value={site}
                            onChange={value => this.handleChange('site', value)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Status')}</Form.Label>
                        <Select className={`react-select-container`}
                            classNamePrefix="react-select" isSearchable={false}
                            options={statusOptions} value={status}
                            onChange={value => this.handleChange('status', value)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={this.handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                        <Button variant="primary" size="sm" onClick={this.handleOpenCreateModalDialog}>
                            {LangHelper.getResource('Create')}
                        </Button>
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAllUserParams } = state;
    return { getAllUserParams };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);