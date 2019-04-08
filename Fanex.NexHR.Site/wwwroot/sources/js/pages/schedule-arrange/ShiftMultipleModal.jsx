import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, Modal } from 'react-bootstrap';

import ShiftAutocomplete from './ShiftAutocomplete';

export default class ShiftMultipleModal extends Component {
    static propTypes = {
        show: PropTypes.bool,
        onHide: PropTypes.func,
        onSubmit: PropTypes.func
    };

    static defaultProps = {
        show: false,
        onHide: function () { },
        onSubmit: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            shift: null,
            isOffday: false
        };
    }

    handleSubmit = event => {
        event.preventDefault();

        const { shift, isOffday } = this.state;

        this.props.onSubmit(shift, isOffday);
    }

    handleChangeShift = (name, shift) => {
        this.setState({ shift });
    }

    handleChangeOffday = event => {
        this.setState({ isOffday: event.target.checked });
    }

    render() {
        const { shift, isOffday } = this.state;

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.show} onHide={this.props.onHide}
                dialogClassName="shift-multiple-modal modal-dialog-centered"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Shift Multiple</Modal.Title>
                </Modal.Header>
                <Form id="shift-multiple-form" horizontal onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        <div className="form-group">
                            <div className="label-form">
                                <label className="control-label">Shift</label>
                            </div>
                            <div className="input-form">
                                <div className="shift-autocomplete-wrapper">
                                    <ShiftAutocomplete
                                        className="form-control"
                                        value={shift !== null ? shift.code : ''}
                                        shiftOptions={this.props.shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="checkbox">
                                <input type="checkbox" className={`checkbox-custom`} id={`ckb-isOffday`}
                                    checked={isOffday} onChange={this.handleChangeOffday}
                                />
                                <label htmlFor={`ckb-isOffday`}> Is Offday</label>
                            </div>
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button type="submit" bsStyle="primary">Ok</Button>
                        <Button onClick={this.props.onHide} className="btn-cancel">Cancel</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}