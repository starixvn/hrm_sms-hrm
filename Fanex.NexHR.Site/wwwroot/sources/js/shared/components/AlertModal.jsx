import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';
import { Button, Modal } from 'react-bootstrap';

export default class AlertModal extends PureComponent {
    static propTypes = {
        message: PropTypes.string,
        show: PropTypes.bool,
        onHide: PropTypes.func
    };

    static defaultProps = {
        message: '',
        show: false,
        onHide: function () { }
    };

    render() {
        return (
            <Modal
                show={this.props.show} onHide={this.props.onHide}
                dialogClassName="modal-dialog-centered"
            >
                <Modal.Header closeButton>
                    <Modal.Title>SMS HRM</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>{this.props.message}</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.props.onHide} bsStyle="primary">Ok</Button>
                </Modal.Footer>

            </Modal>
        );
    }
}