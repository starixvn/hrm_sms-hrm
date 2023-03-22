import LangHelper from '@jsroot/common/langHelper';

export const validateFormik = values => {
    const errors = {};

    if (!values.username || values.username.trim().length === 0) {
        errors.username = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Username'));
    } else {
        if (values.username.trim().length > 50) {
            errors.username = LangHelper.formatString(
                LangHelper.getResource('FieldMaxLength'),
                LangHelper.getResource('Username'),
                50);
        }
    }

    if (!values.password || values.password.trim().length === 0) {
        errors.password = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Password'));
    } else {
        if (values.password.trim().length > 50) {
            errors.password = LangHelper.formatString(
                LangHelper.getResource('FieldMaxLength'),
                LangHelper.getResource('Password'),
                50);
        }
    }

    if (values.nickname && values.nickname.trim().length > 50) {
        errors.nickname = LangHelper.formatString(
            LangHelper.getResource('FieldMaxLength'),
            LangHelper.getResource('Nickname'),
            50);
    }

    if (values.fullName && values.fullName.trim().length > 100) {
        errors.fullName = LangHelper.formatString(
            LangHelper.getResource('FieldMaxLength'),
            LangHelper.getResource('FullName'),
            100);
    }

    if (values.email) {
        if (values.email.trim().length > 50) {
            errors.email = LangHelper.formatString(
                LangHelper.getResource('FieldMaxLength'),
                LangHelper.getResource('Email'),
                50);
        }

        if (!/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i.test(values.email.trim())) {
            errors.email = 'Email is invalid.';
        }
    }

    return errors;
};