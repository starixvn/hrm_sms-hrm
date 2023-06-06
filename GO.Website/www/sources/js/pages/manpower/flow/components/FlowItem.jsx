import React, { useState } from 'react';
import Select from 'react-select';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

const FlowItem = ({ flows, processId, onAddFlowItem, onRemoveFlowItem }) => {
    const [employeeOptions, setEmployeeOptions] = useState([]);

    const handleInputChange = Site.debounce(keyword => {
        if (keyword.length >= 2) {
            const url = Site.resolveClientUrl('Common/SearchEmployees');
            DataService.getDataAsync(url, { keyword }).then(response => {
                const emps = response.data.map(item => {
                    return {
                        ...item,
                        value: `${item.siteId}-${item.empId}`,
                        label: `${item.fullName} (${item.nickname})`
                    };
                });
                setEmployeeOptions(emps);
            });
        } else {
            setEmployeeOptions([]);
        }
    });

    const formatOptionLabel = option => (
        <>
            <div><span className="font-weight-bold">Site Name</span>: {option.siteName}</div>
            <div><span className="font-weight-bold">Code</span>: {option.code}</div>
            <div><span className="font-weight-bold">Nickname</span>: {option.nickname}</div>
            <div><span className="font-weight-bold">Full Name</span>: {option.fullName}</div>
        </>
    );

    const handleAdd = item => {
        onAddFlowItem(processId, item);
    };

    const handleRemove = item => {
        onRemoveFlowItem(processId, item);
    };

    const flowItems = flows.filter(item => item.processId === processId);

    return (
        <div className="flow-setting">
            <Select className={`react-select-container`}
                classNamePrefix="react-select"
                value={null} options={employeeOptions}
                formatOptionLabel={formatOptionLabel}
                isClearable={false} placeholder="nickname or full name"
                backspaceRemovesValue={false}
                components={{ NoOptionsMessage: () => null }}
                onInputChange={keyword => handleInputChange(keyword)}
                onChange={handleAdd}
            />
            {
                flowItems.map(item => {
                    return (
                        <div key={`process${item.processId}-site${item.siteId}-emp${item.empId}`} className="flow-item mt-2">
                            <img className="avatar" />
                            <div className="personal-info">
                                <div><span className="font-weight-bold">Site Name</span>: {item.siteName}</div>
                                <div><span className="font-weight-bold">Code</span>: {item.code}</div>
                                <div><span className="font-weight-bold">Nickname</span>: {item.nickname}</div>
                                <div><span className="font-weight-bold">Full Name</span>: {item.fullName}</div>
                            </div>
                            <i className="fa fa-times icon-delete" onClick={() => handleRemove(item)} />
                        </div>
                    );
                })
            }
        </div>
    );
};

export default FlowItem;