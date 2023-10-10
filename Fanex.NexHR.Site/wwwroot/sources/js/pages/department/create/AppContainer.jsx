import axios from 'axios';
import toastr from 'toastr';
import React, { useState } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { Form } from '../Form';

export const AppContainer = () => {
    const [validations, setValidations] = useState({})
    const [values, setValues] = useState({
        name: '',
        code: '',
        isActive: true
    })

    const validateAll = () => {
        const { name, code } = values;
        const validations = { name: '', code: '' };
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
                IsActive: values.isActive
            }).then(response => {
                const { data } = response;
                console.log(data)
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
        <Form setValues={setValues} handleSubmit={handleSubmit} validations={validations.validations} data={values} btn='Create'/>
    )
}