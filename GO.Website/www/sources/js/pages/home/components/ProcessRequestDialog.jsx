import React, { Component } from 'react';
import { Button, Col, Form, Modal, Row, Table } from 'react-bootstrap';
import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import DateTimeService from '@jsroot/common/DateTimeService';

export default class ProcessRequestDialog extends Component {
    constructor(props) {
        super(props);

        const action = { ...props.action };

        this.state = {
            siteId: props.siteId,
            action: {
                ...action,
                metadata: {
                    ...action.metadata,
                    comment: action.metadata.comment ?? ''
                }
            },
            isSubmitting: false
        };
    }

    handleChangeComment = event => {
        const action = this.state.action;

        this.setState({
            action: {
                ...action,
                metadata: {
                    ...action.metadata,
                    comment: event.target.value
                }
            }
        });
    };

    handleChangeIgnore = (event, index) => {
        const action = this.state.action;

        const disciplines = [...action.metadata.disciplines];
        disciplines[index] = {
            ...disciplines[index],
            isIgnore: event.target.checked
        };
        this.setState({
            action: {
                ...action,
                metadata: {
                    ...action.metadata,
                    disciplines
                }
            }
        });
    };

    handleSubmit = async () => {
        this.setState({ isSubmitting: true });

        const url = Site.resolveClientUrl('home/processAction');
        const params = {
            siteId: this.state.siteId,
            action: this.state.action
        };
        await DataService.postDataAsync(url, params).then(response => {
            if (response.isSuccessful) {

            }
        });
    };

    render() {
        const { action, isSubmitting } = this.state;
        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.props.onClose}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{action.name}</Modal.Title>
                </Modal.Header>
                <Form noValidate onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        {
                            action.metadata.disciplines && action.metadata.disciplines.length > 0 &&
                            <Form.Group as={Row}>
                                <Form.Label column sm="4" md="3" className="required" htmlFor="name">
                                    {LangHelper.getResource('Discipline')}
                                </Form.Label>
                                <Col sm="8" md="9">
                                    <Table striped bordered size="sm">
                                        <thead>
                                            <tr>
                                                <th>Date</th>
                                                <th>Type</th>
                                                <th>Details</th>
                                                <th>Ignore</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {
                                                action.metadata.disciplines.map((item, index) =>
                                                    <tr key={index}>
                                                        <td className="text-center">{DateTimeService.toShortDateFormat(item.workDate)}</td>
                                                        <td>{item.name}</td>
                                                        <td>
                                                            <div dangerouslySetInnerHTML={{ __html: item.details }} />
                                                        </td>
                                                        <td className="text-center">
                                                            <input type="radio"
                                                                name={`action.metadata.disciplines[${index}].name`}
                                                                checked={action.isIgnore}
                                                                onChange={event => this.handleChangeIgnore(event, index)}
                                                            />
                                                        </td>
                                                    </tr>
                                                )
                                            }
                                        </tbody>
                                    </Table>
                                </Col>
                            </Form.Group>
                        }
                        <Form.Group as={Row}>
                            <Form.Label column sm="4" md="3" htmlFor="comment">
                                {LangHelper.getResource('Comment')}
                            </Form.Label>
                            <Col sm="8" md="9">
                                <Form.Control as="textarea" rows={3} size="sm" value={action.metadata.comment}
                                    name="comment" id="comment"
                                    className="shadow-none" autoComplete="off"
                                    onChange={this.handleChangeComment}
                                />
                            </Col>
                        </Form.Group>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button type="submit" variant="primary" size="sm" disabled={isSubmitting}>
                            {action.name}
                        </Button>
                        <Button variant="secondary" size="sm" onClick={this.props.onClose}>
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}