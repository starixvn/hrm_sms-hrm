import { Field } from 'formik';
import React, { useEffect, useState } from 'react';
import { axiosError, resolveClientUrl } from '../../../shared/common.functions';
import axios from 'axios';

export const Filter = (props) => {
    const { setChoose, submit } = props;
    const [systemRoles, setSystemRoles] = useState([]);

    const handleGetSystemRoles = () => {
        axios.post(resolveClientUrl('Function/GetSystemRoles'), {
        }).then(response => {
            setSystemRoles(response.data)
        }).catch(axiosError);
    }

    const handleSelectChange = (event) => {

    };

    useEffect(() => {
        handleGetSystemRoles();
    }, [])

    return (
        <form className="filter">
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Role</label>
                    </div>
                    <div className="select-form">
                        <select name="unit" as="select" className="form-control" onChange={handleSelectChange}>
                            {systemRoles.map((role) => (
                                <option key={role.roleId} value={role.roleId}>
                                    {role.name}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>
            </div>
        </form>
    )
};