import axios from 'axios';
import { stringify } from 'qs';

import { reorder, resolveClientUrl } from '../../../shared/common.functions';

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

function reorderSupervisor(key, employeeId, supervisors, callback) {
    const totalItems = supervisors.length,
        sourceIndex = supervisors.findIndex(item => item.ownerId === employeeId);
    let destinationIndex = 0;

    switch (key) {
        case 'up':
            destinationIndex = sourceIndex - 1;
            break;
        case 'down':
            destinationIndex = sourceIndex + 1;
            break;
        case 'bottom':
            destinationIndex = totalItems;
            break;
    }

    if (destinationIndex < 0 || destinationIndex > totalItems || sourceIndex === destinationIndex) {
        return;
    }

    if (callback !== null && typeof callback === 'function') {
        supervisors = reorder(supervisors, sourceIndex, destinationIndex);
        callback(supervisors);
    }
}

export {
    getSupervisors,
    editEmployeeSupervisor,
    editSectionSupervisor,
    reorderSupervisor
};