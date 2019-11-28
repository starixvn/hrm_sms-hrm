import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, resolveClientUrl } from '../../../shared/common.functions';
import { getSupervisors, editSectionSupervisor, reorderSupervisor } from '../supervisor/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            managers: [],
            employeeOptions: []
        };

        this.sectionId = document.getElementById('SectionId').value;
        this.roleId = document.getElementById('RoleId').value;
    }

    componentDidMount() {
        this.getDisciplineManagers();
    }

    getDisciplineManagers = () => {
        blockUI();

        axios(resolveClientUrl('Team/GetDisciplineManagerBySectionId'), {
            params: {
                sectionId: this.sectionId
            }
        }).then(response => {
            this.setState({ managers: response.data });
        }).catch(axiosError).then(() => unblockUI());
    }

    getEmployees = query => {
        if (query.trim().length > 1) {
            const excludeIds = this.state.managers.map(item => item.ownerId).join(',');

            getSupervisors(query, this.roleId, excludeIds).then(response => {
                const employeeOptions = response.data.map(item => {
                    return {
                        id: item.Id,
                        nickname: item.NickName,
                        fullName: item.FullName,
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
        const managers = [...this.state.managers, { ownerId: employee.id, nickname: employee.nickname, fullName: employee.fullName }];
        this.setState({ managers, employeeOptions: [] });
    }

    handleRemoveEmployee = employeeId => {
        const managers = [...this.state.managers],
            index = managers.findIndex(item => item.ownerId === employeeId);
        managers.splice(index, 1);
        this.setState({ managers });
    }

    handleChangeOrder = (key, employeeId) => {
        let vm = this;
        reorderSupervisor(key, employeeId, [...this.state.managers], function (managers) {
            vm.setState({ managers });
        });
    }

    handleSave = () => {
        const supervisors = this.state.managers.map((item, index) => {
            return {
                ownerId: item.ownerId,
                displayOrder: index + 1
            };
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
                <a className="btn btn-default" href={resolveClientUrl(`team/TeamSupervisor/${this.sectionId}`)}>Back</a>
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('discipline-wrapper'));