import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import { isEmpty } from 'lodash';
import axios from 'axios';
import { resolveClientUrl, axiosError, alert } from '../../../shared/common.functions';
import Draggable from 'react-draggable';
import * as Yup from 'yup';

const CreateSchema = Yup.object({
    name: Yup.string().trim().required('Job Title is required!')
});

export const Create = (props) => {
    const { createModal, setCreateModal, reloadTable, jobCategory } = props;

    const handleClose = () => {
        setCreateModal(false);
    };

    const onSubmit = (values) => {
        axios.post(resolveClientUrl(`JobTitle/Insert`), {
            name: values.name.trim(),
            categoryId: values.categoryId
        })
            .then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    handleClose();
                    alert('Create successfully!');
                    reloadTable();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
    };

    const renderForm = (errors) => {
        return (
            <div className="card">
                {
                    !isEmpty(errors) && <div className="validation-summary-errors">
                        <ul>
                            {errors.name && <li>{errors.name}</li>}
                        </ul>
                    </div>
                }
                <div className="form-horizontal clearfix">
                    <div className="form-group form-modal ">
                        <div className="label-form">
                            <label className="control-label">Job Title</label>
                        </div>
                        <div className="input-form">
                            <Field name="name" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                    <div className="form-group form-modal">
                        <div className="label-form">
                            <label className="control-label">Category</label>
                        </div>
                        <div className="select-form">
                            <Field name="category" as="select" className="form-control">
                                {jobCategory.map((category, i) => (
                                    <option key={i} value={category.value}>
                                        {category.text}
                                    </option>
                                ))}
                            </Field>
                        </div>
                    </div>
                </div>
            </div>
        )
    }

    return (
        <Draggable handle=".modal-header" >
            <Modal show={createModal} onHide={handleClose} keyboard={false} backdrop="static">
                <Formik
                    validateOnBlur={false}
                    validateOnChange={false}
                    initialValues={{ name: '', categoryId: 0 }}
                    validationSchema={CreateSchema}
                    onSubmit={onSubmit}
                >
                    {({ errors }) => (
                        <Form>
                            <Modal.Header>
                                <Modal.Title>Add Job Title</Modal.Title>
                            </Modal.Header>
                            <Modal.Body>
                                {renderForm(errors)}
                            </Modal.Body>
                            <Modal.Footer>
                                <Button bsStyle="primary" type="submit">Submit</Button>
                                <Button className="btn-cancel" onClick={handleClose}>Cancel</Button>
                            </Modal.Footer>
                        </Form>
                    )}
                </Formik>
            </Modal>
        </Draggable>
    )
};