import axios from 'axios';
import toastr from 'toastr';
import React, { useState, useEffect } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { Form } from '../Form';

export const AppContainer = () => {
    const [validations, setValidations] = useState({})
    const [values, setValues] = useState({
        code: '',
        name: '',
        isActive: true,
        unitId: 0,
        units: []
    })

    const handleGetUnits = () => {
        axios(resolveClientUrl(`Department/GetUnits`)).then(response => {
            const { data } = response;
            if (data) {
                setValues(prevValues => ({
                    ...prevValues,
                    units: data
                }));
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetUnits();
    }, [])

    const validateAll = () => {
        const { name, code, unit } = values;
        const validations = { name: '', code: '', unit: '' };
        let isValid = true;
        if (!name) {
            isValid = false;
            validations.name = 'The Name field is required!';
        }
        if (name && name.length > 200) {
            isValid = false;
            validations.name = 'Name must be 200 characters';
        }
        if (!code) {
            isValid = false;
            validations.code = 'The Code field is required!';
        }
        if (code && code.length > 50) {
            isValid = false;
            validations.code = 'Code must be 50 characters';
        }
        //if (!unit) {
        //    isValid = false;
        //    validations.code = 'The Unit field is required!';
        //}
        if (!isValid) {
            setValidations({ validations })
        }

        return isValid;
    }

    const handleSubmit = (e) => {
        e.preventDefault()

        const isValid = validateAll()

        if (!isValid) {
            return false;
        } else {
            axios.post(resolveClientUrl(`Department/Create`), {
                Code: values.code,
                Name: values.name,
                IsActive: values.isActive,
                UnitId: values.UnitId
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
            setValidations({})
        }
    }

    return (
        <Form setValues={setValues} handleSubmit={handleSubmit} validations={validations.validations} data={values} btn='Create' />
    )
}