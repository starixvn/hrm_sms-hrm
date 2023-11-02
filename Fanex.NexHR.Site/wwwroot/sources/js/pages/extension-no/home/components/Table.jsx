import React, { Fragment } from 'react';
import Row from './Row';

export const Table = ({ data, onDelete }) => {
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
                    data.length > 0 ? data.map(item => (
                        <Fragment key={item.name}>
                            <tr>
                                <td className="rowdivider align-left" colSpan={window.hasPermission === 'True' ? '6' : '4'}>
                                    <b>{item.name}</b>
                                </td>
                            </tr>
                            {
                                item.extensions.map(item => <Row key={item.employeeExtensionId} rowIndex={index++} item={item} onDelete={onDelete}/>)
                            }
                        </Fragment>
                    )) : <tr>
                        <td colSpan={window.hasPermission === 'True' ? '6' : '4'}>
                            No information available
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    );
}