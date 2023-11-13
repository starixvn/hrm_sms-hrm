import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import * as Yup from 'yup';
import axios from 'axios';
import toastr from 'toastr';
import { Formik } from 'formik';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';
import RenderFrom from '../../RenderForm';

const AppContainer = () => {
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
    const handleChange = items => {
        const selected = employee && employee.find(item => item.id === items.value);
        setSelectEmployee(selected);
    };
    useEffect(() => {
        getExtentionById(window.isInternal, window.id);
    }, [window.isInternal, window.id]);

    useEffect(() => {
        getGroupExtension(isInternal);
    }, [isInternal]);

    const handleChangeInternal = () => {
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
            {({ errors, isSubmitting }) => (
                <RenderFrom
                    errors={errors}
                    isInternal={isInternal}
                    onChangeInternal={handleChangeInternal}
                    selectExtension={selectExtension}
                    employee={employee}
                    getEmployees={getEmployees}
                    onChangeEmployee={handleChange}
                    isSubmitting={isSubmitting}
                />
            )}
        </Formik>

    );
};
export default AppContainer;