import React, { Component } from 'react';
import { ModalDialog } from 'react-bootstrap';
import Draggable from 'react-draggable';

class DraggableModalDialog extends Component {
    render() {
        return (
            <Draggable handle='.modal-header' grid={[10, 10]}>
                <ModalDialog {...this.props} />
            </Draggable>
        );
    }
}

export default DraggableModalDialog;