import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';
import { Alert } from 'react-bootstrap';

class ResponseMessage extends PureComponent {
    static propTypes = {
        isSuccessful: PropTypes.bool,
        message: PropTypes.string
    };

    static defaultProps = {
        isSuccessful: false,
        message: ''
    };

    render() {
        const { isSuccessful, message } = this.props;
        return (
            <Alert variant={isSuccessful ? 'success' : 'danger'} className='d-flex'>
                <div><i className={`fa ${isSuccessful ? 'fa-check' : 'fa-exclamation-triangle'}`} /></div>
                <div className='ml-2'>{message}</div>
            </Alert>
        );
    }
}

export default ResponseMessage;