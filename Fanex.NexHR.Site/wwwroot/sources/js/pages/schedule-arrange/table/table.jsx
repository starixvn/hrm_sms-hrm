import React, { PureComponent, Fragment } from 'react';
import { DragDropContext } from 'react-beautiful-dnd';

import THead from './THead';
import DroppableSection from './DroppableSection';

export default class Table extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            isFixedHeader: false
        };

        this.contentScrollElement = document.getElementsByClassName('content_scroll')[0];
    }

    componentDidMount() {
        this.contentScrollElement.addEventListener('scroll', this.handleScroll);
    }

    componentWillUnmount() {
        this.contentScrollElement.removeEventListener('scroll', this.handleScroll);
    }

    handleScroll = event => {
        let scrollTop = event.srcElement.scrollTop;

        if (scrollTop > 70) {
            if (!this.state.isFixedHeader) {
                this.setState({ isFixedHeader: true });
            }
        } else {
            if (this.state.isFixedHeader) {
                this.setState({ isFixedHeader: false });
            }
        }
    }

    render() {
        const { isHost, fromDate, toDate, shiftOptions, data } = this.props;

        return (
            <Fragment>
                <div className="table-wrapper">
                    <div className={`table-header ${this.state.isFixedHeader ? 'fixed' : ''}`}>
                        <THead fromDate={fromDate} toDate={toDate} isHost={isHost}
                            onSave={this.props.onSave}
                            onChangeSelectWeekDays={this.props.onChangeSelectWeekDays}
                            onChangeOffNoShift={this.props.onChangeOffNoShift}
                            onChangeOffdayOT={this.props.onChangeOffdayOT}
                            onChangeWorkday={this.props.onChangeWorkday}
                            onShiftMultiple={this.props.onShiftMultiple}
                            onChangePublics={this.props.onChangePublics}
                            onPrevious={this.props.onPrevious}
                            onNext={this.props.onNext}
                        />
                    </div>
                    <div className="table-body">
                        {
                            data.length === 0 &&
                            <div className="nodata">
                                No information available
                            </div>
                        }
                        {
                            data.length > 0 &&
                            <DragDropContext onDragEnd={this.props.onDragEnd}>
                                <DroppableSection isHost={isHost} data={data}
                                    shiftOptions={shiftOptions}
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
                            </DragDropContext>
                        }
                    </div>
                </div>
            </Fragment>
        );
    }
}