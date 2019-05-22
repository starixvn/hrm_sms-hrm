import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Button, Modal } from 'react-bootstrap';

import ShiftAutocomplete from './ShiftAutocomplete';

export default class WeeklyDefaultShiftModal extends Component {
    static propTypes = {
        show: PropTypes.bool,
        isHost: PropTypes.bool,
        nickname: PropTypes.string,
        data: PropTypes.object,
        isUsedWeeklyDefaultShift: PropTypes.bool,
        shiftOptions: PropTypes.arrayOf(PropTypes.object),
        onHide: PropTypes.func,
        onSubmit: PropTypes.func
    };

    static defaultProps = {
        show: false,
        isHost: false,
        nickname: '',
        data: null,
        isUsedWeeklyDefaultShift: false,
        shiftOptions: [],
        onHide: function () { },
        onSubmit: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            data: props.data,
            isUsedWeeklyDefaultShift: props.isUsedWeeklyDefaultShift
        };
    }

    handleSubmit = event => {
        event.preventDefault();

        const { data, isUsedWeeklyDefaultShift } = this.state;

        this.props.onSubmit(data, isUsedWeeklyDefaultShift);
    }

    handleChangeShift = (name, shift) => {
        const shiftId = shift !== null ? shift.id : -1;
        const shiftCode = shift !== null ? shift.code : 'Off';
        let data = { ...this.state.data, [name + 'ShiftId']: shiftId, [name + 'ShiftCode']: shiftCode };

        this.setState({ data });
    }

    handleChangeCheckbox = event => {
        this.setState({ isUsedWeeklyDefaultShift: event.target.checked });
    }

    render() {
        const offDayShiftId = -1;
        const { isUsedWeeklyDefaultShift, data } = this.state;
        const { isHost, show, nickname, shiftOptions } = this.props;

        return (
            <Modal keyboard={false} backdrop="static"
                show={show} onHide={this.props.onHide}
                dialogClassName="modal-dialog-centered modal-weekly-default-shift"
            >
                <Modal.Header closeButton>
                    <Modal.Title>Weekly default shift: {nickname}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="table-weekly-default-shift">
                        <div className="table-header">
                            <div className="header">
                                <div className="table-cell cell-shift">Mon</div>
                                <div className="table-cell cell-shift">Tue</div>
                                <div className="table-cell cell-shift">Wed</div>
                                <div className="table-cell cell-shift">Thu</div>
                                <div className="table-cell cell-shift">Fri</div>
                                <div className="table-cell cell-shift">Sat</div>
                                <div className="table-cell cell-shift">Sun</div>
                            </div>
                        </div>
                        <div className="table-body">
                            <div className="table-row">
                                <div className="table-cell cell-shift">
                                    <ShiftAutocomplete isDisabled={!isHost}
                                        className={`${data !== null && data.monShiftId === offDayShiftId ? 'offday' : ''}`} name="mon"
                                        value={data !== null ? data.monShiftCode : ''}
                                        shiftOptions={shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                                <div className="table-cell cell-shift">
                                    <ShiftAutocomplete isDisabled={!isHost}
                                        className={`${data !== null && data.tueShiftId === offDayShiftId ? 'offday' : ''}`} name="tue"
                                        value={data !== null ? data.tueShiftCode : ''}
                                        shiftOptions={shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                                <div className="table-cell cell-shift">
                                    <ShiftAutocomplete isDisabled={!isHost}
                                        className={`${data !== null && data.wedShiftId === offDayShiftId ? 'offday' : ''}`} name="wed"
                                        value={data !== null ? data.wedShiftCode : ''}
                                        shiftOptions={shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                                <div className="table-cell cell-shift">
                                    <ShiftAutocomplete isDisabled={!isHost}
                                        className={`${data !== null && data.thuShiftId === offDayShiftId ? 'offday' : ''}`} name="thu"
                                        value={data !== null ? data.thuShiftCode : ''}
                                        shiftOptions={shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                                <div className="table-cell cell-shift">
                                    <ShiftAutocomplete isDisabled={!isHost}
                                        className={`${data !== null && data.friShiftId === offDayShiftId ? 'offday' : ''}`} name="fri"
                                        value={data !== null ? data.friShiftCode : ''}
                                        shiftOptions={shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                                <div className="table-cell cell-shift">
                                    <ShiftAutocomplete isDisabled={!isHost}
                                        className={`${data !== null && data.satShiftId === offDayShiftId ? 'offday' : ''}`} name="sat"
                                        value={data !== null ? data.satShiftCode : ''}
                                        shiftOptions={shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                                <div className="table-cell cell-shift">
                                    <ShiftAutocomplete isDisabled={!isHost}
                                        className={`${data !== null && data.sunShiftId === offDayShiftId ? 'offday' : ''}`} name="sun"
                                        value={data !== null ? data.sunShiftCode : ''}
                                        shiftOptions={shiftOptions}
                                        onChange={this.handleChangeShift}
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-weeklyDefaultShift`}
                            checked={isUsedWeeklyDefaultShift} disabled={!isHost} onChange={this.handleChangeCheckbox}
                        />
                        <label htmlFor={`ckb-weeklyDefaultShift`}> Use weekly default shift.</label>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    {
                        this.props.isHost &&
                        <Button type="button" bsStyle="primary" onClick={this.handleSubmit}>Save</Button>
                    }
                    <Button onClick={this.props.onHide} className="btn-cancel">Cancel</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}