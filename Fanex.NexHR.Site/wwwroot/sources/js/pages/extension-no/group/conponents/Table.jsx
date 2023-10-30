import React from 'react';

export const Table = (props) => {
    const { data, handleDeleteData, handleGetDataById } = props;
    if (!data) return null
    return (
        <table className="table table-custom" id="data" style={{ width: 'auto', minWidth: '600px' }}>
            <thead>
                <tr>
                    <th>#</th>
                    <th>Team Name</th>
                    <th>Edit</th>
                    <th>Delete</th>
                </tr>
            </thead>
            <tbody>
                {
                    data.length > 0 ? data.map((item, i) => (
                        <tr key={item.teamId}>
                            <td>{i + 1}</td>
                            <td className="align-left">{item.teamName}</td>
                            <td>
                                <i
                                    className="fa fa-pencil fa-lg icon-edit"
                                    style={{ cursor: 'pointer' }}
                                    onClick={() => handleGetDataById(item.teamId)}
                                />
                            </td>
                            <td>
                                {
                                    <i
                                        className={`fa fa-times fa-lg ${item.totalMember > 0 ? 'fa-disabled' : ''} icon-delete`}
                                        style={{ cursor: `${item.totalMember > 0 ? 'no-drop' : 'pointer'}` }}
                                        onClick={item.totalMember > 0 ? null : () => handleDeleteData(item.teamId)}
                                    />
                                }
                            </td>
                        </tr>
                    )) : <tr>
                        <td colSpan='4'>
                            No information available
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    );
}