import React, { Fragment } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../../shared/common.functions';
export const Table = (props) => {
    const { data, hanleGetSection } = props;
    const handleViewId = (id) => {
        window.location.href = (`/section/edit/${id}`);
    }
    const handleDeleteId = (id) => {
        confirm('Are you sure you want to delete this section?', () => {
            axios.post(resolveClientUrl(`Section/Delete/${id}`)).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    hanleGetSection();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    }
    return (
        <Fragment>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Code</th>
                        <th>Section</th>
                        <th>Department</th>
                        <th>Active</th>
                        {
                            window.hasPermission === 'True' && <Fragment>
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
                                <td>
                                    {item.department ? item.department.name : null }
                                </td>
                                <td>
                                    {item.isActive && <i className="fa fa-check fa-lg icon-check" />}
                                </td>
                                {
                                    window.hasPermission === 'True' && <Fragment>
                                        <td>
                                            <a onClick={() => handleViewId(item.id)} style={{ cursor: 'pointer' }}>
                                                <i className="fa fa-pencil fa-lg icon-edit" />
                                            </a>
                                        </td>
                                        <td>
                                            <a onClick={() => handleDeleteId(item.id)} style={{ cursor: 'pointer' }}>
                                                <i className="fa fa-times fa-lg icon-delete" />
                                            </a>
                                        </td>
                                    </Fragment>
                                }
                            </tr>
                        )) : <tr>
                                <td colSpan={ window.hasPermission === 'True' ? '7' : '5' }>
                                No information available
                            </td>
                        </tr>
                    }

                </tbody>
            </table>
        </Fragment>
    )
}