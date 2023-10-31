import React, { Fragment } from 'react';

export const Table = (props) => {
    const { data, handleEditInfo, handleDelete } = props;

    const handleLink = (id) => {
        window.location.href = (`/Employee/UserManagement?jobTitleId=${id}`);
    }

    const colSpan = window.hasPermission === 'True' ? 8 : 6;

    return (
        <Fragment>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Job Title</th>
                        <th>Category</th>
                        <th>Total Expat</th>
                        <th>Total Local</th>
                        <th>Total(s)</th>
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
                    {data.length ? data.map((item, i) => (
                        <tr id={`row_${i}`} key={item.id}>
                            <td>{i + 1}</td>
                            <td className="align-left">{item.name}</td>
                            <td className="align-left">{item.categoryName}</td>
                            <td >{item.totalExpat}</td>
                            <td >{item.totalLocal}</td>
                            <td>
                                {item.value !== "0" ? <a onClick={() => handleLink(item.id)} style={{ cursor: 'pointer' }}>{item.value}</a> : 0}
                            </td>
                            {
                                window.hasPermission === 'True' &&
                                <Fragment>
                                    <td>
                                        <a onClick={() => handleEditInfo(item.id, item.name, item.categoryId)} style={{ cursor: 'pointer' }}>
                                            <i className="fa fa-pencil fa-lg icon-edit"></i>
                                        </a>
                                    </td>
                                    <td>
                                        {item.value !== "0" ?
                                            <a>
                                                <i className="fa fa-times fa-lg fa-disabled icon-delete"></i>
                                            </a> :
                                            <a onClick={() => handleDelete(item.id)} style={{ cursor: 'pointer' }}>
                                                <i className="fa fa-times fa-lg icon-delete"></i>
                                            </a>
                                        }
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