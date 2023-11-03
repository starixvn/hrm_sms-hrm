import React, { Fragment } from 'react';

import { resolveClientUrl } from '../../../../shared/common.functions';

const Row = ({ rowIndex, item, onDelete }) => {
    const handleEdit = () => {
        window.location = resolveClientUrl(`ExtensionNo/Edit/${item.employeeExtensionId}?isInternal=${item.isInternal}`);
    };

    const handleDelete = () => {
        onDelete(item.employeeExtensionId);
    };

    return (
        <tr>
            <td>{rowIndex}</td>
            <td className="align-left">{item.fullName}</td>
            <td>{item.nickName}</td>
            <td style={{ color: '#0789D1' }}>{item.extensionNo}</td>
            {
                window.hasPermission &&
                <Fragment>
                    <td>
                        <i className="fa fa-pencil fa-lg icon-edit" onClick={handleEdit} style={{ cursor: 'pointer' }} />
                    </td>
                    <td>
                        <i className="fa fa-times fa-lg icon-delete" onClick={handleDelete} style={{ cursor: 'pointer' }} />
                    </td>
                </Fragment>
            }
        </tr>
    );
};

export default Row;