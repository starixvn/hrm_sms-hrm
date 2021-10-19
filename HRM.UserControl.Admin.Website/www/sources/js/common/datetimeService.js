import moment from 'moment';

const ShortDateFormat = 'MMM DD, YYYY';
const ShortDateTimeFormat = 'MMM DD, YYYY hh:mm a';
const LongDateTimeFormat = 'MMM DD, YYYY hh:mm:ss a';

export default class DateTimeService {
    static startOfDay(date) {
        return moment(date).startOf('day');
    }

    static endOfDay(date) {
        return moment(date).endOf('day');
    }

    static firstDayOfWeek(date) {
        return moment(date).startOf('week');
    }

    static lastDayOfWeek(date) {
        return moment(date).endOf('week');
    }

    static firstDayOfMonth(date) {
        return moment(date).startOf('month');
    }

    static lastDayOfMonth(date) {
        return moment(date).endOf('month');
    }

    static toShortDateFormat(date) {
        return moment(date).format(ShortDateFormat);
    }

    static toShortDateTimeFormat(date) {
        return moment(date).format(ShortDateTimeFormat);
    }

    static toLongDateTimeFormat(date) {
        return moment(date).format(LongDateTimeFormat);
    }

    static isAfterDay(date1, date2) {
        return moment(date1).isAfter(date2, 'day');
    }
}