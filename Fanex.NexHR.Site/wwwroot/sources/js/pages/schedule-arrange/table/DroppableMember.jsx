import React, { PureComponent } from 'react';
import { Droppable, Draggable } from 'react-beautiful-dnd';

import Row from './Row';

export default class DroppableMember extends PureComponent {
    render() {
        const { isHost, shiftOptions, groupId, data } = this.props;

        return (
            <Droppable droppableId={`dropable${groupId}`} isDropDisabled={!isHost} type={`droppableMember-${groupId}`}>
                {
                    provided =>
                        <div ref={provided.innerRef} {...provided.droppableProps}>
                            {
                                data.map((member, eindex) =>
                                    <Draggable key={`member${member.employeeId}`} draggableId={`member${member.employeeId}`}
                                        index={eindex} isDragDisabled={!isHost}
                                    >
                                        {
                                            (memberProvided, memberSnapshot) =>
                                                <div>
                                                    <Row draggableProvided={memberProvided}
                                                        isHost={isHost} groupId={groupId} data={member}
                                                        shiftOptions={shiftOptions} isDragging={memberSnapshot.isDragging}
                                                        showRemarkPopover={this.props.showRemarkPopover}
                                                        onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                                                        onChangeShift={this.props.onChangeShift}
                                                        onChangeDefaultShift={this.props.onChangeDefaultShift}
                                                        onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                                                        onShowWeeklyDefaultShift={this.props.onShowWeeklyDefaultShift}
                                                        onUpdateRemark={this.props.onUpdateRemark}
                                                        onChangeOrder={this.props.onChangeOrder}
                                                    />
                                                    {memberProvided.placeholder}
                                                </div>
                                        }
                                    </Draggable>
                                )
                            }
                            {provided.placeholder}
                        </div>
                }
            </Droppable>
        );
    }
}