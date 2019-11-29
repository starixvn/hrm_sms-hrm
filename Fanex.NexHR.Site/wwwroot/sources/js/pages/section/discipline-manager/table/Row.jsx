import React, { PureComponent } from 'react';

import DropdownOrder from '../../../../shared/components/DropdownOrder';

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
                        <DropdownOrder id={`dropdown${data.ownerId}`}
                            onMoveUp={this.handleMoveUp}
                            onMoveDown={this.handleMoveDown}
                            onMoveTop={this.handleMoveTop}
                            onMoveBottom={this.handleMoveBottom}
                        />
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