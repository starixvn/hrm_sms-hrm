import React from 'react';
import DatePicker from 'react-datepicker';
import CustomInput from './CustomInput';

const DatePickerField = ({ className, showTimeSelect, isInvalid, ...rest }) => {
	return (
		<DatePicker
			className={`form-control form-control-sm datetime-picker ${className}`}
			dateFormat={showTimeSelect ? 'MMM dd, yyyy hh:mm aa' : 'MMM dd, yyyy'}
			timeFormat='hh:mm aa'
			timeIntervals={5}
			customInput={<CustomInput isInvalid={isInvalid} />}
			{...rest}
		/>
	);
};

export default DatePickerField;