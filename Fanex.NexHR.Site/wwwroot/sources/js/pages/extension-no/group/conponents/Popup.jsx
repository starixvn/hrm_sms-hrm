import React, { Fragment} from 'react';
import 'jquery-ui-bundle/jquery-ui';
import { Modal } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { isEmpty } from 'lodash';
import Draggable from 'react-draggable';

export const Popup = (props) => {
    const CreateExternalGroup = Yup.object({
        TeamName: Yup.string()
            .trim()
            .min(3, 'Length of external group must be 3 between and 100.')
            .max(100, 'Length of external group must be 3 between and 100.')
            .required('The TeamName field is required!'),
    });
    const { data, title } = props;
    const teamName = data ? data.teamName : '';
    const teamId = data ? data.teamId : '';
    const renderForm = (errors) => {
        return (
            <Fragment>
                {
                    !isEmpty(errors) && <div className="validation-summary-errors">
                        <ul>
                            {errors.TeamName && <li>{errors.TeamName}</li>}
                        </ul>
                    </div>
                }
                <div className="form-group" style={{ marginBottom : '0' }}>
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
            </Fragment>
        )
    }

    return (
        <Draggable handle=".modal-header" >
            <Modal
                {...props}
                aria-labelledby="contained-modal-title-vcenter"
            >
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
                            <Modal.Header closeButton>
                                <Modal.Title id="contained-modal-title-vcenter">
                                    {title} external group
                                </Modal.Title>
                            </Modal.Header>
                            <Modal.Body>
                                {renderForm(errors)}
                            </Modal.Body>
                            <Modal.Footer>
                                <button type="submit" className="btn btn-primary">Save</button>
                                <a className="btn btn-default" onClick={props.onHide}>Cancel</a>
                            </Modal.Footer>
                        </Form>
                    )}
                </Formik>
            </Modal>
        </Draggable>
    );
}