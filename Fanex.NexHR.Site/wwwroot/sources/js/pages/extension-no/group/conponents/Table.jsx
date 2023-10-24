import React, { Fragment } from 'react';

export const Table = (props) => {
    const { data } = props;
    if (!data) return null
    return (
        <table className="table table-custom" id="data" style={{ width: 'auto', minWidth: '600px' }}>
            <thead>
                <tr>
                    <th>#</th>
                    <th>TeamName</th>
                    <th>Edit</th>
                    <th>Delete</th>
                </tr>
            </thead>
            <tbody>
                {
                    data.map((item , i) => (
                        <tr key={ item.teamId }>
                            <td>{ i+ 1}</td>
                            <td className="align-left">{item.teamName}</td>
                            <td>
                                <i className="fa fa-pencil fa-lg icon-edit" style={{ cursor: 'pointer' }}></i>
                            </td>
                            <td>
                                {
                                    <i className={`fa fa-times fa-lg ${item.totalMember > 0 ? 'fa-disabled' : ''} icon-delete`} style={{ cursor: `${item.totalMember > 0 ? 'no-drop' : 'pointer'}` }}></i>
                                }
                            </td>
                        </tr>
                    ))
                }
            </tbody>
        </table>
    );
}