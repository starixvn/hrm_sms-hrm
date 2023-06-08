import React, { PureComponent } from 'react';
import { Button, Form } from 'react-bootstrap';
import Select from 'react-select';
import LangHelper from '@jsroot/common/langHelper';
import { DefaultPageSize } from '@jsroot/common/constants';

export default class Filter extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            status: { value: 1, label: 'Active' }
        };
    }

    componentDidMount() {
        const params = this.getParams();
        this.props.getPeriodInfos(params);
    }

    getParams = () => {
        const { status } = this.state;
        return {
            IsActive: status.value,
            pageIndex: 1,
            pageSize: DefaultPageSize
        };
    };

    handleSubmit = event => {
        event.preventDefault();
        const params = this.getParams();
        this.props.onFilterChange(params);
        this.props.getPeriodInfos(params);
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    handleOpenCreateModalDialog = () => {
        this.props.handleModalCreateDialog(true);
    };

    render() {
        const { status } = this.state;
        const statusOptions = [{ value: null, label: 'All' },
        { value: 1, label: 'Active' },
        { value: 0, label: 'Inactive' }
        ];

        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Status')}</Form.Label>
                        <Select className={`react-select-container`}
                            classNamePrefix="react-select" isSearchable={false}
                            options={statusOptions} value={status}
                            onChange={value => this.handleChange('status', value)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={this.handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                    </Form.Group>
                    <Form.Group>
                        <Button variant="primary" size="sm" className="mr-3" onClick={this.handleOpenCreateModalDialog}>
                            {LangHelper.getResource('AddNew')}
                        </Button>
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}