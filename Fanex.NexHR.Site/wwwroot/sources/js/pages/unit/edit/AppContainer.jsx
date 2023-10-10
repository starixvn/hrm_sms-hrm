import axios from 'axios';
import toastr from 'toastr';
import React, { useEffect, useState } from 'react';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import { Form } from '../Form';


export const AppContainer = () => {
    
    const url = window.location.href;
    const id = url.substring(url.lastIndexOf('/') + 1);
    const [validations, setValidations] = useState({})
    const [values, setValues] = useState({
        id : null,
        name: '',
        code: '',
        isActive: false
    })

    const handleGetDataDetail = (id) => {
        axios(resolveClientUrl(`Unit/getById/${id}`)).then(response => {
            const { data } = response;
            if (data) {
                setValues({
                    id : data.id,
                    code: data.code,
                    name: data.name,
                    isActive: data.isActive
                });
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        if (id) {
            handleGetDataDetail(id)
        }
    }, [id])
    const validateAll = () => {
        const { name, code } = values;
        const validations = { name: '', code : '' };
        let isValid = true;
        if (!name) {
            isValid = false;
            validations.name = 'Name is required';
        }
        if (name && name.length > 200) {
            isValid = false;
            validations.name = 'Name must be 200 characters';
        }
        if (!code) {
            isValid = false;
            validations.code = 'Code is required';
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
            axios.post(resolveClientUrl(`Unit/Edit`), {
                UnitId: values.id,
                Code: values.code,
                Name: values.name,
                IsActive: values.isActive
            }).then(response => {
                const { data } = response;
                console.log(data)
                if (data.IsSuccessful) {
                    toastr.success('Successfully!', '', toastOptions());
                    setTimeout(function () {
                        document.location = resolveClientUrl("Unit/Index");
                    }, 3000);
                } else {
                    toastr.error(data.Message, '', toastOptions());
                }
            }).catch(axiosError);
            setValidations({})
        }
    }
    return (
        <Form setValues={setValues}  handleSubmit={handleSubmit} validations={validations.validations} data={values} btn='Update'/>
    )
};