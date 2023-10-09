import React, { Fragment } from 'react';

export const Table = (props) => {
    const { data } = props;
    const handleViewId = (id) => {
        window.location.href = (`/unit/edit/${id}`);
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
                                            <a style={{ cursor: 'pointer' }}>
                                                <i className="fa fa-times fa-lg icon-delete"></i>
                                            </a>
                                        </td>
                                    </Fragment>
                                }
                            </tr>
                        )) : <tr>
                                <td colSpan='10'>
                                No information available
                            </td>
                        </tr>
                    }
                    
                </tbody>
            </table>
        </Fragment>
    )
}