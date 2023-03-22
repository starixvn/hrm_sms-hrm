import LangHelper from '@jsroot/common/langHelper';

export const validateFormik = values => {
    const errors = {};

    if (!values.name || values.name.trim().length === 0) {
        errors.name = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Name'));
    } else {
        if (values.name.trim().length > 50) {
            errors.name = LangHelper.formatString(
                LangHelper.getResource('FieldMaxLength'),
                LangHelper.getResource('Name'),
                50);
        }
    }

    if (!values.url || values.url.trim().length === 0) {
        errors.url = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('URL'));
    } else {
        if (values.url.trim().length > 100) {
            errors.url = LangHelper.formatString(
                LangHelper.getResource('FieldMaxLength'),
                LangHelper.getResource('URL'),
                100);
        }
    }

    if (!values.company) {
        errors.company = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Company'));
    }

    return errors;
};