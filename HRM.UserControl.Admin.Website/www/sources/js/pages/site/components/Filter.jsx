import LangHelper from '@jsroot/common/langHelper';
import React, { PureComponent } from 'react';
import { Button, Form } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';
import { ActionCreators } from '../actions';
import { ActiveStatusOptions } from '@jsroot/common/constants';

export class Filter extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            status: ActiveStatusOptions[0]
        };
    }

    getParams = () => {
        return {
            isActive: this.state.status.value
        };
    };

    handleSubmit = event => {
        event.preventDefault();
        const params = this.getParams();
        this.props.setGetAllSiteParams(params);
        this.props.getAllSite(params);
    };

    handleOpenCreateModalDialog = () => {
        this.props.setCreateModalDialog({ show: true });
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    render() {
        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Status')}</Form.Label>
                        <Select className='react-select-container' classNamePrefix="react-select" isSearchable={false}
                            options={ActiveStatusOptions} value={this.state.status}
                            onChange={value => this.handleChange('status', value)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={this.handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                        <Button variant="primary" size="sm" onClick={this.handleOpenCreateModalDialog}>
                            {LangHelper.getResource('Create')}
                        </Button>
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAllSiteParams } = state;
    return { getAllSiteParams };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);