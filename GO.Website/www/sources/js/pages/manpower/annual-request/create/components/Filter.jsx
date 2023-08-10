import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';

import { ActionCreators } from '../../action';
import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ unitSelected, setUnitSelected, getDataForCreateAnnualRequest }) => {
    const handleChange = value => {
        setUnitSelected(value);
        getDataForCreateAnnualRequest(value.value);
    };

    const unitOptions = window.units.map(item => {
        return { value: item, label: item };
    });

    return (
        <div className="filter">
            <span className="unit">{LangHelper.getResource('Unit')}</span>
            <Select className="react-select-container unit-option"
                classNamePrefix="react-select" isSearchable={false}
                options={unitOptions} value={unitSelected}
                onChange={handleChange}
            />
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { unitSelected, getDataForCreateAnnualRequest } = state;
    return { unitSelected, getDataForCreateAnnualRequest };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);