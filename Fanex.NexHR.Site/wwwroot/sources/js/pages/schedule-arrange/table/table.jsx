import React, { Component } from 'react';
import moment from 'moment';

export default class Table extends Component {
    handleSave = () => {
    }

    render() {
        const dayOfWeeks = [0, 1, 2, 3, 4, 5, 6];
        const { isHost, isSARole, fromDate, toDate } = this.props;

        return (
            <table className="table table-custom persist-area">
                <thead className="persist-header">
                    <tr className="header-control">
                        <th colSpan="15">
                            {
                                isHost &&
                                <div className="left">
                                    <span
                                        onClick={this.handleSave}
                                        className="btn save glyphicon glyphicon-floppy-disk"
                                        title="Save"
                                    />
                                </div>
                            }
                            <div className="right">
                                <span className="btn prev-week glyphicon glyphicon-chevron-left" />
                                <span className="btn next-week glyphicon glyphicon-chevron-right" />
                                <span className="label-week">{`${fromDate} - ${toDate}`}</span>
                            </div>
                        </th>
                    </tr>
                    <tr>
                        {isHost && <th />}
                        <th>Dept./Section</th>
                        <th>Nickname</th>
                        <th>Default Shift</th>
                        {
                            dayOfWeeks.map(i => {
                                return (
                                    <th key={`dayOfWeeks${i}`}>
                                        {moment(fromDate).add(i, 'days').format('D MMM, ddd')}
                                        {
                                            isHost &&
                                            <input className="right" type="checkbox" />
                                        }
                                    </th>
                                );
                            })
                        }
                        <th>Public</th>
                        <th>Modified by</th>
                    </tr>
                </thead>
            </table>
        );
    }
}