import React, { Fragment } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../../shared/common.functions';

export const Table = (props) => {
    const { data, handleGetDepartment } = props;
    const handleEdit = (id) => {
        window.location.href = (`/department/edit/${id}`);
    }
    const handleDelete = (id) => {
        confirm('Are you sure you want to delete this department?', () => {
            axios.post(resolveClientUrl('Department/Delete'), {
                Id: id
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    handleGetDepartment();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    }
    const colSpan = window.hasPermission ? 7 : 5;

    return (
        <Fragment>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Code</th>
                        <th>Department</th>
                        <th>Unit</th>
                        <th>Active</th>
                        {
                            window.hasPermission && <Fragment>
                                <th>Edit</th>
                                <th>Delete</th>
                            </Fragment>
                        }
                    </tr>
                </thead>
                <tbody>
                    {
                        data.length ? data.map((item, i) => (
                            <tr id={`row_${i}`} key={item.id}>
                                <td>{i + 1}</td>
                                <td>{item.code}</td>
                                <td>{item.name}</td>
                                <td>{item.unit.name}</td>
                                <td>
                                    {item.isActive && <i className="fa fa-check fa-lg icon-check"></i>}
                                </td>
                                {
                                    window.hasPermission && <Fragment>
                                        <td>
                                            <a onClick={() => handleEdit(item.id)}>
                                                <i className="fa fa-pencil fa-lg icon-edit"></i>
                                            </a>
                                        </td>
                                        <td>
                                            <a onClick={() => handleDelete(item.id)}>
                                                <i className="fa fa-times fa-lg icon-delete"></i>
                                            </a>
                                        </td>
                                    </Fragment>
                                }
                            </tr>
                        )) :
                            <tr>
                                <td colSpan={colSpan}>No information available</td>
                            </tr>
                    }
                </tbody>
            </table>
        </Fragment>
    )
}