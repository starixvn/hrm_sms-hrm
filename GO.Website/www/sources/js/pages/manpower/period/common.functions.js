import LangHelper from '@jsroot/common/langHelper';

export const validateFormik = values => {
    const errors = {};

    if (!values.name || values.name.trim().length === 0) {
        errors.name = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Name'));
    } else if (values.name.trim().length > 100) {
        errors.name = LangHelper.formatString(
            LangHelper.getResource('FieldMaxLength'),
            LangHelper.getResource('Name'),
            100);
    }

    if (!values.toDateUnitHead || !values.fromDateUnitHead) {
        errors.unitHead = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('UnitHead'));
    }
    else if (values.fromDateUnitHead > values.toDateUnitHead ) {
        errors.unitHead = LangHelper.formatString(
            LangHelper.getResource('ErrorDateRange'),
            LangHelper.getResource('UnitHead'));
    }

    if (!values.fromDateVerifier || !values.toDateVerifier) {
        errors.verifier = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Verifier'));
    }
    else if (values.fromDateVerifier > values.toDateVerifier) {
        errors.verifier = LangHelper.formatString(
            LangHelper.getResource('ErrorDateRange'),
            LangHelper.getResource('Verifier'));
    }

    if (!values.fromDateApprover || !values.toDateApprover) {
        errors.approver = LangHelper.formatString(
            LangHelper.getResource('FieldRequired'),
            LangHelper.getResource('Approver'));
    }
    else if (values.fromDateApprover > values.toDateApprover) {
        errors.approver = LangHelper.formatString(
            LangHelper.getResource('ErrorDateRange'),
            LangHelper.getResource('Approver'));
    }

    return errors;
};