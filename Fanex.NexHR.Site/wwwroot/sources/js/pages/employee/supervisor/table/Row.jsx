import React, { PureComponent } from 'react';
import { Dropdown, MenuItem } from 'react-bootstrap';

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
                        <Dropdown id={`dropdown${data.ownerId}`} componentClass="div" className="customize-order">
                            <Dropdown.Toggle noCaret useAnchor>
                                <i className="icon icon-sortable" title="Customize order" />
                            </Dropdown.Toggle>
                            <Dropdown.Menu>
                                <MenuItem onClick={this.handleMoveUp}>Up</MenuItem>
                                <MenuItem onClick={this.handleMoveDown}>Down</MenuItem>
                                <MenuItem onClick={this.handleMoveTop}>Top</MenuItem>
                                <MenuItem onClick={this.handleMoveBottom}>Bottom</MenuItem>
                            </Dropdown.Menu>
                        </Dropdown>
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