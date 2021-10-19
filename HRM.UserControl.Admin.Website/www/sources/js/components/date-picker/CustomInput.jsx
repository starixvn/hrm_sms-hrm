import React, { Fragment } from 'react';
import { Form } from 'react-bootstrap';

export default class CustomInput extends React.PureComponent {
    constructor(props) {
        super(props);
    }

    render() {
        const { disabled, onClick, value } = this.props;

        return (
            <Fragment>
                <Form.Control as='input' readOnly disabled={disabled} onClick={onClick} value={value} />
                <i className='date-picker-icon fa fa-calendar' onClick={onClick} />
            </Fragment>
        );
    }
}