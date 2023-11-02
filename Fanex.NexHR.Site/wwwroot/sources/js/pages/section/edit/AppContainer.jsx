import axios from 'axios';
import toastr from 'toastr';
import React, { useEffect, useState } from 'react';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { validationSchema } from '../common.functions';



const AppContainer = () => {
    const [section, setSection] = useState(null)
    const handleGoBack = () => {
        history.back();
    }
    useEffect(() => {
        axios(resolveClientUrl(`Section/GetById/${window.sectionId}`)).then(response => {
            setSection(response.data);
        }).catch(axiosError);
    }, [])
    const onSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        axios.post(resolveClientUrl('Section/Edit'), {
            SectionId: values.id,
            Code: values.code.trim(),
            Name: values.name.trim(),
            IsActive: values.isActive,
            DepartmentId: values.department
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('Section/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        }).catch(axiosError);
    }
    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <div className="card">
                {
                    !isEmpty(errors) && <div className="validation-summary-errors">
                        <ul>
                            {errors.code && <li>{errors.code}</li>}
                            {errors.name && <li>{errors.name}</li>}
                            {errors.department && <li>{errors.department}</li>}
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
                            <label className="control-label">Department</label>
                        </div>
                        <div className="input-form">
                            <Field name="department" as="select" className="form-control">
                                <option defaultChecked></option>
                                {
                                    section.departments.map((item) => (
                                        <option value={item.id} key={item.id} defaultValue={section.departmentId === item.id}>{item.name}</option>
                                    ))
                                }
                            </Field>
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
        )
    }
    if (section === null) {
        return null;
    }
    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            enableReinitialize={true}
            initialValues={{
                id: section.sectionId || '',
                name: section.name || '',
                code: section.code || '',
                isActive: section.isActive || false,
                department: section.departmentId ||'',
            }}
            validationSchema={validationSchema}
            onSubmit={onSubmit}
        >
            {renderForm}
        </Formik>
    )
};

export default AppContainer;