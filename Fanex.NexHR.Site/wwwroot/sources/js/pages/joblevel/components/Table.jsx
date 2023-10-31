import React, { Fragment } from 'react';

export const Table = (props) => {
    const { data, handleDelete, handleEditInfo } = props;
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
                                        <a onClick={() => handleEditInfo(item.id, item.name)} style={{ cursor: 'pointer' }}>
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