import React, { useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { validationSchema } from '../validate/common.functions';

const AppContainer = () => {
    const handleGoBack = () => {
        history.back();
    };

    const [department, setDepartment] = useState([]);

    useEffect(() => {
        axios(resolveClientUrl(`Department/getById/${window.departmentId}`)).then(response => {
            setDepartment(response.data);
        }).catch(axiosError);
    }, []);

    const hanldeSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);

        axios.post(resolveClientUrl('Department/Edit'), {
            departmentId: values.id,
            code: values.code.trim(),
            name: values.name.trim(),
            isActive: values.isActive,
            unitId: values.unit
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('Department/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
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
                            {errors.unit && <li>{errors.unit}</li>}
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
                            <label className="control-label">Unit</label>
                        </div>
                        <div className="select-form">
                            <Field name="unit" as="select" className="form-control">
                                {data.units ? data.units.map((unit) => (
                                    <option key={unit.id} value={unit.id}>
                                        {unit.name}
                                    </option>
                                )) : ''}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Is Active</label>
                        </div>
                        <div className="input-form">
                            <Field type="checkbox" name="isActive" style={{ cursor: 'pointer' }} />
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

    if (department === null) {
        return null;
    }

    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            enableReinitialize={true}
            initialValues={{
                id: department.departmentId || '',
                name: department.name || '',
                code: department.code || '',
                isActive: department.isActive || false,
                unit: department.unitId || ''
            }}
            validationSchema={validationSchema}
            onSubmit={hanldeSubmit}
        >
            {renderForm}
        </Formik>
    );
};

export default AppContainer;