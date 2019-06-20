import React, { PureComponent, Fragment } from 'react';
import { Dropdown, MenuItem } from 'react-bootstrap';
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
                    <div>
                        <button className="btn save fa fa-floppy-o"
                            title="Save" onClick={this.props.onSave} disabled={!isHost}
                        />
                        <div className="btn checkAllWeekDays">
                            <div className="checkbox">
                                <input type="checkbox" className={`checkbox-custom`} id={`ckb-checkAllWeekDays`} disabled={!isHost}
                                    onChange={event => this.props.onChangeSelectWeekDays(null, event.target.checked)}
                                />
                                <label htmlFor={`ckb-checkAllWeekDays`}> Check/Uncheck all</label>
                            </div>
                        </div>
                        <button className="btn offNoshift" onClick={this.props.onChangeOffNoShift} disabled={!isHost}>Off No Shift</button>
                        <button className="btn offdayOT" onClick={this.props.onChangeOffdayOT} disabled={!isHost}>Off Day OT</button>
                        <button className="btn workday" onClick={this.props.onChangeWorkday} disabled={!isHost}>Workday</button>
                        <button className="btn shiftMultiple" onClick={this.props.onShiftMultiple} disabled={!isHost}>Shift Multiple</button>
                    </div>
                    <div>
                        <button className="btn prev-week fa fa-chevron-left" onClick={this.props.onPrevious} title="Prev" />
                        <button className="btn next-week fa fa-chevron-right" onClick={this.props.onNext} title="Next" />
                        <span className="label-week">{`${fromDate} - ${toDate}`}</span>
                        <Dropdown id={`dropdownColorDescription`} componentClass="span" className="dropdown-color">
                            <Dropdown.Toggle noCaret className="btn">
                                <img src="../wwwroot/sources/styles/images/pantone.svg" width="17" />
                            </Dropdown.Toggle>
                            <Dropdown.Menu>
                                <MenuItem><span className="holiday" />Public Holiday</MenuItem>
                                <MenuItem><span className="offday" />Off Day</MenuItem>
                                <MenuItem><span className="offdayreplacement" />Off Replacement</MenuItem>
                                <MenuItem><span className="keepinviewday" />Keep In View</MenuItem>
                                <MenuItem><span className="changeshift" />Change Shift</MenuItem>
                                <MenuItem><span className="overtime" />Overtime</MenuItem>
                                <MenuItem><span className="annualleave" />Annual Leave</MenuItem>
                                <MenuItem><span className="unpaidleave" />Unpaid Leave</MenuItem>
                                <MenuItem><span className="businesstrip" />Business Trip</MenuItem>
                                <MenuItem><span className="weddingleave" />Wedding Leave</MenuItem>
                                <MenuItem><span className="leavehalfday2" />2 Leaves Half Day</MenuItem>
                                <MenuItem><span className="leaveday" />Others Leave</MenuItem>
                            </Dropdown.Menu>
                        </Dropdown>
                    </div>
                </div>
                <div className="header">
                    <div className="table-cell cell-index" />
                    <div className="table-cell cell-nickname">Nickname</div>
                    <div className="table-cell cell-shift">Default Shift</div>
                    {
                        this.dayOfWeeks.map(item => {
                            return (
                                <div className="table-cell cell-shift" key={`dayOfWeeks${item.value}`}>
                                    <div className="cell-control-wrapper">
                                        <div>{moment(fromDate).add(item.value, 'days').format('D MMM, ddd')}</div>
                                        <div className="checkbox">
                                            <input type="checkbox" className={`checkbox-custom`} id={`ckb-${item.name}`} disabled={!isHost}
                                                onChange={event => this.props.onChangeSelectWeekDays(item.name, event.target.checked)}
                                            />
                                            <label htmlFor={`ckb-${item.name}`} />
                                        </div>
                                    </div>
                                </div>
                            );
                        })
                    }
                    <div className="table-cell cell-modifier">Modified by</div>
                </div>
            </Fragment>
        );
    }
}