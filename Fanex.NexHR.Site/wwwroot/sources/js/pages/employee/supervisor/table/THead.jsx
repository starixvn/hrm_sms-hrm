import React, { PureComponent } from 'react';
import Autocomplete from 'react-autocomplete';

import { RoleSystem } from '../../../../shared/constants';

export default class THead extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            value: '',
            employee: null
        };
    }

    handleChange = event => {
        const value = event.target.value;

        this.setState({ value });
        this.props.onSearchEmployees(value);
    }

    handleSelect = (value, item) => {
        this.setState({ value, employee: item });
    }

    handleAddEmployee = () => {
        const employee = this.state.employee;
        if (employee !== null) {
            this.props.onAddEmployee(employee);
            this.setState({ value: '', employee: null });
        }
    }

    render() {
        const { employeeOptions, roleId } = this.props;
        const { value } = this.state;
        let roleName = '';
        switch (roleId) {
            case RoleSystem.Verifier:
                roleName = 'Verifier';
                break;
            case RoleSystem.Approver:
                roleName = 'Approver';
                break;
            case RoleSystem.ActingSupervisor:
                roleName = 'Employee';
                break;
        }

        return (
            <thead>
                <tr>
                    <th colSpan={9}>
                        <div className="header-control">
                            <div>{roleName}</div>
                            <Autocomplete
                                items={employeeOptions}
                                getItemValue={item => item.value}
                                wrapperProps={{ className: 'react-autocomplete-wrapper' }}
                                wrapperStyle={{ display: 'block' }}
                                renderInput={(props) => <input type="text" {...props} />}
                                renderMenu={(items, value, style) => <div className="items" children={items} />}
                                renderItem={(item, isHighlighted) =>
                                    <div className={`item ${isHighlighted ? 'highlighted' : ''}`} key={item.id}>
                                        {item.value}
                                    </div>
                                }
                                inputProps={{
                                    className: 'form-control',
                                    placeholder: 'nickname or full name'
                                }}
                                value={value}
                                onChange={this.handleChange}
                                onSelect={this.handleSelect}
                            />
                            <a href="javascript:;" onClick={this.handleAddEmployee}>+ Add to list</a>
                        </div>
                    </th>
                </tr>
                <tr>
                    <th rowSpan={2}>{roleName}</th>
                    <th rowSpan={2}>Section</th>
                    <th colSpan={6}>Request</th>
                    <th rowSpan={2} />
                </tr>
                <tr>
                    <th>Leave</th>
                    <th>Change Shift</th>
                    <th>Late In/Early Out</th>
                    <th>Check in/out</th>
                    <th>Overtime</th>
                    <th>Keep in view</th>
                </tr>
            </thead>
        );
    }
}