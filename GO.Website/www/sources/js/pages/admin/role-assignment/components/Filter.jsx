import React, { useEffect, useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';

import { ActionCreators } from '../actions';
import LangHelper from '@jsroot/common/langHelper';
import { DefaultPageSize } from '@jsroot/common/constants';

const Filter = ({ setUserRoleFilterParams, getUserRoles }) => {
    const [filterParams, setFilterParams] = useState({
        keyword: '',
        status: { value: 1, label: 'Active' }
    });

    useEffect(() => {
        const params = getParams();
        setUserRoleFilterParams(params);
        getUserRoles(params);
    }, []);

    const getParams = () => {
        return {
            keyword: filterParams.keyword,
            statusId: filterParams.status.value,
            pageIndex: 1,
            pageSize: DefaultPageSize
        };
    };

    const handleSubmit = event => {
        event.preventDefault();
        const params = getParams();
        setUserRoleFilterParams(params);
        getUserRoles(params);
    };

    const handleChange = (name, value) => {
        setFilterParams({ ...filterParams, [name]: value });
    };

    const statusOptions = [
        { value: null, label: 'All' },
        { value: 1, label: 'Active' },
        { value: 2, label: 'Inactive' },
        { value: 3, label: 'Deleted' }
    ];

    return (
        <Form className="form-filter" onSubmit={handleSubmit}>
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Keyword')}</Form.Label>
                    <Form.Control type="text" size="sm" value={filterParams.keyword}
                        name="keyword" id="keyword"
                        className="shadow-none" autoComplete="off"
                        onChange={event => handleChange('keyword', event.target.value)}
                    />
                </Form.Group>
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Status')}</Form.Label>
                    <Select className={`react-select-container`}
                        classNamePrefix="react-select" isSearchable={false}
                        options={statusOptions} value={filterParams.status}
                        onChange={value => handleChange('status', value)}
                    />
                </Form.Group>
                <Form.Group>
                    <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={handleSubmit}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                </Form.Group>
            </Form.Row>
        </Form>
    );
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

export default connect(null, mapDispatchToProps)(Filter);