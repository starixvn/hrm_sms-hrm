import React, { Fragment } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../shared/common.functions';

export const Table = (props) => {
    const { data, setEditModal, editJobLevel, reloadTable } = props;

    const handleDelete = (id) => {
        confirm('Are you sure you want to delete?', () => {
            axios.post(resolveClientUrl('JobLevel/Delete'), {
                id: id
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    alert('Delete successfully!');
                    reloadTable();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    }

    const handleEdit = (id, name) => {
        setEditModal(true);
        editJobLevel(id, name);
    }

    const colSpan = window.hasPermission === 'True' ? 4 : 2;

    return (
        <Fragment>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Name</th>
                        {
                            window.hasPermission === 'True' &&
                            <Fragment>
                                <th>Edit</th>
                                <th>Delete</th>
                            </Fragment>
                        }
                    </tr>
                </thead>
                <tbody>
                    {data ? data.map((item, i) => (
                        <tr id={`row_${i}`} key={item.id}>
                            <td>{i + 1}</td>
                            <td className="align-left">{item.name}</td>
                            {
                                window.hasPermission === 'True' &&
                                <Fragment>
                                    <td>
                                        <a onClick={() => handleEdit(item.id, item.name)} style={{ cursor: 'pointer' }}>
                                            <i className="fa fa-pencil fa-lg icon-edit"></i>
                                        </a>
                                    </td>
                                    <td>
                                        <a onClick={() => handleDelete(item.id)} style={{ cursor: 'pointer' }}>
                                            <i className="fa fa-times fa-lg icon-delete"></i>
                                        </a>
                                    </td>
                                </Fragment>
                            }
                        </tr>
                    ))
                        :
                        <tr>
                            <td colSpan={colSpan}>No information available</td>
                        </tr>
                    }
                </tbody>
            </table>
        </Fragment>
    )
}