import React, { Fragment } from 'react';

const Row = ({ rowIndex, item, onDelete, onEdit }) => {
    const handleDelete = () => {
        onDelete(item);
    };

    const handleEdit = () => {
        onEdit(item);
    };

    return (
        <tr id={`row_${item.id}`}>
            <td>{rowIndex}</td>
            <td className="align-left">{item.name}</td>
            {
                window.hasPermission === 'True' &&
                <Fragment>
                    <td>
                        <a onClick={handleEdit} style={{ cursor: 'pointer' }}>
                            <i className="fa fa-pencil fa-lg icon-edit"></i>
                        </a>
                    </td>
                    <td>
                        <a onClick={handleDelete} style={{ cursor: 'pointer' }}>
                            <i className="fa fa-times fa-lg icon-delete"></i>
                        </a>
                    </td>
                </Fragment>
            }
        </tr>
    );
};

export default Row;