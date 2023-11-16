import 'jquery-ui-bundle/jquery-ui';
import React, { useEffect, useState } from 'react';
import { Formik } from 'formik';
import * as Yup from 'yup';
import axios from 'axios';
import toastr from 'toastr';

import RenderFrom from '../../RenderForm';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [selectExtension, setSelectExtension] = useState(null);
    const [isInternal, setIsInternal] = useState(false);
    const [inputSelect, setInputSelect] = useState('');
    const [employee, setEmployee] = useState([]);
    const [selectEmployee, setSelectEmployee] = useState(null);
    const createExtensionSchema = Yup.object({
        ExtensionNo: Yup.string()
            .trim()
            .required('Extension No field is required!'),
        Employee: isInternal ? Yup.string()
            .trim()
            .required('Employee field is required!') : false
    });

    const getEmployees = query => {
        if (query.trim().length >= 2) {
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

    useEffect(() => {
        getGroupExtension(isInternal);
    }, [isInternal]);

    const handleSelect = (input , newValue) => {
        const selected = employee.find(item => item.id === newValue.id);
        setSelectEmployee(selected);
        setInputSelect(input);
    };

    const handleChangeInternal = () => {
        setIsInternal(event.target.checked);
    };

    const handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        axios.post(resolveClientUrl('ExtensionNo/Create'), {
            FullName: values.FullName,
            NickName: values.NickName,
            ExtensionNo: values.ExtensionNo,
            IsInternal: values.IsInternal,
            EmpId: values.EmpId,
            TeamId: values.TeamId
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Created successfully!', '', toastOptions());
                setTimeout(function () {
                    document.location = resolveClientUrl('ExtensionNo/Index');
                }, 3000);
            } else {
                toastr.error(data.message, '', toastOptions());
                setSubmitting(false);
            }
        }).catch(axiosError);
    };

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
            validationSchema={createExtensionSchema}
            onSubmit={handleSubmit}
        >
            {({ errors, isSubmitting }) => (
                <RenderFrom
                    errors={errors}
                    isInternal={isInternal}
                    onChangeInternal={handleChangeInternal}
                    selectExtension={selectExtension}
                    employee={employee}
                    onSearchEmployees={getEmployees}
                    onSelectEmployee={handleSelect}
                    isSubmitting={isSubmitting}
                    inputSelect={inputSelect}
                />
            )}
        </Formik>
    );
};
export default AppContainer;