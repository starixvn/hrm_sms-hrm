import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, resolveClientUrl } from '../../../shared/common.functions';
import { editSectionSupervisor, reorderSupervisor } from './common.functions';
import { RequestType } from '../../../shared/constants';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            supervisors: [],
            employeeOptions: []
        };

        this.sectionId = parseInt(document.getElementById('SectionId').value, 10);
        this.roleId = parseInt(document.getElementById('RoleId').value, 10);
    }

    componentDidMount() {
        this.getSupervisors();
    }

    getSupervisors = () => {
        blockUI();

        axios(resolveClientUrl('Team/GetSectionSupervisorByRoleId'), {
            params: {
                sectionId: this.sectionId,
                roleId: this.roleId
            }
        }).then(response => {
            this.setState({ supervisors: response.data });
        }).catch(axiosError).then(() => unblockUI());
    }

    getEmployees = query => {
        if (query.trim().length > 1) {
            axios(resolveClientUrl('employee/getemployees'), {
                params: {
                    query,
                    enable: true,
                    isDelete: false
                }
            }).then(response => {
                const employeeOptions = response.data.map(item => {
                    return {
                        id: item.Id,
                        nickname: item.NickName,
                        fullName: item.FullName,
                        team: item.Team !== null ? item.Team.Code : '',
                        value: item.DisplayName
                    };
                });

                this.setState({ employeeOptions: employeeOptions });
            }).catch(axiosError);
        } else {
            this.setState({ employeeOptions: [] });
        }
    }

    handleAddEmployee = employee => {
        const index = this.state.supervisors.findIndex(item => item.ownerId === employee.id);
        this.setState({ employeeOptions: [] });
        if (index < 0) {
            const supervisors = [...this.state.supervisors, {
                ownerId: employee.id,
                nickName: employee.nickname,
                fullName: employee.fullName,
                team: employee.team,
                leaves: true,
                changeShifts: true,
                lateInEarlyOuts: true,
                checkInOuts: true,
                overtimes: true,
                keepInViews: true
            }];
            this.setState({ supervisors });
        }
    }

    handleRemoveEmployee = employeeId => {
        const supervisors = [...this.state.supervisors],
            index = supervisors.findIndex(item => item.ownerId === employeeId);
        supervisors.splice(index, 1);
        this.setState({ supervisors });
    }

    handleChangePermissionOnRequest = (employeeId, requestType, isChecked) => {
        const supervisors = [...this.state.supervisors],
            index = supervisors.findIndex(item => item.ownerId === employeeId);
        supervisors[index] = { ...supervisors[index], [requestType]: isChecked };
        this.setState({ supervisors });
    }

    handleChangeOrder = (key, employeeId) => {
        let vm = this;
        reorderSupervisor(key, employeeId, [...this.state.supervisors], function (supervisors) {
            vm.setState({ supervisors });
        });
    }

    handleSave = () => {
        const supervisors = [];
        const requestTypes = [
            { key: 'leaves', value: RequestType.Leave },
            { key: 'changeShifts', value: RequestType.ChangeShift },
            { key: 'lateInEarlyOuts', value: RequestType.LateIn },
            { key: 'lateInEarlyOuts', value: RequestType.EarlyOut },
            { key: 'checkInOuts', value: RequestType.CheckIn },
            { key: 'checkInOuts', value: RequestType.CheckOut },
            { key: 'overtimes', value: RequestType.Overtime },
            { key: 'keepInViews', value: RequestType.KeepInView }
        ];
        this.state.supervisors.forEach((item, index) => {
            const ownerId = item.ownerId;
            const displayOrder = index + 1;
            requestTypes.forEach(requestType => {
                if (item[requestType.key]) {
                    supervisors.push({
                        ownerId,
                        requestType: requestType.value,
                        displayOrder
                    });
                }
            });
        });

        editSectionSupervisor(this.sectionId, this.roleId, supervisors).then(response => {
            const data = response.data;
            if (data.isSuccessful) {
                toast.success('Updated Successfully!');
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        const { employeeOptions, supervisors } = this.state;

        return (
            <Fragment>
                <ToastLayout />
                <Table data={supervisors} roleId={this.roleId}
                    employeeOptions={employeeOptions}
                    onSearchEmployees={this.getEmployees}
                    onAddEmployee={this.handleAddEmployee}
                    onChangePermissionOnRequest={this.handleChangePermissionOnRequest}
                    onRemoveEmployee={this.handleRemoveEmployee}
                    onChangeOrder={this.handleChangeOrder}
                />
                <Button onClick={this.handleSave} bsStyle="primary">Save</Button>
                <a className="btn btn-default" href={resolveClientUrl(`team/TeamSupervisor/${this.sectionId}`)}>Back</a>
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('supervisor-wrapper'));