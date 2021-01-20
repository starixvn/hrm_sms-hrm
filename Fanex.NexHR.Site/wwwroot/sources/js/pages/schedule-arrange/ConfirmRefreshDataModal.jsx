import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';
import { Button, Modal } from 'react-bootstrap';

export default class ConfirmRefreshDataModal extends PureComponent {
    static propTypes = {
        show: PropTypes.bool,
        onHide: PropTypes.func,
        onSubmit: PropTypes.func
    };

    static defaultProps = {
        show: false,
        onHide: function () {
            // This is intentional
        },
        onSubmit: function () {
            // This is intentional
        }
    };

    constructor(props) {
        super(props);
        this.siteId = document.getElementById('SiteId').value;
        switch (siteId) {
            case 1:
                this.title = 'SMS HRM';
                break;
            case 2:
                this.title = 'HM HRM';
                break;
            case 3:
                this.title = 'CTS HRM';
                break;
        }
    }

    handleSubmit = event => {
        event.preventDefault();
        this.props.onSubmit();
    };

    render() {
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.show} onHide={this.props.onHide}
                dialogClassName="modal-dialog-centered"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{this.title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>It looks like you have been editing something. If you refresh before saving, your changes will be lost.</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={this.props.onSubmit} bsStyle="primary">Ok</Button>
                    <Button onClick={this.props.onHide} className="btn-cancel">Cancel</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}