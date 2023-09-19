import React, { useEffect, useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import Select from 'react-select';

import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ periods, onSubmit, exportExcel, periodSelected }) => {
    const [period, setPeriod] = useState(null);
    const [unitType, setUnitType] = useState({ value: 'BU,PU', label: 'PU - BU' });

    useEffect(() => {
        if (periods.length > 0) {
            setPeriod({ value: periods[0].periodId, label: periods[0].name });

            const params = {
                periodId: periods[0].periodId,
                unitType: unitType.value
            };
            onSubmit(params);
        }
    }, [periods]);

    const getParams = () => {
        return {
            periodId: period ? period.value : 0,
            unitType: unitType.value,
            year: periodSelected.year,
            periodName: periodSelected.name
        };
    };

    const handleSubmit = event => {
        event.preventDefault();

        if (periods.length > 0) {
            const params = getParams();
            onSubmit(params);
        }
    };

    const handleExportExcel = event => {
        event.preventDefault();

        if (periods.length > 0) {
            const params = getParams();
            exportExcel(params);
        }
    };

    const periodOptions = periods.map(item => {
        return {
            value: item.periodId,
            label: item.name
        };
    });

    const unitTypeOptions = [
        { value: 'BU,PU', label: 'PU - BU' },
        { value: 'SU', label: 'SU' }
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
                    <Form.Label>{LangHelper.getResource('UnitType')}</Form.Label>
                    <Select className={`react-select-container`}
                        classNamePrefix="react-select" isSearchable={false}
                        options={unitTypeOptions} value={unitType}
                        onChange={value => setUnitType(value)}
                    />
                </Form.Group>
                <Form.Group className="form-group-width">
                    <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={handleSubmit}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                    <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={handleExportExcel}>
                        {LangHelper.getResource('Export Excel')}
                    </Button>
                </Form.Group>
            </Form.Row>
        </Form>
    );
};

export default Filter;