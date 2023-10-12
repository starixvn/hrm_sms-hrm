import axios from 'axios';
import toastr from 'toastr';
import React, { useEffect, useState } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { isEmpty } from 'lodash';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

const EditDepartmentSchema = Yup.object({
    code: Yup.string()
        .max(50, 'Code must be 50 characters')
        .required('The Code field is required!'),
    name: Yup.string()
        .max(200, 'Name must be 200 characters')
        .required('The Name field is required!'),
    unit: Yup.string()
        .required('The Unit field is required!')
});

export const AppContainer = () => {
    const url = window.location.href;
    const id = url.substring(url.lastIndexOf('/') + 1);
    const handleGoBack = () => {
        history.back();
    }

    const [data, setData] = useState([]);
    const [units, setUnits] = useState([]);

    const handleGetById = (id) => {
        axios(resolveClientUrl(`department/getById/${id}`)).then(response => {
            const { data } = response;
            if (data) {
                setData(data);
                console.log(data)
            }
        }).catch(axiosError);
    }

    const handleGetUnits = () => {
        axios(resolveClientUrl(`Department/GetUnits`)).then(response => {
            const { data } = response;
            if (data) {
                setUnits(data);
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        if (id) {
            handleGetById(id);
            handleGetUnits();
        }
    }, [id])
    const onSubmit = (data) => {
        if (data) {
            axios.post(resolveClientUrl(`Department/Edit`), {
                DepartmentId: data.id,
                Code: data.code.trim(),
                Name: data.name.trim(),
                IsActive: data.isActive,
                UnitId: data.unit
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    toastr.success('Successfully!', '', toastOptions());
                    setTimeout(function () {
                        document.location = resolveClientUrl("Department/Index");
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
                                {units.map((unit) => (
                                    <option key={unit.id} value={unit.id}>
                                        {unit.name}
                                    </option>
                                ))}
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
                id: data.id || '',
                name: data.name || '',
                code: data.code || '',
                isActive: data.isActive || false,
                unit: data.unit || ''
            }}
            validationSchema={EditDepartmentSchema}
            onSubmit={onSubmit}
        >
            {({ errors }) => (
                renderForm(errors)
            )}
        </Formik>
    )
};