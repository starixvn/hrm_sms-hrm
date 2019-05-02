import React, { PureComponent } from 'react';
import { Droppable, Draggable } from 'react-beautiful-dnd';

import Row from './Row';

export default class DroppableMember extends PureComponent {
    render() {
        const { isHost, shiftOptions, groupId, data } = this.props;

        return (
            <Droppable droppableId={`dropable${groupId}`} isDropDisabled={!isHost} type={`droppableMember-${groupId}`}>
                {
                    (provided, snapshot) =>
                        <div ref={provided.innerRef} {...provided.droppableProps}>
                            {
                                data.map((member, eindex) =>
                                    <Draggable key={`member${member.employeeId}`} draggableId={`member${member.employeeId}`}
                                        index={eindex} isDragDisabled={!isHost}
                                    >
                                        {
                                            (provided, snapshot) =>
                                                <div>
                                                    <Row draggableProvided={provided}
                                                        isHost={isHost} groupId={groupId} data={member}
                                                        shiftOptions={shiftOptions} isDragging={snapshot.isDragging}
                                                        showRemarkPopover={this.props.showRemarkPopover}
                                                        onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                                                        onChangeShift={this.props.onChangeShift}
                                                        onChangeDefaultShift={this.props.onChangeDefaultShift}
                                                        onChangePublics={this.props.onChangePublics}
                                                        onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                                                        onUpdateRemark={this.props.onUpdateRemark}
                                                        onChangeOrder={this.props.onChangeOrder}
                                                    />
                                                    {provided.placeholder}
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