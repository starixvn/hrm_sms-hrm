import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import axios from 'axios';
import { stringify } from 'qs';

import { axiosError, blockUI, unblockUI, reorder, resolveClientUrl } from '../../../shared/common.functions';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Table from '../discipline-manager/table/table';

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
        this.getShiftManagers();
    }

    getShiftManagers = () => {
        blockUI();

        axios(resolveClientUrl('Team/GetShiftManagerBySectionId'), {
            params: {
                sectionId: this.sectionId
            }
        }).then(response => {
            this.setState({ managers: response.data });
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
                        value: `${item.NickName} (${item.FullName})`
                    };
                });

                this.setState({ employeeOptions: employeeOptions });
            }).catch(axiosError);
        } else {
            this.setState({ employeeOptions: [] });
        }
    }

    handleAddEmployee = employee => {
        const index = this.state.managers.findIndex(item => item.ownerId === employee.id);
        this.setState({ employeeOptions: [] });
        if (index < 0) {
            const managers = [...this.state.managers, { ownerId: employee.id, nickname: employee.nickname, fullName: employee.fullName }];
            this.setState({ managers });
        }
    }

    handleRemoveEmployee = employeeId => {
        const managers = [...this.state.managers],
            index = managers.findIndex(item => item.ownerId === employeeId);
        managers.splice(index, 1);
        this.setState({ managers });
    }

    handleChangeOrder = (key, employeeId) => {
        let managers = [...this.state.managers];
        const totalItems = managers.length,
            sourceIndex = managers.findIndex(item => item.ownerId === employeeId);
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

        managers = reorder(managers, sourceIndex, destinationIndex);
        this.setState({ managers });
    }

    handleSave = () => {
        const supervisors = this.state.managers.map((item, index) => {
            return {
                ownerId: item.ownerId,
                displayOrder: index + 1
            };
        });

        axios({
            url: resolveClientUrl('Team/EditSectionSupervisor'),
            method: 'POST',
            data: stringify({
                sectionId: this.sectionId,
                roleId: this.roleId,
                supervisors
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
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

ReactDOM.render(<App />, document.getElementById('shift-wrapper'));