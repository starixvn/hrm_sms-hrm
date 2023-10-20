import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../shared/common.functions';
import { Button, Modal } from 'react-bootstrap';
import { Formik, Form, Field, ErrorMessage } from 'formik';

export const AppContainer = () => {
    const [jobLevel, setJobLevel] = useState([]);
    const [showModal, setShowModal] = useState(false);

    useEffect(() => {
        handleGetJobLevel();
    }, []);

    const handleOpen = () => {
        setShowModal(true);
    };

    const handleClose = () => {
        setShowModal(false);
    };

    const handleSubmit = () => {
        onCreateJobLevel();
        handleClose();
    };

    const handleJobLevelChange = (event) => {
        setJobLevel(event.target.value);
    };

    const handleGetJobLevel = () => {
        axios.post(resolveClientUrl('JobLevel/GetJobLevel'), {
        }).then(response => {
            setJobLevel(response.data)
        }).catch(axiosError);
    }

    const handleDelete = (id) => {
        confirm('Are you sure you want to delete?', () => {
            axios.post(resolveClientUrl('JobLevel/Delete'), {
                id: id
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    alert('Delete successfully!');
                    handleGetJobLevel();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    }

    const handleEdit = (id, name) => {
        setShowModal(true);
    }

    const colSpan = window.hasPermission ? 4 : 2;

    return (
        <Fragment>
            {window.hasPermission &&
                <div className="filter">
                    <Button className="btn btn-request" type="button" onClick={handleOpen}>
                        <span className="fa fa-plus" /> New Job Level
                    </Button>
                </div>
            }
            <Modal show={showModal} onHide={handleClose} >
                <Modal.Header>
                    <Modal.Title>Add Job Level</Modal.Title>
                </Modal.Header>
                <Modal.Body >
                    <Formik
                        validateOnBlur={false}
                        validateOnChange={false}
                        enableReinitialize={true}
                        initialValues={{ name: '' }}
                        validate={(values) => {
                            const errors = {};
                            if (!values.name) {
                                errors.name = 'Required';
                            }
                            return errors;
                        }}
                        onSubmit={handleSubmit}
                    >
                        <Form className="form-horizontal clearfix">
                            <div className="form-group">
                                <label htmlFor="name">Job Level</label>
                                <Field
                                    type="text"
                                    name="name"
                                    className="form-control"
                                    autoComplete="off"
                                />
                                <ErrorMessage name="name" component="div" className="text-danger" />
                            </div>
                        </Form>
                    </Formik>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-primary" onClick={handleSubmit} >
                        Submit
                    </Button>
                    <Button onClick={handleClose}>
                        Cancel
                    </Button>
                </Modal.Footer>
            </Modal>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Name</th>
                        {
                            window.hasPermission &&
                            <Fragment>
                                <th>Edit</th>
                                <th>Delete</th>
                            </Fragment>
                        }
                    </tr>
                </thead>
                <tbody>
                    {jobLevel.length > 0 ? jobLevel.map((item, i) => (
                        <tr id={`row_${i}`} key={item.id}>
                            <td>{i + 1}</td>
                            <td className="align-left">{item.name}</td>
                            {
                                window.hasPermission && <Fragment>
                                    <td>
                                        <a onClick={() => handleEdit(item.id, item.name)}>
                                            <i className="fa fa-pencil fa-lg icon-edit"></i>
                                        </a>
                                    </td>
                                    <td>
                                        <a onClick={() => handleDelete(item.id)}>
                                            <i className="fa fa-times fa-lg icon-delete"></i>
                                        </a>
                                    </td>
                                </Fragment>
                            }
                        </tr>
                    ))
                        :
                        <tr>
                            <td colSpan={colSpan}>No information available</td>
                        </tr>
                    }
                </tbody>
            </table>
        </Fragment>
    )
};