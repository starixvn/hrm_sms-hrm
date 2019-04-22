import React, { PureComponent } from 'react';

import ShiftAutocomplete from '../ShiftAutocomplete';

export default class ColDefaultShift extends PureComponent {
    handleChangeDefaultShift = (name, shift) => {
        const { groupId, employeeId } = this.props.employeeId;
        this.props.onChangeDefaultShift(groupId, employeeId, shift);
    }

    render() {
        const { isHost, shiftOptions, data } = this.props;

        return (
            <div className="table-cell cell-shift">
                <ShiftAutocomplete
                    value={data !== null ? data.code : ''}
                    isDisabled={!isHost}
                    shiftOptions={shiftOptions}
                    onChange={this.handleChangeDefaultShift}
                />
            </div>
        );
    }
}