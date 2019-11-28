import React, { PureComponent } from 'react';

import DroppableMember from './DroppableMember';
import DropdownOrder from './DropdownOrder';

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

    handleChangePublic = event => {
        const { groupId } = this.props.data;
        this.props.onChangePublics(groupId, event.target.checked);
    }

    render() {
        const { isHost, draggableProvided, isDragging, shiftOptions } = this.props;
        const { groupId, name, isPublic, members } = this.props.data;

        return (
            <div className={`${isDragging ? 'dragging' : ''}`}
                ref={draggableProvided.innerRef} {...draggableProvided.draggableProps} {...draggableProvided.dragHandleProps}
            >
                <div className="row-section">
                    <div className="section-name">
                        <div>{name}</div>
                        {
                            isHost &&
                            <DropdownOrder id={`dropdownSection${groupId}`}
                                onMoveUp={this.handleMoveUp}
                                onMoveDown={this.handleMoveDown}
                                onMoveTop={this.handleMoveTop}
                                onMoveBottom={this.handleMoveBottom}
                            />
                        }
                    </div>
                    <div className="publish">
                        <div className="checkbox">
                            <input type="checkbox" className={`checkbox-custom`} id={`ckb-publish${groupId}`}
                                checked={isPublic} disabled={!isHost} onChange={this.handleChangePublic}
                            />
                            <label htmlFor={`ckb-publish${groupId}`}> Publish</label>
                        </div>
                    </div>
                </div>
                <DroppableMember isHost={isHost}
                    groupId={groupId} data={members}
                    shiftOptions={shiftOptions}
                    showRemarkPopover={this.props.showRemarkPopover}
                    onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                    onChangeShift={this.props.onChangeShift}
                    onChangeDefaultShift={this.props.onChangeDefaultShift}
                    onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                    onShowWeeklyDefaultShift={this.props.onShowWeeklyDefaultShift}
                    onUpdateRemark={this.props.onUpdateRemark}
                    onChangeOrder={this.props.onChangeOrder}
                />
            </div>
        );
    }
}