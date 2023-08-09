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
            keyword: '',
            status: { value: 1, label: 'Active' }
        };
    }

    componentDidMount() {
        const params = this.getParams();
        this.props.setUserRoleFilterParams(params);
        this.props.getUserRoles(params);
    }

    getParams = () => {
        return {
            keyword: this.state.keyword,
            statusId: this.state.status.value,
            pageIndex: 1,
            pageSize: DefaultPageSize
        };
    };

    handleSubmit = event => {
        event.preventDefault();
        const params = this.getParams();
        this.props.setUserRoleFilterParams(params);
        this.props.getUserRoles(params);
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    render() {
        const { keyword, status } = this.state;

        const statusOptions = [{ value: null, label: 'All' },
            { value: 1, label: 'Active' },
            { value: 2, label: 'Inactive' },
            { value: 3, label: 'Deleted' }];

        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Keyword')}</Form.Label>
                        <Form.Control type="text" size="sm" value={keyword}
                            name="keyword" id="keyword"
                            className="shadow-none" autoComplete="off"
                            onChange={event => this.handleChange('keyword', event.target.value)}
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
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

export default connect(null, mapDispatchToProps)(Filter);