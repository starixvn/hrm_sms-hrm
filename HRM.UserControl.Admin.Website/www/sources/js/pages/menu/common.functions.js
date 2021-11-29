import LangHelper from '@jsroot/common/langHelper';

export const validateFormik = (values, siteId) => {
    const errors = {};

    if (!values.menuItemName || values.menuItemName.trim().length === 0) {
        errors.menuItemName = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Name'));
    } else if(values.menuItemName.trim().length > 50) {
            errors.menuItemName = LangHelper.formatString(
            LangHelper.getResource('FieldMaxLength'),
            LangHelper.getResource('Name'),
            50);
    }
    if(!siteId || siteId === null) {
        errors.siteId = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Site'));
    }   

    return errors;
};