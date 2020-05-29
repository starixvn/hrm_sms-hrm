import React, { PureComponent } from 'react';
import { Droppable, Draggable } from 'react-beautiful-dnd';

import RowTeam from './RowTeam';

export default class DroppableTeam extends PureComponent {
    render() {
        const { isHost, shiftOptions, data } = this.props;

        return (
            <Droppable droppableId="droppable" isDropDisabled={!isHost} type="droppableTeam">
                {
                    provided =>
                        <div ref={provided.innerRef} {...provided.droppableProps}>
                            {
                                data.map((team, gindex) =>
                                    <Draggable key={`team{team.teamId}`} draggableId={`team{team.teamId}`}
                                        index={gindex} isDragDisabled={!isHost}
                                    >
                                        {
                                            (teamProvided, teamSnapshot) =>
                                                <div>
                                                    <RowTeam draggableProvided={teamProvided}
                                                        isHost={isHost} data={team}
                                                        shiftOptions={shiftOptions} isDragging={teamSnapshot.isDragging}
                                                        showRemarkPopover={this.props.showRemarkPopover}
                                                        onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                                                        onChangeShift={this.props.onChangeShift}
                                                        onChangeDefaultShift={this.props.onChangeDefaultShift}
                                                        onChangePublics={this.props.onChangePublics}
                                                        onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                                                        onShowWeeklyDefaultShift={this.props.onShowWeeklyDefaultShift}
                                                        onUpdateRemark={this.props.onUpdateRemark}
                                                        onChangeTeamOrder={this.props.onChangeteamOrder}
                                                        onChangeOrder={this.props.onChangeOrder}
                                                    />
                                                    {teamProvided.placeholder}
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