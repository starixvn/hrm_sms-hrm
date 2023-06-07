import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';
import { Alert } from 'react-bootstrap';
import { isEmpty } from 'lodash';

class FormikErrorMessage extends PureComponent {
    static propTypes = {
        errors: PropTypes.object,
        fieldNames: PropTypes.arrayOf(PropTypes.string),
        touched: PropTypes.object
    };

    static defaultProps = {
        touched: {},
        errors: {},
        fieldNames: []
    };

    constructor(props) {
        super(props);
    }

    validateFormikField = (touched, errors, fieldName) => !(touched[fieldName] && errors[fieldName]);

    render() {
        const { errors, touched, fieldNames } = this.props;
        return (
            !isEmpty(errors)
            && !isEmpty(touched)
            && <Alert variant='danger'>
                <ul className='list-message-error'>
                    {
                        fieldNames.map(field => {
                            const isValid = this.validateFormikField(touched, errors, field);
                            return !isValid && <li key={field}>{errors[field]}</li>;
                        })
                    }
                </ul>
            </Alert>
        );
    }
}

export default FormikErrorMessage;