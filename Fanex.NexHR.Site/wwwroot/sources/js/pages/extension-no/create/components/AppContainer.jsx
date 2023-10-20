import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { debounce, isEmpty } from 'lodash';
export const AppContainer = () => {
    const [selectExtension, setSelectExtension] = useState([]);
    const [checked, setChecked] = useState(false);
    const [employee, setEmployee] = useState('');
    const CreateExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .required('The ExtensionNo field is required!'),
        Employee: checked ? Yup.string()
            .required('The Employee field is required!') : false
    });
    const initialValues = {
        Employee: '',
        FullName : '',
        TeamId: '',
        NickName: '',
        ExtensionNo : '',
        IsInternal : false
    }
    const handleGoBack = () => {
        history.back();
    }


    const handleChange = (e) => {
        setEmployee(e.target.value);
    }

    console.log(employee)


    const getGroupExtension = (checked) => {
        axios(resolveClientUrl('ExtensionNo/GetGroupExtension'), {
            params: { IsInternal: checked }
        }).then(response => {
            const { data } = response;
            if (data) {
                setSelectExtension(data)
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        getGroupExtension(checked);
    }, [checked])





    const renderForm = (errors) => {
        return (
            <div className="card">
                {
                    !isEmpty(errors) && <div className="validation-summary-errors">
                        <ul>
                            {errors.ExtensionNo && <li>{errors.ExtensionNo}</li>}
                            {errors.Employee && <li>{errors.Employee}</li>}
                        </ul>
                    </div>
                }
                <Form className="form-horizontal">
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Is Internal</label>
                        </div>
                        <div className="input-form">
                            <Field name="IsInternal">
                                {({
                                    field, form
                                }) => (
                                    <input
                                        type="checkbox"
                                        {...field}
                                        onChange={e => {
                                            setChecked(e.target.checked)
                                            form.setFieldValue('IsInternal', e.target.checked)
                                        }}
                                    />
                                )}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Group</label>
                        </div>
                        <div className="input-form">
                            <Field as="select" name="TeamId" className="form-control">
                                {
                                    selectExtension && selectExtension.map(item => (
                                        <option value={item.value} key={item.value}>{item.text}</option>
                                    ))
                                }
                            </Field>
                        </div>
                    </div>
                    {
                        checked && <div className="form-group">
                            <div className="label-form">
                                <label className="control-label">Employee</label>
                            </div>
                            <div className="input-form">
                                <Field type="text" name="Employee" className={`form-control ${errors && errors.Employee ? 'input-validation-error' : ''}`}>
                                    {({
                                        field, form
                                    }) => (
                                        <input
                                            className="form-control"
                                            type="text"
                                            {...field}
                                            onChange={e => {
                                                handleChange(e)
                                                form.setFieldValue('Employee', e.target.value)
                                            }}
                                        />
                                    )}
                                </Field>
                            </div>
                        </div>
                    }

                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Full Name</label>
                        </div>
                        <div className="input-form">
                            <Field name="FullName" name="FullName">
                                {({
                                    field
                                }) => (
                                    <input
                                        className="form-control"
                                        type="text"
                                        {...field}
                                        readOnly={checked ? checked : false}
                                    />
                                )}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Nick Name</label>
                        </div>
                        <div className="input-form">
                            <Field name="NickName" className="form-control">
                                {({
                                    field
                                }) => (
                                    <input
                                        className="form-control"
                                        type="text"
                                        {...field}
                                        readOnly={checked ? checked : false}
                                    />
                                )}
                            </Field>
                        </div>
                    </div>
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Extension No</label>
                        </div>
                        <div className="input-form">
                            <Field type="text" name="ExtensionNo" className={`form-control ${errors && errors.ExtensionNo ? 'input-validation-error' : ''}`} />
                        </div>
                    </div>
                    <div className="form-group">
                        <button type="submit" id="btnSubmit" className="btn btn-primary" style={{ marginRight: "5px" }}>Add</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    }

    const onSubmit = (data) => {
        console.log(data)
    }


    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={initialValues}
            validationSchema={CreateExtensionSchema}
            onSubmit={onSubmit}
        >
            {({ errors }) => (
                renderForm(errors)
            )}
        </Formik>
        
    )
};