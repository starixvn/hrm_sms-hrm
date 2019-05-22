import React, { PureComponent } from 'react';
import { Dropdown, MenuItem } from 'react-bootstrap';

import DroppableMember from './DroppableMember';

export default class RowSection extends PureComponent {
    handleMoveUp = () => {
        const { groupId } = this.props.data;
        this.props.onChangeSectionOrder('up', groupId);
    }

    handleMoveDown = () => {
        const { groupId } = this.props.data;
        this.props.onChangeSectionOrder('down', groupId);
    }

    handleMoveTop = () => {
        const { groupId } = this.props.data;
        this.props.onChangeSectionOrder('top', groupId);
    }

    handleMoveBottom = () => {
        const { groupId } = this.props.data;
        this.props.onChangeSectionOrder('bottom', groupId);
    }

    render() {
        const { isHost, draggableProvided, isDragging, shiftOptions } = this.props;
        const { groupId, name, members } = this.props.data;

        return (
            <div className={`${isDragging ? 'dragging' : ''}`}
                ref={draggableProvided.innerRef} {...draggableProvided.draggableProps} {...draggableProvided.dragHandleProps}
            >
                <div className="row-section">
                    <div className="row-section-wrapper">
                        <div>{name}</div>
                        {
                            isHost &&
                            <Dropdown id={`dropdownSection${groupId}`} componentClass="div" className="customize-order">
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
                        }
                    </div>
                </div>
                <DroppableMember isHost={isHost}
                    groupId={groupId} data={members}
                    shiftOptions={shiftOptions}
                    showRemarkPopover={this.props.showRemarkPopover}
                    onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                    onChangeShift={this.props.onChangeShift}
                    onChangeDefaultShift={this.props.onChangeDefaultShift}
                    onChangePublics={this.props.onChangePublics}
                    onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                    onShowWeeklyDefaultShift={this.props.onShowWeeklyDefaultShift}
                    onUpdateRemark={this.props.onUpdateRemark}
                    onChangeOrder={this.props.onChangeOrder}
                />
            </div>
        );
    }
}