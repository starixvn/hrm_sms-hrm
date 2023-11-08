import React from 'react';
import { Modal, Button } from 'react-bootstrap';
import Draggable from 'react-draggable';

const SweetModalDialog = ({ show, message, onConfirm, onCancel }) => {

    let title = '';
    switch (window.siteId) {
        case 1:
            title = 'SMS HRM';
            break;
        case 2:
            title = 'HM HRM';
            break;
        case 3:
            title = 'CTS HRM';
            break;
    }

    return (
        <Draggable handle=".modal-header" >
            <Modal keyboard={false} backdrop="static" show={show} onHide={onCancel}>
                <Modal.Header>
                    <Modal.Title>{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>{message}</Modal.Body>
                <Modal.Footer>
                    <Button bsStyle="primary" onClick={onConfirm}> OK </Button>
                    <Button className="btn-cancel" onClick={onCancel}> Cancel </Button>
                </Modal.Footer>
            </Modal>
        </Draggable>
    );
};

export default SweetModalDialog;