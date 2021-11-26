import LangHelper from '@jsroot/common/langHelper';

export const validateFormik = (values, siteId) => {
    const errors = {};

    if (!values.functionName || values.functionName.trim().length === 0) {
        errors.functionName = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('FunctionName'));
    } else if(values.functionName.trim().length > 50) {
            errors.functionName = LangHelper.formatString(
            LangHelper.getResource('FieldMaxLength'),
            LangHelper.getResource('FunctionName'),
            50);
    }
    if(!siteId || siteId === null) {
        errors.siteId = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Site'));
    }   

    return errors;
};