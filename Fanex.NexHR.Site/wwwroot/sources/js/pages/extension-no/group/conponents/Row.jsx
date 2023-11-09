import React from 'react';


const Row = ({ rowIndex, item, onDelete , onEdit }) => {
    const handleDelete = () => {
        onDelete(item.teamId);
    };
    const handleGetDataById = () => {
        onEdit(item.teamId, item.teamName)
    };
    return (
        <tr>
            <td>{rowIndex}</td>
            <td className="align-left">{item.teamName}</td>
            <td>
                <i
                    className="fa fa-pencil fa-lg icon-edit"
                    style={{ cursor: 'pointer' }}
                    onClick={handleGetDataById}
                />
            </td>
            <td>
                {
                    <i
                        className={`fa fa-times fa-lg ${item.totalMember > 0 ? 'fa-disabled' : ''} icon-delete`}
                        style={{ cursor: `${item.totalMember > 0 ? 'no-drop' : 'pointer'}` }}
                        onClick={item.totalMember > 0 ? null : handleDelete}
                    />
                }
            </td>
        </tr>
    );
};

export default Row;