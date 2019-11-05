import React, { PureComponent } from 'react';
import { Droppable, Draggable } from 'react-beautiful-dnd';

import RowSection from './RowSection';

export default class DroppableSection extends PureComponent {
    render() {
        const { isHost, shiftOptions, data } = this.props;

        return (
            <Droppable droppableId="droppable" isDropDisabled={!isHost} type="droppableSection">
                {
                    provided =>
                        <div ref={provided.innerRef} {...provided.droppableProps}>
                            {
                                data.map((section, gindex) =>
                                    <Draggable key={`section${section.groupId}`} draggableId={`section${section.groupId}`}
                                        index={gindex} isDragDisabled={!isHost}
                                    >
                                        {
                                            (sectionProvided, sectionSnapshot) =>
                                                <div>
                                                    <RowSection draggableProvided={sectionProvided}
                                                        isHost={isHost} data={section}
                                                        shiftOptions={shiftOptions} isDragging={sectionSnapshot.isDragging}
                                                        showRemarkPopover={this.props.showRemarkPopover}
                                                        onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                                                        onChangeShift={this.props.onChangeShift}
                                                        onChangeDefaultShift={this.props.onChangeDefaultShift}
                                                        onChangePublics={this.props.onChangePublics}
                                                        onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                                                        onShowWeeklyDefaultShift={this.props.onShowWeeklyDefaultShift}
                                                        onUpdateRemark={this.props.onUpdateRemark}
                                                        onChangeSectionOrder={this.props.onChangeSectionOrder}
                                                        onChangeOrder={this.props.onChangeOrder}
                                                    />
                                                    {sectionProvided.placeholder}
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