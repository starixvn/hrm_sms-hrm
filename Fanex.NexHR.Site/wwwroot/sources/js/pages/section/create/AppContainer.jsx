import axios from 'axios';
import toastr from 'toastr';
import React, { useEffect, useState } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';


const CreateSectionSchema = Yup.object({
    code: Yup.string()
        .trim()
        .max(50, 'The Code must be 50 characters!')
        .required('The Code field is required!'),
    name: Yup.string()
        .trim()
        .max(200, 'The Name must be 200 characters!')
        .required('The Name field is required!'),
    department: Yup.string().required('The Deparment field is required!')
});
const AppContainer = () => {
    const [deparments, setDeparments] = useState([]);
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

    const onSubmit = data => {
        if (data) {
            axios.post(resolveClientUrl('Section/Create'), {
                Code: data.name.trim(),
                Name: data.code.trim(),
                IsActive: data.isActive,
                DepartmentId: data.department
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success('Successfully!', '', toastOptions());
                    setTimeout(function () {
                        document.location = resolveClientUrl('Section/Index');
                    }, 3000);
                } else {
                    toastr.error(data.message, '', toastOptions());
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
                                    deparments.map((item, i) => (
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
            initialValues={{
                code: '',
                name: '',
                isActive: false,
                department: ''
            }}
            validationSchema={CreateSectionSchema}
            onSubmit={onSubmit}
        >
            {({ errors }) => (
                renderForm(errors)
            )}
        </Formik>

    )
}
export default AppContainer;