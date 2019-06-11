import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, blockUI, unblockUI } from '../../../shared/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            managers: [],
            employeeOptions: []
        };

        this.teamId = document.getElementById('TeamId').value;
        this.roleId = document.getElementById('RoleId').value;
    }

    componentDidMount() {
        this.getDisciplineManagers();
    }

    getDisciplineManagers = () => {
        blockUI();

        axios(_global.root + 'Team/GetDisciplineManagerByTeamId', {
            params: {
                teamId: this.teamId
            }
        }).then(response => {
            this.setState({ managers: response.data });
        }).catch(axiosError).then(() => unblockUI());
    }

    getEmployees = query => {
        axios(_global.root + 'employee/GetSupervisors', {
            params: {
                roleIds: this.roleId,
                query: query
            }
        }).then(response => {
            let employeeOptions = response.data.map(item => {
                return {
                    id: item.Id,
                    nickname: item.NickName,
                    fullName: item.FullName,
                    value: `${item.NickName} (${item.FullName})`
                };
            });

            this.setState({ employeeOptions: employeeOptions });
        }).catch(axiosError);
    }

    handleAddEmployee = employee => {
        let managers = [...this.state.managers, { ownerId: employee.id, nickname: employee.nickname, fullName: employee.fullName }];
        this.setState({ managers });
    }

    handleRemoveEmployee = employeeId => {
        let managers = [...this.state.managers];
        const index = managers.findIndex(item => item.ownerId === employeeId);
        managers.splice(index, 1);
        this.setState({ managers });
    }

    reorder = (managers, startIndex, endIndex) => {
        const [removed] = managers.splice(startIndex, 1);
        managers.splice(endIndex, 0, removed);

        return managers;
    }

    handleChangeOrder = (key, employeeId) => {
        let managers = [...this.state.managers];
        const totalItems = managers.length;
        let sourceIndex = managers.findIndex(item => item.ownerId === employeeId);
        let destinationIndex = 0;

        switch (key) {
            case 'up':
                destinationIndex = sourceIndex - 1;
                break;
            case 'down':
                destinationIndex = sourceIndex + 1;
                break;
            case 'top':
                destinationIndex = 0;
                break;
            case 'bottom':
                destinationIndex = totalItems;
                break;
        }

        if (destinationIndex < 0 || destinationIndex > totalItems || sourceIndex === destinationIndex) {
            return;
        }

        managers = [...this.reorder(managers, sourceIndex, destinationIndex)];
        this.setState({ managers });
    }

    handleSave = () => {
        const managerIds = this.state.managers.map(item => item.ownerId).join(',');

        axios({
            url: _global.root + 'Team/EditDisciplineManager',
            method: 'POST',
            data: stringify({ teamId: this.teamId, supervisorIds: managerIds }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated Successfully!');
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        const { employeeOptions, managers } = this.state;

        return (
            <Fragment>
                <ToastLayout />
                <Table data={managers}
                    employeeOptions={employeeOptions}
                    onSearchEmployees={this.getEmployees}
                    onAddEmployee={this.handleAddEmployee}
                    onRemoveEmployee={this.handleRemoveEmployee}
                    onChangeOrder={this.handleChangeOrder}
                />
                <Button onClick={this.handleSave} bsStyle="primary">Save</Button>
                <a className="btn btn-default" href={_global.root + `team/TeamSupervisor/${this.teamId}`}>Back</a>
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('discipline-wrapper'));