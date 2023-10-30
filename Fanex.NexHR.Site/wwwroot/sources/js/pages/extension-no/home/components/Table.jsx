import React, { Fragment } from 'react';

export const Table = (props) => {
    const { data, handleDeleteRow, handleEditRow } = props;
    let index = 1
    return (
        <table className="table table-custom" id="data">
            <thead>
                <tr>
                    <th>No</th>
                    <th>Full Name</th>
                    <th>Nickname</th>
                    <th>Extension Number</th>
                    {
                        window.hasPermission &&
                        <Fragment>
                            <th>Edit</th>
                            <th>Delete</th>
                        </Fragment>
                    }
                </tr>
            </thead>
            <tbody>
                {
                    data.map(item => (
                        <Fragment key={item.name}>
                            <tr>
                                <td className="rowdivider align-left" colSpan={window.hasPermission === 'True' ? '6' : '4'}>
                                    <b>{item.name}</b>
                                </td>
                            </tr>
                            {
                                item.extensions.map(item => (
                                    <tr key={item.employeeExtensionId}>
                                        <td>
                                            {index++}
                                        </td>
                                        <td className="align-left">
                                            {item.fullName}
                                        </td>
                                        <td>
                                            {item.nickName}
                                        </td>

                                        <td style={{ color: '#0789D1' }}>
                                            {item.extensionNo}
                                        </td>
                                        {
                                            window.hasPermission &&
                                            <Fragment>
                                                <td>
                                                    <i className="fa fa-pencil fa-lg icon-edit" onClick={() => handleEditRow(item.employeeExtensionId, item.isInternal)} style={{ cursor: 'pointer' }}></i>
                                                </td>
                                                <td>
                                                    <i className="fa fa-times fa-lg icon-delete" onClick={() => handleDeleteRow(item.employeeExtensionId)} style={{ cursor: 'pointer' }}></i>
                                                </td>
                                            </Fragment>
                                        }

                                    </tr>
                                ))
                            }
                        </Fragment>
                    ))
                }
            </tbody>
        </table>
    );
}