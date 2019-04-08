import React, { PureComponent } from 'react';

import ShiftAutocomplete from '../ShiftAutocomplete';

export default class ColDefaultShift extends PureComponent {
    handleChangeDefaultShift = (name, shift) => {
        const employeeId = this.props.employeeId;
        this.props.onChangeDefaultShift(employeeId, shift);
    }

    render() {
        const { isHost, shiftOptions, data } = this.props;

        return (
            <td className="col-shift">
                <div className="shift-autocomplete-wrapper">
                    <ShiftAutocomplete
                        value={data !== null ? data.code : ''}
                        isDisabled={!isHost}
                        shiftOptions={shiftOptions}
                        onChange={this.handleChangeDefaultShift}
                    />
                </div>
            </td>
        );
    }
}