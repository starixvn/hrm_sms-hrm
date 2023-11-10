import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import * as Yup from 'yup';
import axios from 'axios';
import toastr from 'toastr';
import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';
import { Formik, Form, Field } from 'formik';

import Autocomplete from '../../../../shared/components/Autocomplete';
import Errors from '../../../../shared/components/Errors';

const AppContainer = () => {
    const handleGoBack = () => {
        history.back();
    };
    const [extensionDetail, setExtensionDetail] = useState(null);
    const [selectExtension, setSelectExtension] = useState(null);
    const [isInternal, setIsInternal] = useState(window.isInternal === 'True');
    const [employee, setEmployee] = useState(null);
    const [selectEmployee, setSelectEmployee] = useState(null);

    const editExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .trim()
            .required('The ExtensionNo field is required!'),
        Employee: isInternal ? Yup.string()
            .trim()
            .required('The Employee field is required!') : false
    });

    const getExtentionById = (isInternal, id) => {
        axios.get(resolveClientUrl('ExtensionNo/GetExtentionById'), {
            params: {
                id: id,
                isInternal: isInternal
            }
        }).then(response => {
            setExtensionDetail(response.data);
        }).catch(axiosError);
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
            params: { isInternal }
        }).then(response => {
            setSelectExtension(response.data);
        }).catch(axiosError);
    };

    const handleChange = data => {
        const selected = employee && employee.find(item => item.id === data.value);
        setSelectEmployee(selected);
    };
    useEffect(() => {
        getExtentionById(window.isInternal, window.id);
    }, [window.isInternal, window.id]);

    useEffect(() => {
        getGroupExtension(isInternal);
    }, [isInternal]);

    const handleChangeStatus = () => {
        setIsInternal(event.target.checked);
        axios(resolveClientUrl('ExtensionNo/GetGroupExtension'), {
            params: { isInternal: event.target.checked }
        }).then(response => {
            setSelectExtension(response.data);
            setExtensionDetail({
                empId: extensionDetail.empId,
                teamId: response.data[0].value,
                extensionNo: extensionDetail.extensionNo,
                nickName: extensionDetail.nickName,
                fullName: extensionDetail.fullName,
                employeeExtensionId: extensionDetail.employeeExtensionId,
                employee: extensionDetail.employee
            });
        }).catch(axiosError);
    }

    
    
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
                                    field
                                }) => (
                                    <input
                                        checked={isInternal}
                                        type="checkbox"
                                        {...field}
                                        onChange={handleChangeStatus}
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
                                        <option defaultValue={item.value} value={item.value} key={item.value}>{item.text}</option>
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
                        <button type="submit" id="btnSubmit" className="btn btn-primary" disabled={isSubmitting} style={{ marginRight: "5px" }}>Update</button>
                        <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                    </div>
                </Form>
            </div>
        );
    };

    if (extensionDetail == null) {
        return null;
    };
    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        axios.post(resolveClientUrl('ExtensionNo/Edit'), {
            FullName: values.FullName,
            NickName: values.NickName,
            ExtensionNo: values.ExtensionNo,
            IsInternal: values.IsInternal,
            EmpId: values.EmpId,
            EmployeeExtensionId: extensionDetail.employeeExtensionId,
            TeamId: values.TeamId
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Created successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl("ExtensionNo/Index");
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        }).catch(axiosError);
    };
    const empId = selectEmployee && selectEmployee.id || extensionDetail.empId;
    const emPloyee = selectEmployee && selectEmployee.value || '';
    const fullName = selectEmployee && selectEmployee.fullName || extensionDetail.fullName || '';
    const teamId = extensionDetail.teamId || '';
    const nickName = selectEmployee && selectEmployee.nickName || extensionDetail.nickName || '';
    const extensionNo = extensionDetail.extensionNo || '';
    return (
        <Formik
            validateOnBlur={false}
            validateOnChange={false}
            initialValues={{
                EmpId: empId,
                Employee: emPloyee,
                FullName: fullName,
                TeamId: teamId,
                NickName: nickName,
                ExtensionNo: extensionNo,
                IsInternal: isInternal
            }}
            enableReinitialize={true}
            validationSchema={editExtensionSchema}
            onSubmit={handleSubmit}
        >
            {renderForm}
        </Formik>

    )
};

export default AppContainer;