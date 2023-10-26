import React from 'react';
import { Modal } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { isEmpty } from 'lodash';
import Draggable from 'react-draggable';

export const Popup = (props) => {

    const CreateExternalGroup = Yup.object({
        TeamName: Yup.string()
            .trim('The TeamName field is required!')
            .min(3, 'Length of external group must be 3 between and 100.')
            .max(100, 'Length of external group must be 3 between and 100.')
            .required('The TeamName field is required!'),
    });

    const { data, title } = props;
    const teamName = data ? data.teamName : '';
    const teamId = data ? data.teamId : '';
    return (
        <Draggable handle=".modal-header" >
            <Modal
                {...props}
                aria-labelledby="contained-modal-title-vcenter"
            >
                <Modal.Header closeButton>
                    <Modal.Title id="contained-modal-title-vcenter">
                        { title } external group
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Formik
                        validateOnBlur={false}
                        validateOnChange={false}
                        enableReinitialize={true}
                        initialValues={{
                            TeamName: teamName,
                            TeamId: teamId
                        }}
                        validationSchema={CreateExternalGroup}
                        onSubmit={props.onSubmit}
                    >
                        {({ errors }) => (
                            <Form className="form-horizontal">
                                <div className="form-group">
                                    <div className="label-form">Group Name</div>
                                    <div className="input-form">
                                        <Field name="TeamName" className="form-control">
                                            {({
                                                field
                                            }) => (
                                                <input
                                                    autoComplete='off'
                                                    className="form-control"
                                                    type="text"
                                                    {...field}
                                                />
                                            )}
                                        </Field>
                                    </div>
                                </div>
                                {
                                    !isEmpty(errors) && <div className="validation-summary-errors">
                                        <ul>
                                            {errors.TeamName && <li>{errors.TeamName}</li>}
                                        </ul>
                                    </div>
                                }
                                <Modal.Footer style={{ padding: '15px 0 0' }}>
                                    <button type="submit" className="btn btn-primary">Save</button>
                                    <a className="btn btn-default" onClick={props.onHide}>Cancel</a>
                                </Modal.Footer>
                            </Form>
                        )}
                    </Formik>
                </Modal.Body>
            </Modal>
        </Draggable>
    );
}