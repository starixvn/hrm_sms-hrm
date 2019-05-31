import React, { Component } from 'react';
import moment from 'moment';

import { DateFormat } from '../constants';

export default class WeekPicker extends Component {
    componentDidMount() {
        this.$el = $(this.el);
        this.updateWeekPicker();

        this.$el.on('mousemove', '.ui-datepicker-calendar tr', function () { $(this).find('td a').addClass('ui-state-hover'); });
        this.$el.on('mouseleave', '.ui-datepicker-calendar tr', function () { $(this).find('td a').removeClass('ui-state-hover'); });

        document.addEventListener('mousedown', this.handleClickOutside);
    }

    shouldComponentUpdate(nextProps) {
        return this.props.fromDate !== nextProps.fromDate
            || this.props.toDate !== nextProps.toDate;
    }

    componentDidUpdate() {
        this.updateWeekPicker();
    }

    componentWillUnmount() {
        this.$el.weekpicker('destroy');
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    handleClickOutside = event => {
        if (this.wrapperRef && !this.wrapperRef.contains(event.target)) {
            this.$el.hide();
        }
    }

    updateWeekPicker = () => {
        let vm = this;

        this.$el.weekpicker({
            firstDay: 1,
            dateFormat: 'dd M, yy',
            currentText: vm.props.fromDate,
            onSelect: function (dateText, startDateText, startDate, endDate, inst) {
                vm.props.onChange(moment(startDate).format(DateFormat), moment(endDate).format(DateFormat));
                vm.$el.hide();
            }
        });
    }

    handleShowCalendar = () => {
        if (!this.props.isDisabled) {
            this.$el.toggle();
        }
    }

    render() {
        return (
            <div className="weekpicker-wrapper" ref={node => this.wrapperRef = node}>
                <input
                    value={this.props.fromDate}
                    onClick={this.handleShowCalendar}
                    className="fromDate form-control" readOnly
                />
                <input value={this.props.toDate}
                    onClick={this.handleShowCalendar}
                    className="toDate form-control" readOnly
                />
                <div className="calendar" ref={node => this.el = node} />
                <i className="icon-calendar fa fa-calendar" onClick={this.handleShowCalendar} />
            </div>
        );
    }
}