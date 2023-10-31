import axios from 'axios';
import toastr from 'toastr';
import React, { useEffect, useState } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

const EditUnitSchema = Yup.object({
    code: Yup.string()
        .trim()
        .max(50, 'The Code must be 50 characters!')
        .required('The Code field is required!'),
    name: Yup.string()
        .trim()
        .max(200, 'The Name must be 200 characters!')
        .required('The Name field is required!')
});

export const AppContainer = () => {
    const id = window.id;
    const handleGoBack = () => {
        history.back();
    }
    const [data, setData] = useState([])

    const handleGetDataDetail = (id) => {
        axios(resolveClientUrl(`Unit/getById/${id}`)).then(response => {
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
            axios.post(resolveClientUrl('Unit/Edit'), {
                UnitId: data.id,
                Code: data.code.trim(),
                Name: data.name.trim(),
                IsActive: data.isActive
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    toastr.success('Successfully!', '', toastOptions());
                    setTimeout(function () {
                        document.location = resolveClientUrl('Unit/Index');
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
                id: data.id || '',
                name: data.name || '',
                code: data.code || '',
                isActive: data.isActive || false
            }}
            validationSchema={EditUnitSchema}
            onSubmit={onSubmit}
        >
            {({ errors }) => (
                renderForm(errors)
            )}
        </Formik>
    )
};