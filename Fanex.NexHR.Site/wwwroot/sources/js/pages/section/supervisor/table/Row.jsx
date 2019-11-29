import React, { PureComponent } from 'react';

import DropdownOrder from '../../../../shared/components/DropdownOrder';

export default class Row extends PureComponent {
    handleRemoveEmployee = () => {
        const { ownerId } = this.props.data;
        this.props.onRemoveEmployee(ownerId);
    }

    handleChangePermissionOnRequest = event => {
        const { ownerId } = this.props.data;
        this.props.onChangePermissionOnRequest(ownerId, event.target.name, event.target.checked);
    }

    handleMoveUp = () => {
        const { ownerId } = this.props.data;
        this.props.onChangeOrder('up', ownerId);
    }

    handleMoveDown = () => {
        const { ownerId } = this.props.data;
        this.props.onChangeOrder('down', ownerId);
    }

    handleMoveTop = () => {
        const { ownerId } = this.props.data;
        this.props.onChangeOrder('top', ownerId);
    }

    handleMoveBottom = () => {
        const { ownerId } = this.props.data;
        this.props.onChangeOrder('bottom', ownerId);
    }

    render() {
        const { data } = this.props;

        return (
            <tr>
                <td className="align-left cell-name">
                    <div className="wrapper">
                        <div>{`${data.nickName} (${data.fullName})`}</div>
                        <DropdownOrder id={`dropdown${data.ownerId}`}
                            onMoveUp={this.handleMoveUp}
                            onMoveDown={this.handleMoveDown}
                            onMoveTop={this.handleMoveTop}
                            onMoveBottom={this.handleMoveBottom}
                        />
                    </div>
                </td>
                <td className="align-left">{data.team}</td>
                <td>
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-leave${data.ownerId}`}
                            name="leaves" checked={data.leaves} onChange={this.handleChangePermissionOnRequest}
                        />
                        <label htmlFor={`ckb-leave${data.ownerId}`} />
                    </div>
                </td>
                <td>
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-changeShift${data.ownerId}`}
                            name="changeShifts" checked={data.changeShifts} onChange={this.handleChangePermissionOnRequest}
                        />
                        <label htmlFor={`ckb-changeShift${data.ownerId}`} />
                    </div>
                </td>
                <td>
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-lateInEarlyOut${data.ownerId}`}
                            name="lateInEarlyOuts" checked={data.lateInEarlyOuts} onChange={this.handleChangePermissionOnRequest}
                        />
                        <label htmlFor={`ckb-lateInEarlyOut${data.ownerId}`} />
                    </div>
                </td>
                <td>
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-checkInOut${data.ownerId}`}
                            name="checkInOuts" checked={data.checkInOuts} onChange={this.handleChangePermissionOnRequest}
                        />
                        <label htmlFor={`ckb-checkInOut${data.ownerId}`} />
                    </div>
                </td>
                <td>
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-overtime${data.ownerId}`}
                            name="overtimes" checked={data.overtimes} onChange={this.handleChangePermissionOnRequest}
                        />
                        <label htmlFor={`ckb-overtime${data.ownerId}`} />
                    </div>
                </td>
                <td>
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-keepInView${data.ownerId}`}
                            name="keepInViews" checked={data.keepInViews} onChange={this.handleChangePermissionOnRequest}
                        />
                        <label htmlFor={`ckb-keepInView${data.ownerId}`} />
                    </div>
                </td>
                <td>
                    <a className="fa fa-times fa-lg icon-delete" href="javascript:;"
                        onClick={this.handleRemoveEmployee}
                    />
                </td>
            </tr>
        );
    }
}