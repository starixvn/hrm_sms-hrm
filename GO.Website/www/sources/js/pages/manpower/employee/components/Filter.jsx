import React, { useEffect, useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import Select from 'react-select';
import LangHelper from '@jsroot/common/langHelper';
import { DefaultPageSize } from '@jsroot/common/constants';

const Filter = ({ companies, getEmployeeInfos, onFilterChange }) => {
    const [keyword, setKeyword] = useState('');
    const [company, setCompany] = useState({ value: null, label: 'All' });
    const [status, setStatus] = useState({ value: 1, label: 'Include' });

    useEffect(() => {
        const params = getParams();
        getEmployeeInfos(params);
    }, []);

    const getParams = () => {
        return {
            keyword,
            companyId: company.value,
            statusId: status.value,
            pageIndex: 1,
            pageSize: DefaultPageSize
        };
    };

    const handleSubmit = event => {
        event.preventDefault();
        const params = getParams();
        onFilterChange(params);
        getEmployeeInfos(params);
    };

    const companyOptions = companies.map(item => {
        return {
            value: item.companyId,
            label: item.name
        };
    });
    companyOptions.unshift({ value: null, label: 'All' });

    const statusOptions = [
        { value: null, label: 'All' },
        { value: 1, label: 'Include' },
        { value: 2, label: 'Excluded' }
    ];

    return (
        <Form className="form-filter" onSubmit={handleSubmit}>
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Keyword')}</Form.Label>
                    <Form.Control type="text" size="sm" value={keyword}
                        name="keyword" placeholder="nickname or full name"
                        className="shadow-none" autoComplete="off"
                        onChange={event => setKeyword(event.target.value)}
                    />
                </Form.Group>
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Company')}</Form.Label>
                    <Select className={`react-select-container`} classNamePrefix="react-select"
                        options={companyOptions} value={company} isSearchable={false}
                        onChange={value => setCompany(value)}
                    />
                </Form.Group>
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Status')}</Form.Label>
                    <Select className={`react-select-container`}
                        classNamePrefix="react-select" isSearchable={false}
                        options={statusOptions} value={status}
                        onChange={value => setStatus(value)}
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
};

export default Filter;