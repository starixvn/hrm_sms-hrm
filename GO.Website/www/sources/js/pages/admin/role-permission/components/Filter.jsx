import React, { useEffect } from 'react';
import { Form } from 'react-bootstrap';
import Select from 'react-select';
import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ roles, getRolePermission, roleSelected, setRoleSelected }) => {
    const handleChange = selectedItem => {
        setRoleSelected(selectedItem);
        getRolePermission({
            roleId: selectedItem.value
        });
    };

    useEffect(() => {
        getRolePermission({
            roleId: roleSelected.value
        });
    }, [roles]);

    const roleOptions = roles.map(item => { return { value: item.roleId, label: item.name }; });

    return (
        <Form className="form-filter">
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <Form.Label>{LangHelper.getResource('Role')}</Form.Label>
                    <Select className={`react-select-container`} classNamePrefix="react-select"
                        options={roleOptions} value={roleSelected}
                        onChange={handleChange}
                    />
                </Form.Group>
            </Form.Row>
        </Form>
    );
}
export default Filter;