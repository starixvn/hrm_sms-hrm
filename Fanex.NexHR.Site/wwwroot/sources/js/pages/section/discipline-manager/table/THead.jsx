import React, { PureComponent } from 'react';
import Autocomplete from 'react-autocomplete';

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
        const { employeeOptions } = this.props;
        const { value } = this.state;

        return (
            <thead>
                <tr>
                    <th colSpan={2}>
                        <div className="header-control">
                            <div>Employee</div>
                            <Autocomplete
                                items={employeeOptions}
                                getItemValue={item => item.value}
                                wrapperProps={{ className: 'react-autocomplete-wrapper' }}
                                wrapperStyle={{ display: 'block' }}
                                renderInput={props => <input type="text" {...props} />}
                                renderMenu={items => <div className="items" children={items} />}
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
                    <th>Employee</th>
                    <th />
                </tr>
            </thead>
        );
    }
}