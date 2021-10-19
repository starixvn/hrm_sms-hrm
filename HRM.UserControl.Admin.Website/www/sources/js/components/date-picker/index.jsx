import React from 'react';
import DatePicker from 'react-datepicker';
import CustomInput from './CustomInput';

export default class DatePickerField extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        const { className, ...rest } = this.props;

        return (
            <DatePicker
                className={`form-control form-control-sm datetime-picker ${className}`}
                dateFormat={this.props.showTimeSelect ? 'MMM dd, yyyy hh:mm aa' : 'MMM dd, yyyy'}
                timeFormat='hh:mm aa'
                timeIntervals={5}
                customInput={<CustomInput />}
                {...rest}
            />
        );
    }
}