import React, { Component } from 'react';
import moment from 'moment';

import Row from './Row';

export default class Table extends Component {
    //shouldComponentUpdate(nextProps) {
    //    return this.props.isHost !== nextProps.isHost
    //        || this.props.isSARole !== nextProps.isSARole
    //        || this.props.fromDate !== nextProps.fromDate
    //        || this.props.toDate !== nextProps.toDate
    //        || this.props.shiftOptions !== nextProps.shiftOptions
    //        || this.props.data !== nextProps.data;
    //}

    dayOfWeeks = [
        { name: 'mondaySchedule', value: 0 },
        { name: 'tuesdaySchedule', value: 1 },
        { name: 'wednesdaySchedule', value: 2 },
        { name: 'thursdaySchedule', value: 3 },
        { name: 'fridaySchedule', value: 4 },
        { name: 'saturdaySchedule', value: 5 },
        { name: 'sundaySchedule', value: 6 }
    ];

    handleSave = () => {
    }

    render() {
        console.log('table');
        const totalCols = 15;
        const { isHost, isSARole, fromDate, toDate, shiftOptions, data } = this.props;

        return (
            <table className="table table-custom persist-area">
                <thead className="persist-header">
                    <tr className="header-control">
                        <th colSpan={totalCols}>
                            {
                                isHost &&
                                <div className="left">
                                    <span
                                        onClick={this.handleSave}
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
                                    <span className="btn offNoshift">Off No Shift</span>
                                    <span className="btn offDayOT">Offday OT</span>
                                    <span className="btn workday">Workday</span>
                                    <span className="btn shiftMultiple">Shift Multiple</span>
                                </div>
                            }
                            <div className="right">
                                <span className="btn prev-week glyphicon glyphicon-chevron-left" onClick={this.props.onPrevious} />
                                <span className="btn next-week glyphicon glyphicon-chevron-right" onClick={this.props.onNext} />
                                <span className="label-week">{`${fromDate} - ${toDate}`}</span>
                            </div>
                        </th>
                    </tr>
                    <tr>
                        <th />
                        <th>Dept./Section</th>
                        <th>Nickname</th>
                        <th>Default Shift</th>
                        {
                            this.dayOfWeeks.map(item => {
                                return (
                                    <th key={`dayOfWeeks${item.value}`}>
                                        {moment(fromDate).add(item.value, 'days').format('D MMM, ddd')}
                                        {
                                            isHost &&
                                            <div className="checkbox">
                                                <input type="checkbox" className={`checkbox-custom`} id={`ckb-${item.name}`}
                                                    onChange={event => this.props.onChangeSelectWeekDays(item.name, event.target.checked) }
                                                />
                                                <label htmlFor={`ckb-${item.name}`} />
                                            </div>
                                        }
                                    </th>
                                );
                            })
                        }
                        <th>
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
                        </th>
                        <th>Modified by</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        data.length === 0 &&
                        <tr>
                            <td colSpan={totalCols}>No information available</td>
                        </tr>
                    }
                    {
                        data.length > 0 &&
                        data.map(item => {
                            return (
                                <Row key={item.employeeId}
                                    isHost={isHost}
                                    isSARole={isSARole}
                                    data={item}
                                    shiftOptions={shiftOptions}
                                    onChangeSelectWeekDay={this.props.onChangeSelectWeekDay}
                                    onChangeShift={this.props.onChangeShift}
                                    onChangeDefaultShift={this.props.onChangeDefaultShift}
                                    onChangePublics={this.props.onChangePublics}
                                />
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
}