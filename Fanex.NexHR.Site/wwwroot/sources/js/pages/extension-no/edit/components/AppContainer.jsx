import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

import Autocomplete from '../../../../shared/components/Autocomplete';
import Errors from '../../../../shared/components/Errors';


export const AppContainer = () => {
    const handleGoBack = () => {
        history.back();
    }
    const internal = window.isInternal;
    const id = window.id;
    const [extensionDetail, setExtensionDetail] = useState(null);
    const [selectExtension, setSelectExtension] = useState(null);
    const [isInternal, setIsInternal] = useState(internal === 'True');
    const [employee, setEmployee] = useState(null);
    const [selectEmployee, setSelectEmployee] = useState(null);

    const EditExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .trim()
            .required('The ExtensionNo field is required!')
    });

    const GetExtentionById = (isInternal, id) => {
        axios.get(resolveClientUrl('ExtensionNo/GetExtentionById'), {
            params: {
                id: id,
                isInternal: isInternal
            }
        }).then(response => {
            setExtensionDetail(response.data);
        }).catch(axiosError);
    }

    const getEmployees = query => {
        if (query.length >= 2) {
            axios.get(resolveClientUrl('employee/getemployees'), {
                params: { query }
            }).then(response => {
                const { data } = response;
                const employeeOptions = data.map(item => {
                    return {
                        id: item.Id,
                        value: `${item.NickName} (${item.FullName})`,
                        fullName: item.FullName,
                        nickName: item.NickName
                    };
                });

                setEmployee(employeeOptions);
            }).catch(axiosError);
        }
    };

    const getGroupExtension = (isInternal) => {
        axios(resolveClientUrl('ExtensionNo/GetGroupExtension'), {
            params: { IsInternal: isInternal }
        }).then(response => {
            setSelectExtension(response.data);
        }).catch(axiosError);
    }

    useEffect(() => {
        getGroupExtension(isInternal);
        GetExtentionById(isInternal, id);
    }, [isInternal, isInternal, id])


    const handleChange = (data) => {
        const selected = employee.find(item => item.id === data.value);
        setSelectEmployee(selected);
    }
    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <div className="card">
                <Errors errors={errors }/>
                <Form className="form-horizontal">
                    <input name="EmpId" hidden />
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Is Internal</label>
                        </div>
                        <div className="input-form">
                            <Field name="IsInternal" className="form-control">
                                {({
                                    field, form
                                }) => (
                                    <input
                                        checked={isInternal}
                                        type="checkbox"
                                        {...field}
                                        onChange={e => {
                                            setIsInternal(e.target.checked)
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
                                    selectExtension.map((item, i) => (
                                        <option value={item.value} key={item.value}>{item.text}</option>
                                    ))
                                }
                            </Field>
                        </div>
                    </div>
                    {
                        isInternal && <div className="form-group">
                            <div className="label-form">
                                <label className="control-label">Employee</label>
                            </div>
                            <div className="select-form">
                                <Field type="text" name="Employee" className="form-control">
                                    {({
                                        field, form
                                    }) => (

                                        <Autocomplete
                                            className='form-control'
                                            sources={employee}
                                            onInput={getEmployees}
                                            field={field}
                                            form={form}
                                            nameInput='Employee'
                                            onChange={handleChange}
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
                            <Field name="FullName">
                                {({
                                    field
                                }) => (
                                    <input
                                        autoComplete='off'
                                        className="form-control"
                                        type="text"
                                        {...field}
                                        readOnly={isInternal || false}
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
                                        autoComplete='off'
                                        className="form-control"
                                        type="text"
                                        {...field}
                                        readOnly={isInternal || false}
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
                            <Field type="text" name="ExtensionNo" className="form-control" autoComplete='off'/>
                        </div>
                    </div>
                    <div className="form-group">
                        <button type="submit" id="btnSubmit" className="btn btn-primary" disabled={isSubmitting} style={{ marginRight: "5px" }}>Update</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    }

    if (!selectExtension || extensionDetail == null) {
        return null;
    }
    const handleSubmit = (data) => {
        axios.post(resolveClientUrl('ExtensionNo/Edit'), {
            FullName: data.FullName,
            NickName: data.NickName,
            ExtensionNo: data.ExtensionNo,
            IsInternal: data.IsInternal,
            EmpId: data.EmpId,
            EmployeeExtensionId: extensionDetail.employeeExtensionId,
            TeamId: data.TeamId
        }).then(response => {
            console.log(response.data);
            const { data } = response;
            if (data.isSuccessful) {
                document.location = resolveClientUrl("ExtensionNo/Index");
            }
        }).catch(axiosError);
    }
    const fullName = selectEmployee ? selectEmployee.fullName : '';
    const nickName = selectEmployee ? selectEmployee.nickName : '';
    const empName = selectEmployee ? selectEmployee.value : '';
    const empId = selectEmployee ? selectEmployee.id : '';
    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={{
                EmpId: extensionDetail.EmpId ? extensionDetail.EmpId : empId,
                Employee: empName || '',
                FullName: fullName || extensionDetail.fullName || '',
                TeamId: extensionDetail.teamId || '',
                NickName: nickName || extensionDetail.nickName || '',
                ExtensionNo: extensionDetail.extensionNo || '',
                IsInternal: isInternal
            }}
            enableReinitialize={true}
            validationSchema={EditExtensionSchema}
            onSubmit={handleSubmit}
        >
            {renderForm }
        </Formik>
        
    )
};