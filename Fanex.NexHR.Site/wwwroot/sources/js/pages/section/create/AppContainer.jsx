import axios from 'axios';
import toastr from 'toastr';
import React, { useEffect, useState } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';


const CreateSectionSchema = Yup.object({
    code: Yup.string()
        .max(50, 'Code must be 50 characters')
        .required('Code is required'),
    name: Yup.string()
        .max(200, 'Name must be 200 characters')
        .required('Name is required'),
    department: Yup.string().required('Deparment is required')
});
export const AppContainer = () => {
    const [deparments, setDeparments] = useState([]);
    const initialValues = {
        code: '',
        name: '',
        isActive: false,
        department : ''
    }
    const handleGoBack = () => {
        history.back();
    }

    const handleGetDepartment = () => {
        axios(resolveClientUrl('Section/GetDeparment')).then(response => {
            const { data } = response;
            if (data) {
                setDeparments(data);
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetDepartment();
    },[])

    const onSubmit = (data) => {
        if (data) {
            const name = data.name.trim();
            const code = data.code.trim();
            axios.post(resolveClientUrl('Section/Create'), {
                Code: code,
                Name: name,
                IsActive: data.isActive,
                DepartmentId: data.department
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
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
                                    deparments && deparments.map((item, i) => (
                                        <option value={item.id} key={item.id}>{item.name}</option>
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
                        <button type="submit" className="btn btn-primary" id="btnSubmit">Add</button>
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
            initialValues={initialValues}
            validationSchema={CreateSectionSchema}
            onSubmit={onSubmit}
        >
            {({ errors }) => (
                renderForm(errors)
            )}
        </Formik>

    )
}