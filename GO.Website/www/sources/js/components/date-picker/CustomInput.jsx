import React, { forwardRef } from 'react';
import { Form } from 'react-bootstrap';

const CustomInput = forwardRef(({ disabled, isInvalid, onClick, value }, ref) => {
	return (
		<>
			<Form.Control as='input' readOnly disabled={disabled} onClick={onClick} value={value} ref={ref} className={`${isInvalid}`} />
			<i className='date-picker-icon fa fa-calendar' onClick={onClick} />
		</>
	);
});

export default CustomInput;