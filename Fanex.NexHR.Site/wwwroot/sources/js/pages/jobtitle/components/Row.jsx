import React, { Fragment } from 'react';

const Row = ({ rowIndex, item, onDelete, onEdit }) => {
    const handleDelete = () => {
        onDelete(item);
    };

    const handleEdit = () => {
        onEdit(item);
    };

    const handleLink = () => {
        window.location.href = (`/Employee/UserManagement?jobTitleId=${item.id}`);
    };

    return (
        <tr id={`row_${item.id}`}>
            <td>{rowIndex}</td>
            <td className="align-left">{item.name}</td>
            <td className="align-left">{item.categoryName}</td>
            <td >{item.totalExpat}</td>
            <td >{item.totalLocal}</td>
            <td>
                {item.value !== "0" ? <a onClick={handleLink} style={{ cursor: 'pointer' }}>{item.value}</a> : 0}
            </td>
            {
                window.hasPermission === 'True' &&
                <Fragment>
                    <td>
                        <a onClick={handleEdit} style={{ cursor: 'pointer' }}>
                            <i className="fa fa-pencil fa-lg icon-edit"></i>
                        </a>
                    </td>
                    <td>
                        {
                            item.value !== "0" ?
                                <a>
                                    <i className="fa fa-times fa-lg fa-disabled icon-delete"></i>
                                </a> :
                                <a onClick={handleDelete} style={{ cursor: 'pointer' }}>
                                    <i className="fa fa-times fa-lg icon-delete"></i>
                                </a>
                        }
                    </td>
                </Fragment>
            }
        </tr>
    );
};

export default Row;