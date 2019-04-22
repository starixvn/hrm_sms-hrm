import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip, Dropdown, MenuItem } from 'react-bootstrap';

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
        const { isHost, isOrderMode, draggableProvided, isDragging, shiftOptions } = this.props;
        const { groupId, name, members } = this.props.data;

        return (
            <div className={`${isDragging ? 'dragging' : ''}`}
                ref={draggableProvided.innerRef} {...draggableProvided.draggableProps} {...draggableProvided.dragHandleProps}
            >
                <div className="row-section">
                    <div className="row-section-wrapper">
                        <div>{name}</div>
                        {
                            isOrderMode &&
                            <Dropdown id={`dropdownSection${groupId}`} componentClass="div">
                                <Dropdown.Toggle noCaret useAnchor>
                                    <OverlayTrigger trigger={['hover']} placement={'top'}
                                        overlay={<Tooltip id={`customizeSectionOrder${groupId}`}>Customize order</Tooltip>}
                                    >
                                        <i className="icon icon-sortable" />
                                    </OverlayTrigger>
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
                <DroppableMember isHost={isHost} isOrderMode={isOrderMode}
                    groupId={groupId} data={members}
                    shiftOptions={shiftOptions}
                    showRemarkPopover={this.props.showRemarkPopover}
                    onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                    onChangeShift={this.props.onChangeShift}
                    onChangeDefaultShift={this.props.onChangeDefaultShift}
                    onChangePublics={this.props.onChangePublics}
                    onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                    onUpdateRemark={this.props.onUpdateRemark}
                    onChangeOrder={this.props.onChangeOrder}
                />
            </div>
        );
    }
}