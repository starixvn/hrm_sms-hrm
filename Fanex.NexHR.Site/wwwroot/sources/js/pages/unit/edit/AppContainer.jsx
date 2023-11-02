import React, { useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { validationSchema } from '../validate';

const AppContainer = () => {
    const [unit, setUnit] = useState(null);
    const handleGoBack = () => {
        history.back();
    };

    useEffect(() => {
        axios(resolveClientUrl(`Unit/getById/${window.unitId}`)).then(response => {
            setUnit(response.data);
        }).catch(axiosError);
    }, []);

    const hanldeSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios.post(resolveClientUrl('Unit/Edit'), {
            unitId: values.id,
            code: values.code.trim(),
            name: values.name.trim(),
            isActive: values.isActive
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('Unit/Index');
                }, 3000);
            } else {
                toastr.error(data.Message, '', toastOptions());
                setSubmitting(false);
            }
        }).catch(axiosError);
    };

    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <div className="card">
                {
                    !isEmpty(errors) && <div className="validation-summary-errors">
                        <ul>
                            {errors.code && <li>{errors.code}</li>}
                            {errors.name && <li>{errors.name}</li>}
                        </ul>
                    </div>
                }
                <Form className="form-horizontal">
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Code</label>
                        </div>
                        <div className="input-form">
                            <Field name="code" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Name</label>
                        </div>
                        <div className="input-form">
                            <Field name="name" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Is Active</label>
                        </div>
                        <div className="input-form">
                            <Field type="checkbox" name="isActive" />
                        </div>
                    </div>
                    <div className="form-group">
                        <button type="submit" className="btn btn-primary" id="btnSubmit" disabled={isSubmitting}>Update</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    };

    if (unit === null) {
        return null;
    }

    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            enableReinitialize={true}
            initialValues={{
                id: unit.id || '',
                name: unit.name || '',
                code: unit.code || '',
                isActive: unit.isActive || false
            }}
            validationSchema={ validationSchema }
            onSubmit={hanldeSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;