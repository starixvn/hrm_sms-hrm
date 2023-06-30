import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Form } from 'react-bootstrap';
import Select from 'react-select';
import LangHelper from '@jsroot/common/langHelper';

import { ActionCreators } from '../action';

const Filter = ({ unitSelected, setUnitSelected, getDataForCreateAnnualRequest }) => {
    const handleChange = async value => {
        await setUnitSelected(value);
        getDataForCreateAnnualRequest(value.value);
    };

    const unitOptions = () => {
        const options = [];
        window.units.map(item => {
            options.push({ value: item, label: item })
        });
        return options;
    };
    
    return (
        <Form className="form-filter">
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Unit')}</Form.Label>
                    <Select className={`react-select-container`}
                        classNamePrefix="react-select" isSearchable={false}
                        options={unitOptions()} value={unitSelected}
                        onChange={handleChange}
                    />
                </Form.Group>
            </Form.Row>
        </Form>
    );
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { unitSelected, getDataForCreateAnnualRequest} = state;
    return { unitSelected, getDataForCreateAnnualRequest};
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);