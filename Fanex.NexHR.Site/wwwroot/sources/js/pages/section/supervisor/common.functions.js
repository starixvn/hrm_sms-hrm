import axios from 'axios';
import { stringify } from 'qs';

import { resolveClientUrl } from '../../../shared/common.functions';

function getSupervisors(query, roleIds, excludeIds) {
    return axios(resolveClientUrl('employee/GetSupervisors'), {
        params: {
            roleIds,
            query,
            excludeIds
        }
    });
}

function editEmployeeSupervisor(employeeId, roleId, supervisors) {
    return axios({
        url: resolveClientUrl('employee/EditEmployeeSupervisor'),
        method: 'POST',
        data: stringify({
            employeeId,
            roleId,
            supervisors
        }),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    });
}

function editSectionSupervisor(sectionId, roleId, supervisors) {
    return axios({
        url: resolveClientUrl('Team/EditSectionSupervisor'),
        method: 'POST',
        data: stringify({
            sectionId,
            roleId,
            supervisors
        }),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    });
}

export {
    getSupervisors,
    editEmployeeSupervisor,
    editSectionSupervisor
};