import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { Button } from 'react-bootstrap';

import Filter from './Filter';
import DataGrid from './DataGrid';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = () => {
    const [rolePermission, setRolePermission] = useState(null);
    const [roleSelected, setRoleSelected] = useState({ value: 0, label: null });
    const [roles, setRoles] = useState([]);

    const assignPermission = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Admin/RolePermission/AssignPermission');
        var param = {
            roleId: roleSelected.value,
            permissions: rolePermission.permissions
        }
        await DataService.postDataAsync(url, param).then(response => {
            if (response.data.isSuccessful) {
                toast.success(LangHelper.getResource('Successfully'));
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());

    };

    const getRolePermission = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Admin/RolePermission/GetRolePermission');
        await DataService.getDataAsync(url, params).then(response => {
            setRolePermission(response.data);
        }).then(() => Site.unblockUI());
    };

    useEffect(() => {
        const getRoleBySiteId = async () => {
            const url = Site.resolveClientUrl('Admin/RolePermission/GetRoles');
            await DataService.getDataAsync(url).then(response => {
                setRoles(response.data);
                if (response.data.length > 0) {
                    setRoleSelected({ value: response.data[0].roleId, label: response.data[0].name });
                }
            });
        };
        getRoleBySiteId();
    }, []);

    return (
        <div>
            <Filter roles={roles}
                getRolePermission={getRolePermission}
                roleSelected={roleSelected}
                setRoleSelected={setRoleSelected} />
            <DataGrid rolePermission={rolePermission}
                setRolePermission={setRolePermission} />
            {
                window.hasHostPermission && roles.length > 0 &&
                rolePermission && rolePermission.functions.length > 0 &&
                <Button variant="primary" size="sm" className="mt-3" type="submit" onClick={assignPermission}>
                    {LangHelper.getResource('Submit')}
                </Button>
            }
        </div>
    );
};

export default AppContainer;