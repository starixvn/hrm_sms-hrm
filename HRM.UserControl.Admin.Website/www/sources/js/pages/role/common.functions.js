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

    if (!values.site) {
        errors.site = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Site'));
    }

    return errors;
};