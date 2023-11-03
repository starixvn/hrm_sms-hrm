import React, { Fragment } from 'react';

import { resolveClientUrl } from '../../../../shared/common.functions';

const Row = ({ rowIndex, item, onDelete }) => {
    const handleEdit = () => {
        window.location = resolveClientUrl(`section/edit/${item.id}`);
    };

    const handleDelete = () => {
        onDelete(item.id);
    };

    return (
        <tr id={`row_${item.id}`}>
            <td>{rowIndex}</td>
            <td>{item.code}</td>
            <td>{item.name}</td>
            <td>
                {item.department ? item.department.name : null}
            </td>
            <td>
                {item.isActive && <i className="fa fa-check fa-lg icon-check" />}
            </td>
            {
                window.hasPermission === 'True' &&
                <Fragment>
                    <td>
                        <a onClick={handleEdit} style={{ cursor: 'pointer' }}>
                            <i className="fa fa-pencil fa-lg icon-edit" />
                        </a>
                    </td>
                    <td>
                        <a onClick={handleDelete} style={{ cursor: 'pointer' }}>
                            <i className="fa fa-times fa-lg icon-delete" />
                        </a>
                    </td>
                </Fragment>
            }
        </tr>
    );
};

export default Row;