import React, { useState } from 'react';
import Select from 'react-select';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../actions';

import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import LangHelper from '@jsroot/common/langHelper';

const FlowItem = ({ flows, processId, addFlowItem, removeFlowItem }) => {
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
            <div><span className="font-weight-bold">{LangHelper.getResource('SiteName')}</span>: <span className="font-italic">{option.siteName}</span></div>
            <div><span className="font-weight-bold">{LangHelper.getResource('Code')}</span>: <span className="font-italic">{option.code}</span></div>
            <div><span className="font-weight-bold">{LangHelper.getResource('Nickname')}</span>: <span className="font-italic">{option.nickname}</span></div>
            <div><span className="font-weight-bold">{LangHelper.getResource('FullName')}</span>: <span className="font-italic">{option.fullName}</span></div>
        </>
    );

    const handleAdd = item => {
        addFlowItem({ processId, employee: item });
    };

    const handleRemove = item => {
        removeFlowItem({ processId, employee: item });
    };

    const flowItems = flows.filter(item => item.processId === processId);

    return (
        <div className="flow-setting">
            {
                window.hasHostPermissionOnFlow &&
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
            }
            {
                flowItems.map(item => {
                    return (
                        <div key={`process${item.processId}-site${item.siteId}-emp${item.empId}`} className="flow-item mt-2">
                            <div className="personal-info">
                                <div><span className="font-weight-bold">{LangHelper.getResource('SiteName')}</span>: <span className="font-italic">{item.siteName}</span></div>
                                <div><span className="font-weight-bold">{LangHelper.getResource('Code')}</span>: <span className="font-italic">{item.code}</span></div>
                                <div><span className="font-weight-bold">{LangHelper.getResource('Nickname')}</span>: <span className="font-italic">{item.nickname}</span></div>
                                <div><span className="font-weight-bold">{LangHelper.getResource('FullName')}</span>: <span className="font-italic">{item.fullName}</span></div>
                            </div>
                            {
                                window.hasHostPermissionOnFlow &&
                                <i className="fa fa-times icon-delete" onClick={() => handleRemove(item)} />
                            }
                        </div>
                    );
                })
            }
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { flows } = state;
    return { flows };
};

export default connect(mapStateToProps, mapDispatchToProps)(FlowItem);