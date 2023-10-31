import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import { isEmpty } from 'lodash';
import Draggable from 'react-draggable';
import * as Yup from 'yup';

const validationSchema = Yup.object({
    name: Yup.string().trim().required('Job Level is required!')
});

export const Popup = (props) => {
    const { data, title } = props;

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
                    <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Job Level</label>
                        </div>
                        <div className="input-form">
                            <Field name="name" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                </div>
            </div>
        )
    }

    return (
        <Draggable handle=".modal-header" >
            <Modal {...props} keyboard={false} backdrop="static">
                <Formik
                    validateOnBlur={false}
                    validateOnChange={false}
                    initialValues={{ name: data ? data.name : '' }}
                    validationSchema={validationSchema}
                    onSubmit={props.onSubmit}
                >
                    {({ errors }) => (
                        <Form>
                            <Modal.Header>
                                <Modal.Title>{title} Job Level</Modal.Title>
                            </Modal.Header>
                            <Modal.Body>
                                {renderForm(errors)}
                            </Modal.Body>
                            <Modal.Footer>
                                <Button bsStyle="primary" type="submit">Submit</Button>
                                <Button className="btn-cancel" onClick={props.onHide}>Cancel</Button>
                            </Modal.Footer>
                        </Form>
                    )}
                </Formik>
            </Modal>
        </Draggable>
    )
};