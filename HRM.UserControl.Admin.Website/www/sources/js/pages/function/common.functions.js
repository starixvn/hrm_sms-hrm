import LangHelper from '@jsroot/common/langHelper';

export const validateFormik = values => {
    const errors = {};

    if (!values.functionName || values.functionName.trim().length === 0) {
        errors.functionName = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('FunctionName'));
    } else {
        if (values.functionName.trim().length > 50) {
            errors.functionName = LangHelper.formatString(
                LangHelper.getResource('FieldMaxLength'),
                LangHelper.getResource('FunctionName'),
                50);
        }
    }
    return errors;
};