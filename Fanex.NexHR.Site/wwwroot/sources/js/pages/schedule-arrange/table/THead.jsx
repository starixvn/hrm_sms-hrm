import React, { PureComponent, Fragment } from 'react';
import moment from 'moment';

export default class THead extends PureComponent {
    dayOfWeeks = [
        { name: 'mondaySchedule', value: 0 },
        { name: 'tuesdaySchedule', value: 1 },
        { name: 'wednesdaySchedule', value: 2 },
        { name: 'thursdaySchedule', value: 3 },
        { name: 'fridaySchedule', value: 4 },
        { name: 'saturdaySchedule', value: 5 },
        { name: 'sundaySchedule', value: 6 }
    ];

    render() {
        const { isHost, fromDate, toDate } = this.props;

        return (
            <Fragment>
                <div className="header-control">
                    {
                        isHost &&
                        <div>
                            <span
                                onClick={this.props.onSave}
                                className="btn save glyphicon glyphicon-floppy-disk"
                                title="Save"
                            />
                            <div className="btn checkAllWeekDays">
                                <div className="checkbox">
                                    <input type="checkbox" className={`checkbox-custom`} id={`ckb-checkAllWeekDays`}
                                        onChange={event => this.props.onChangeSelectWeekDays(null, event.target.checked)}
                                    />
                                    <label htmlFor={`ckb-checkAllWeekDays`}> Check/Uncheck all</label>
                                </div>
                            </div>
                            <span className="btn offNoshift" onClick={this.props.onChangeOffNoShift}>Off No Shift</span>
                            <span className="btn offdayOT" onClick={this.props.onChangeOffdayOT}>Offday OT</span>
                            <span className="btn workday" onClick={this.props.onChangeWorkday}>Workday</span>
                            <span className="btn shiftMultiple" onClick={this.props.onShiftMultiple}>Shift Multiple</span>
                        </div>
                    }
                    <div>
                        <span className="btn prev-week glyphicon glyphicon-chevron-left" onClick={this.props.onPrevious} />
                        <span className="btn next-week glyphicon glyphicon-chevron-right" onClick={this.props.onNext} />
                        <span className="label-week">{`${fromDate} - ${toDate}`}</span>
                    </div>
                </div>
                <div className="header">
                    <div className="table-cell cell-index" />
                    <div className="table-cell cell-section">Dept./Section</div>
                    <div className="table-cell cell-nickname">Nickname</div>
                    <div className="table-cell cell-shift">Default Shift</div>
                    {
                        this.dayOfWeeks.map(item => {
                            return (
                                <div className="table-cell cell-shift" key={`dayOfWeeks${item.value}`}>
                                    {moment(fromDate).add(item.value, 'days').format('D MMM, ddd')}
                                    {
                                        isHost &&
                                        <div className="checkbox">
                                            <input type="checkbox" className={`checkbox-custom`} id={`ckb-${item.name}`}
                                                onChange={event => this.props.onChangeSelectWeekDays(item.name, event.target.checked)}
                                            />
                                            <label htmlFor={`ckb-${item.name}`} />
                                        </div>
                                    }
                                </div>
                            );
                        })
                    }
                    <div className="table-cell cell-public">
                        Public
                            {
                            isHost &&
                            <div className="checkbox">
                                <input type="checkbox" className={`checkbox-custom`} id={`ckb-publicAll`}
                                    onChange={event => this.props.onChangePublics(null, event.target.checked)}
                                />
                                <label htmlFor={`ckb-publicAll`} />
                            </div>
                        }
                    </div>
                    <div className="table-cell cell-modifier">Modified by</div>
                </div>
            </Fragment>
        );
    }
}