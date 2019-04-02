import React, { Component } from 'react';

export default class WeekPicker extends Component {
    componentDidMount() {
        this.$el = $(this.el);
        this.updateWeekPicker();

        this.$el.on('mousemove', '.ui-datepicker-calendar tr', function () { $(this).find('td a').addClass('ui-state-hover'); });
        this.$el.on('mouseleave', '.ui-datepicker-calendar tr', function () { $(this).find('td a').removeClass('ui-state-hover'); });
        this.$el.hide();

        document.addEventListener('mousedown', this.handleClickOutside);
    }

    shouldComponentUpdate(nextProps) {
        return this.props.fromDate !== nextProps.fromDate || this.props.toDate !== nextProps.toDate;
    }

    componentDidUpdate() {
        this.updateWeekPicker();
    }

    componentWillUnmount() {
        this.$el.datepicker('destroy');
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    fromDate = '';
    toDate = '';

    handleClickOutside = event => {
        if (this.wrapperRef && !this.wrapperRef.contains(event.target)) {
            this.$el.hide();
        }
    }

    selectCurrentWeek = (vm) => {
        window.setTimeout(function () {
            vm.$el.find('.ui-datepicker-current-day a').addClass('ui-state-active');
        }, 1);
    };

    updateWeekPicker = () => {
        let vm = this;

        this.$el.datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            dateFormat: 'dd M, yy',
            firstDay: 1,
            onSelect: function (dateText, inst) {
                let date = $(this).datepicker('getDate');
                vm.fromDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 1);
                vm.toDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 7);
                const dateFormat = inst.settings.dateFormat || $.datepicker._defaults.dateFormat;

                vm.props.onChange(
                    $.datepicker.formatDate(dateFormat, vm.fromDate, inst.settings),
                    $.datepicker.formatDate(dateFormat, vm.toDate, inst.settings)
                );
                vm.selectCurrentWeek(vm);
                vm.$el.hide();
            },
            beforeShow: function () {
                vm.selectCurrentWeek(vm);
            },
            beforeShowDay: function (date) {
                let cssClass = '';
                if (date >= vm.fromDate && date <= vm.toDate) {
                    cssClass = 'ui-datepicker-current-day';
                }

                vm.selectCurrentWeek(vm);

                return [true, cssClass];
            },
            onChangeMonthYear: function (year, month, inst) {
                vm.selectCurrentWeek(vm);
            }
        });
    }

    handleShowCalendar = () => {
        this.$el.toggle();
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
                <i className="icon-calendar glyphicon glyphicon-calendar" onClick={this.handleShowCalendar} />
            </div>
        );
    }
}