import React, { Fragment } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../shared/common.functions';

export const Table = (props) => {
    //const { data, handleGetDepartment } = props;
    //const handleViewId = (id) => {
    //    window.location.href = (`/department/edit/${id}`);
    //}
    //const handleDeleteId = (id) => {
    //    confirm('Are you sure you want to delete this department?', () => {
    //        axios.post(resolveClientUrl('Department/Delete'), {
    //            Id: id
    //        }).then(response => {
    //            const { data } = response;
    //            if (data.isSuccessful) {
    //                alert('Delete successfully!');
    //                handleGetDepartment();
    //            } else {
    //                alert(data.Message);
    //            }
    //        }).catch(axiosError);
    //    });
    //}
    return (
        <Fragment>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th rowSpan="2">Function</th>
                        <th colSpan="2">Role Permission</th>
                    </tr>
                    <tr>
                        <th>Read</th>
                        <th>Host</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>1</td>
                        <td>2</td>
                        <td>3</td>
                    </tr>
                </tbody>
            </table>
        </Fragment>
    )
}