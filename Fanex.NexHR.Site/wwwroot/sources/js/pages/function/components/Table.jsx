import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';

export const Table = (props) => {
    const { data } = props;
    const [functionPermissions, setFunctionPermissions] = useState([]);

    useEffect(() => {
        setFunctionPermissions(functionList(data.functions));
    }, [data.functions])

    const onSubmit = (e) => {
        e.preventDefault();
        axios.post(resolveClientUrl(`Function/AssignPermission`), {
            roleId: data.roleId,
            permissions: functionPermissions
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Successfully!', '', toastOptions());
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    }

    function getFunctionPermission(functionId) {
        return data.functionPermissions ? data.functionPermissions.find((item) => item.functionId === functionId) : null;
    }

    function createChildFunction(item, functionPermission) {
        return {
            parentId: item.parentId,
            functionId: item.functionId,
            functionName: item.functionName,
            isRead: functionPermission ? functionPermission.isRead : false,
            isHost: functionPermission ? functionPermission.isHost : false,
            level: item.level,
            policy: item.policy
        };
    }

    function functionList(functions) {
        if (!functions) return;
        const updatedList = functions.flatMap((item) => {
            const functionPermission = getFunctionPermission(item.functionId);
            const updatedChildList = item.childs.map((itemChild) => {
                const functionPermissionChild = getFunctionPermission(itemChild.functionId);
                return createChildFunction(itemChild, functionPermissionChild);
            });
            const updatedItem = {
                functionId: item.functionId,
                functionName: item.functionName,
                isRead: functionPermission ? functionPermission.isRead : false,
                isHost: functionPermission ? functionPermission.isHost : false,
                level: item.level,
                policy: item.policy,
                childIds: item.childs.length > 0 ? item.childs.map((itemChild) => {
                    return itemChild.functionId;
                }) : null
            };

            return [updatedItem, ...updatedChildList];
        });

        return updatedList;
    }

    function updateChildCheckbox(childIds, checked, permission) {
        if (!childIds) return;
        childIds.forEach((id) => {
            const childCheckbox = functionPermissions.find((child) => child.functionId === id);
            if (childCheckbox) {
                if (permission === 'read') {
                    childCheckbox.isRead = checked;
                    childCheckbox.isHost = false;
                } else {
                    childCheckbox.isHost = checked;
                    childCheckbox.isRead = checked ? true : childCheckbox.isRead;
                }
            }
        });
    }

    function updateParentCheckboxHost(parentId, functionId) {
        if (!parentId) return;
        const parentCheckbox = functionPermissions.find((child) => child.functionId === parentId);
        if (!event.target.checked) {
            let isFlagged = false;
            parentCheckbox.childIds.forEach((id) => {
                if (id !== functionId) {
                    const childCheckbox = functionPermissions.find((child) => child.functionId === id);
                    if (childCheckbox.isHost) {
                        isFlagged = true;
                        return;
                    }
                }
            });
            parentCheckbox.isHost = isFlagged;
        }
        else {
            parentCheckbox.isHost = true;
            parentCheckbox.isRead = true;
        }
    }

    function updateParentCheckboxRead(parentId, functionId) {
        if (!parentId) {
            return;
        }
        const parentCheckbox = functionPermissions.find((child) => child.functionId === parentId);
        if (!event.target.checked) {
            let isFlaggedRead = false, isFlaggedHost = false;
            parentCheckbox.childIds.forEach((id) => {
                if (id !== functionId) {
                    const childCheckbox = functionPermissions.find((child) => child.functionId === id);
                    isFlaggedRead = isFlaggedRead || childCheckbox.isRead;
                    isFlaggedHost = isFlaggedHost || childCheckbox.isHost;
                }
            });
            parentCheckbox.isRead = isFlaggedRead;
            parentCheckbox.isHost = isFlaggedHost;
        }
        else {
            parentCheckbox.isRead = true;
        }
    }

    const handleCheckboxChange = (event, functionId, permission) => {
        const updatedCheckboxes = functionPermissions.map((checkbox) => {
            if (checkbox.functionId === functionId) {
                const updatedCheckbox = { ...checkbox };
                updateChildCheckbox(checkbox.childIds, event.target.checked, permission);
                if (permission === 'read') {
                    updatedCheckbox.isRead = event.target.checked;
                    updatedCheckbox.isHost = false;
                    updateParentCheckboxRead(checkbox.parentId, checkbox.functionId);
                } else {
                    updatedCheckbox.isHost = event.target.checked;
                    updatedCheckbox.isRead = true;
                    updateParentCheckboxHost(checkbox.parentId, checkbox.functionId);
                }
                return updatedCheckbox;
            }
            return checkbox;
        });

        setFunctionPermissions(updatedCheckboxes);
    };

    const renderTableRows = (items) => {
        const disabled = data.isHost ? '' : "disabled";
        return items.map((item, i) => (
            <Fragment key={i}>
                <tr id={`row_${i}`} className={item.level === 0 ? 'root-row' : 'child-row'}>
                    <td className="align-left">
                        {item.level > 0 && (
                            <Fragment>
                                {Array.from({ length: item.level.length - 1 }).map((_, i) => (
                                    <span key={i} style={{ opacity: 0 }} className="icon-child-level" >&#9495;</span>
                                ))}
                                <span className="icon-child-level">&#9495;</span>
                            </Fragment>
                        )}
                        {item.functionName}
                    </td>
                    <td>
                        {(!item.policy || item.policy == window.read) &&
                            <input
                                type="checkbox"
                                checked={item.isRead}
                                onChange={(event) => handleCheckboxChange(event, item.functionId, 'read')}
                                disabled={disabled}
                            />
                        }
                    </td>
                    <td>
                        {(!item.policy || item.policy == window.host) &&
                            <input
                                type="checkbox"
                                checked={item.isHost}
                                onChange={(event) => handleCheckboxChange(event, item.functionId, 'host')}
                                disabled={disabled}
                            />
                        }
                    </td>
                </tr>
            </Fragment>
        ));
    };

    return (
        <Fragment>
            <form onSubmit={onSubmit}>
                <table className="table table-custom">
                    <thead>
                        <tr>
                            <th rowSpan="2">Function</th>
                            <th colSpan="2">Role Permission</th>
                        </tr>
                        <tr>
                            <th className="th-read">Read</th>
                            <th>Host</th>
                        </tr>
                    </thead>
                    <tbody>
                        {functionPermissions ?
                            renderTableRows(functionPermissions)
                            : (
                                <tr>
                                    <td colSpan="3">No information available</td>
                                </tr>
                            )}
                    </tbody>
                </table>
                {
                    data.isHost &&
                    <div className="form-group">
                        <button type="submit" className="btn btn-primary">Save</button>
                    </div>
                }
            </form>
        </Fragment>
    )
}