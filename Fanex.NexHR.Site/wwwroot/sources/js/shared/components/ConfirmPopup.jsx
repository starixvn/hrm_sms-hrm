import React from 'react';
import { Modal, Button } from 'react-bootstrap';
import Draggable from 'react-draggable';

const ConfirmPopup = ({ show, message, onConfirm, onCancel }) => {
    return (
        <Draggable handle=".modal-header" >
            <Modal keyboard={false} backdrop="static" show={show} onHide={onCancel}>
                <Modal.Header>
                    <Modal.Title>SMS HRM</Modal.Title>
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

export default ConfirmPopup;