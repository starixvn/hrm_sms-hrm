import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { isEmpty } from 'lodash';
import Autocomplete from '../../../../shared/components/Autocomplete';


export const AppContainer = () => {
    const handleGoBack = () => {
        history.back();
    }
    const isInternal = window.isInternal;
    const id = window.id;
    const [detail, setDetail] = useState(null);
    const [selectExtension, setSelectExtension] = useState([]);
    const [checked, setChecked] = useState(isInternal === 'True');
    const [employee, setEmployee] = useState(null);
    const [selectEmployee, setSelectEmployee] = useState(null);

    const CreateExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .trim()
            .required('The ExtensionNo field is required!'),
        Employee: checked ? Yup.string()
            .trim()
            .required('The Employee field is required!') : false
    });

    const GetExtentionById = (isInternal, id) => {
        axios.get(resolveClientUrl('ExtensionNo/GetExtentionById'), {
            params: {
                id: id,
                isInternal: isInternal
            }
        }).then(response => {
            const { data } = response;
            setDetail(data);
        }).catch(axiosError);
    }

    const getEmployees = query => {
        if (query) {
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

    const getGroupExtension = (checked) => {
        axios(resolveClientUrl('ExtensionNo/GetGroupExtension'), {
            params: { IsInternal: checked }
        }).then(response => {
            const { data } = response;
            if (data) {
                setSelectExtension(data);
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        getGroupExtension(checked);
        GetExtentionById(isInternal, id);
    }, [checked, isInternal, id])


    const handleChange = (data) => {
        const selected = employee.find(item => item.id === data.value);
        setSelectEmployee(selected);
    }
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
                                        checked={checked}
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
                                    selectExtension.map((item, i) => (
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
                                        readOnly={checked || false}
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
                                        readOnly={checked || false}
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
                        <button type="submit" id="btnSubmit" className="btn btn-primary" style={{ marginRight: "5px" }}>Update</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    }

    if (!selectExtension || selectExtension[0] == null || detail == null) {
        return null;
    }
    const onSubmit = (data) => {
        if (data) {
            axios.post(resolveClientUrl('ExtensionNo/Edit'), {
                FullName: data.FullName,
                NickName: data.NickName,
                ExtensionNo: data.ExtensionNo,
                IsInternal: data.IsInternal,
                EmpId: data.EmpId,
                EmployeeExtensionId: detail.employeeExtensionId,
                TeamId: data.TeamId
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    document.location = resolveClientUrl("ExtensionNo/Index");
                }
            }).catch(axiosError);
        }
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
                EmpId: detail.EmpId ? detail.EmpId : empId,
                Employee: empName || '',
                FullName: fullName || detail.fullName || '',
                TeamId: detail.teamId || '',
                NickName: nickName || detail.nickName || '',
                ExtensionNo: detail.extensionNo || '',
                IsInternal: checked 
            }}
            enableReinitialize={true}
            validationSchema={CreateExtensionSchema}
            onSubmit={onSubmit}
        >
            {({ errors }) => (
                renderForm(errors)
            )}
        </Formik>
        
    )
};