import React, { useEffect, useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import Select from 'react-select';
import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ periods, onSubmit }) => {
    const [period, setPeriod] = useState(null);
    const [status, setStatus] = useState({ value: null, label: 'All' });

    useEffect(() => {
        if (periods.length > 0) {
            setPeriod({ value: periods[0].periodId, label: periods[0].name });

            const params = {
                periodId: periods[0].periodId,
                statusId: status.value
            };
            onSubmit(params);
        }
    }, [periods]);

    const getParams = () => {
        return {
            periodId: period ? period.value : 0,
            statusId: status.value
        };
    };

    const handleSubmit = event => {
        event.preventDefault();

        if (periods.length > 0) {
            const params = getParams();
            onSubmit(params);
        }
    };

    const periodOptions = periods.map(item => {
        return {
            value: item.periodId,
            label: item.name
        };
    });

    const statusOptions = [
        { value: null, label: 'All' },
        { value: 1, label: 'Pending' },
        { value: 2, label: 'Verified' },
        { value: 3, label: 'Approved' }
    ];

    return (
        <Form className="form-filter" onSubmit={handleSubmit}>
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Period')}</Form.Label>
                    <Select className={`react-select-container`} classNamePrefix="react-select"
                        options={periodOptions} value={period} isSearchable={false}
                        onChange={value => setPeriod(value)}
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