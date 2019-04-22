import React, { Component } from 'react';

import MultipleSelect from '@9softstudio/react-multiselect';
import Autocomplete from '../../shared/components/Autocomplete';
import WeekPicker from '../../shared/components/WeekPicker';

export default class Search extends Component {
    handleGroupChange = (selectedItem, selectedItemsKey) => {
        this.props.onChange('groupIds', selectedItemsKey);
    }

    handleEmployeeChange = (value) => {
        this.props.onChange('employee', value);
    }

    handleDateChange = (fromDate, toDate) => {
        this.props.onChange('fromDate', fromDate);
        this.props.onChange('toDate', toDate);
    }

    handleSubmit = event => {
        this.props.onSubmit(event.target.name === 'currentWeek');
    }

    render() {
        return (
            <div className="filter">
                <div className="form-horizontal clearfix">
                    <div className="form-group form-group--m">
                        <div className="label-form">
                            <label className="control-label">Week</label>
                        </div>
                        <div className="calendar-form">
                            <WeekPicker
                                fromDate={this.props.filter.fromDate}
                                toDate={this.props.filter.toDate}
                                onChange={this.handleDateChange}
                            />
                        </div>
                    </div>
                    <div className="form-group form-group--m">
                        <div className="label-form">
                            <label className="control-label">Section</label>
                        </div>
                        <div className="select-form">
                            <MultipleSelect
                                dataSource={this.props.groupOptions}
                                keyField="id"
                                valueField="name"
                                statusField="checked"
                                onChange={this.handleGroupChange}
                                hasSearchBox
                                maxDisplayItemCount={0}
                            />
                        </div>
                    </div>
                    <div className="form-group form-group--m">
                        <div className="label-form">
                            <label className="control-label">Employee</label>
                        </div>
                        <div className="select-form">
                            <Autocomplete
                                className="form-control"
                                sources={this.props.employeeOptions}
                                onInput={this.props.onSearchEmployees}
                                onChange={this.handleEmployeeChange}
                            />
                        </div>
                    </div>
                </div>
                <div className="form-horizontal clearfix">
                    <div className="button-group">
                        <button className="btn btn-primary" onClick={this.handleSubmit}>Submit</button>
                        <button className="btn btn-primary currentWeek" name="currentWeek" onClick={this.handleSubmit}>
                            Current Week
                        </button>
                    </div>
                </div>
            </div>
        );
    }
}