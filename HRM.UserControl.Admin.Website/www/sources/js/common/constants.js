import LangHelper from './langHelper';

export const PAGING_SIZES = [100, 200, 300, 400, 500];

export const DefaultPageSize = 100;

export const DateFormat = 'MM/dd/yyyy';

export const DateTimeFormat = 'MM/dd/yyyy hh:mm:ss a';

export const TimeFormat = 'hh:mm:ss a';

export const ActiveStatusOptions = [
    { value: null, label: LangHelper.getResource('All') },
    { value: true, label: LangHelper.getResource('Active') },
    { value: false, label: LangHelper.getResource('Inactive') }
];

export const PolicyOptions = [
    { value: null, label: LangHelper.getResource('All') },
    { value: 1, label: LangHelper.getResource('Read') },
    { value: 2, label: LangHelper.getResource('Host') }
];

export const Policy = {
    All: 0,
    Read: 1,
    Host: 2
};