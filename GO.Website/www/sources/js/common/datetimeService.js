import * as dayjs from 'dayjs';

const ShortDateFormat = 'MMM DD, YYYY';
const ShortDateTimeFormat = 'MMM DD, YYYY hh:mm A';
const LongDateTimeFormat = 'MMM DD, YYYY hh:mm:ss A';

export default class DateTimeService {
    static toShortDateFormat(date) {
        return dayjs(date).format(ShortDateFormat);
    }

    static toShortDateTimeFormat(date) {
        return dayjs(date).format(ShortDateTimeFormat);
    }

    static toLongDateTimeFormat(date) {
        return dayjs(date).format(LongDateTimeFormat);
    }
}