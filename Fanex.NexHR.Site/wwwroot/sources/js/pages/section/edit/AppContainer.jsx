import axios from 'axios';
import toastr from 'toastr';
import React, { useEffect, useState } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

const EditSectionSchema = Yup.object({
    code: Yup.string()
        .max(50, 'The Code must be 50 characters!')
        .required('The Code field is required!'),
    name: Yup.string()
        .max(200, 'The Name must be 200 characters!')
        .required('The Name field is required!'),
    department: Yup.string().required('The The Department field is required!')
});

export const AppContainer = () => {
    const url = window.location.href;
    const id = url.substring(url.lastIndexOf('/') + 1);
    const handleGoBack = () => {
        history.back();
    }

    const [data, setData] = useState([])
    const handleGetDataDetail = (id) => {
        axios(resolveClientUrl(`Section/GetById/${id}`)).then(response => {
            const { data } = response;
            if (data) {
                setData(data);
            }

        }).catch(axiosError);
    }

    useEffect(() => {
        if (id) {
            handleGetDataDetail(id)
        }
    }, [id])
    const onSubmit = (data) => {
        if (data) {
            axios.post(resolveClientUrl(`Section/Edit`), {
                SectionId: data.id,
                Code: data.code.trim(),
                Name: data.name.trim(),
                IsActive: data.isActive,
                DepartmentId: data.department
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success('Successfully!', '', toastOptions());
                    setTimeout(function () {
                        document.location = resolveClientUrl("Section/Index");
                    }, 3000);
                } else {
                    toastr.error(data.Message, '', toastOptions());
                }
            }).catch(axiosError);
        }
    }

    const renderForm = (errors) => {
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
                                    data.departments && data.departments.map((item) => (
                                        <option value={item.id} key={item.id} defaultValue={data.departmentId === item.id ? true : false}>{item.name}</option>
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
                        <button type="submit" className="btn btn-primary" id="btnSubmit">Update</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        )
    }
    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            enableReinitialize={true}
            initialValues={{
                id: data.sectionId || '',
                name: data.name || '',
                code: data.code || '',
                isActive: data.isActive || false,
                department: data.departmentId ||'',
            }}
            validationSchema={EditSectionSchema}
            onSubmit={onSubmit}
        >
            {({ errors }) => (
                renderForm(errors)
            )}
        </Formik>
    )
};