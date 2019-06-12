import React, { PureComponent } from 'react';
import { Dropdown, MenuItem } from 'react-bootstrap';

export default class Row extends PureComponent {
    handleRemoveEmployee = () => {
        const { ownerId } = this.props.data;
        this.props.onRemoveEmployee(ownerId);
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
                        <div>{`${data.nickname} (${data.fullName})`}</div>
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
                <td>
                    <a className="fa fa-times fa-lg icon-delete" href="javascript:;"
                        onClick={this.handleRemoveEmployee}
                    />
                </td>
            </tr>
        );
    }
}