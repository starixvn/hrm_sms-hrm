import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import Autocomplete from '../../../../shared/components/Autocomplete';
import Errors from '../../../../shared/components/Errors';


const AppContainer = () => {
    const [selectExtension, setSelectExtension] = useState(null);
    const [isInternal, setIsInternal]= useState(false);
    const [employee, setEmployee] = useState(null);
    const [selectEmployee, setSelectEmployee] = useState(null);
    const CreateExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .trim()
            .required('The ExtensionNo field is required!'),
        Employee: isInternal ? Yup.string()
            .trim()
            .required('The Employee field is required!') : false
    });
    const handleGoBack = () => {
        history.back();
    };
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

    const getGroupExtension = isInternal => {
        axios(resolveClientUrl('ExtensionNo/GetGroupExtension'), {
            params: { IsInternal: isInternal }
        }).then(response => {
            setSelectExtension(response.data);
        }).catch(axiosError);
    };

    useEffect(() => {
        getGroupExtension(isInternal);
    }, [isInternal])


    const handleChange = data => {
        const selected = employee.find(item => item.id === data.value);
        setSelectEmployee(selected);
    };
    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <div className="card">
                <Errors errors={errors} />
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
                            <Field type="text" name="ExtensionNo" className="form-control" autoComplete='off' />
                        </div>
                    </div>
                    <div className="form-group">
                        <button type="submit" id="btnSubmit" className="btn btn-primary" disabled={isSubmitting} style={{ marginRight: "5px" }}>Add</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    };
    
    const handleSubmit = data => {
        axios.post(resolveClientUrl('ExtensionNo/Create'), {
            FullName: data.FullName,
            NickName: data.NickName,
            ExtensionNo: data.ExtensionNo,
            IsInternal: data.IsInternal,
            EmpId: data.EmpId,
            TeamId: data.TeamId
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                document.location = resolveClientUrl("ExtensionNo/Index");
            }
        }).catch(axiosError);
    }

    if (!selectExtension) {
        return null;
    }
    const fullName = selectEmployee ? selectEmployee.fullName : '';
    const teamId = selectExtension[0].value;
    const nickName = selectEmployee ? selectEmployee.nickName : '';
    const empId = selectEmployee ? selectEmployee.id : '';
    const empName = selectEmployee ? selectEmployee.value : '';
    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={{
                EmpId: empId,
                Employee: empName,
                FullName: fullName,
                TeamId: teamId,
                NickName: nickName,
                ExtensionNo: '',
                IsInternal: isInternal || false
            }}
            enableReinitialize={true}
            validationSchema={CreateExtensionSchema}
            onSubmit={handleSubmit}
        >
            {renderForm}
        </Formik>
        
    )
};

export default AppContainer;