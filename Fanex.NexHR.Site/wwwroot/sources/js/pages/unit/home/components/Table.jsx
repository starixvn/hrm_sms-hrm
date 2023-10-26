import React, { Fragment } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../../shared/common.functions';
export const Table = (props) => {
    const { data, hanleGetUnit } = props;
    const handleViewId = (id) => {
        window.location.href = (`/unit/edit/${id}`);
    }
    const handleDeleteId = (id) => {
        confirm('Are you sure you want to delete this unit?', () => {
            axios.post(resolveClientUrl('Unit/Delete'), {
                UnitId: id,
                ModifiedBy: window._global.employeeId
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    alert('Delete successfully!');
                    hanleGetUnit();
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
                        <th>Unit</th>
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
                                <td>{ item.code }</td>
                                <td>{item.name}</td>
                                <td>
                                    { item.isActive && <i className="fa fa-check fa-lg icon-check"></i> }
                                </td>
                                {
                                    window.hasPermission === 'True' && <Fragment>
                                        <td>
                                            <a onClick={() => handleViewId(item.id)} style={{ cursor: 'pointer' }}>
                                                <i className="fa fa-pencil fa-lg icon-edit"></i>
                                            </a>
                                        </td>
                                        <td>
                                            <a onClick={() => handleDeleteId(item.id)} style={{ cursor: 'pointer' }}>
                                                <i className="fa fa-times fa-lg icon-delete"></i>
                                            </a>
                                        </td>
                                    </Fragment>
                                }
                            </tr>
                        )) : <tr>
                                <td colSpan={window.hasPermission === 'True' ? '6' : '4'}>
                                No information available
                            </td>
                        </tr>
                    }
                    
                </tbody>
            </table>
        </Fragment>
    )
}