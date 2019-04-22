import React, { Component } from 'react';
import { Droppable, Draggable } from 'react-beautiful-dnd';

import RowSection from './RowSection';

export default class DroppableSection extends Component {
    render() {
        const { isHost, isOrderMode, shiftOptions, data } = this.props;

        return (
            <Droppable droppableId="droppable" isDropDisabled={!isOrderMode} type="droppableSection">
                {
                    (provided, snapshot) =>
                        <div ref={provided.innerRef} {...provided.droppableProps}>
                            {
                                data.map((section, gindex) =>
                                    <Draggable key={`section${section.groupId}`} draggableId={`section${section.groupId}`}
                                        index={gindex} isDragDisabled={!isOrderMode}
                                    >
                                        {
                                            (provided, snapshot) =>
                                                <div>
                                                    <RowSection draggableProvided={provided}
                                                        isHost={isHost} isOrderMode={isOrderMode} data={section}
                                                        shiftOptions={shiftOptions} isDragging={snapshot.isDragging}
                                                        showRemarkPopover={this.props.showRemarkPopover}
                                                        onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                                                        onChangeShift={this.props.onChangeShift}
                                                        onChangeDefaultShift={this.props.onChangeDefaultShift}
                                                        onChangePublics={this.props.onChangePublics}
                                                        onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                                                        onUpdateRemark={this.props.onUpdateRemark}
                                                        onChangeSectionOrder={this.props.onChangeSectionOrder}
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