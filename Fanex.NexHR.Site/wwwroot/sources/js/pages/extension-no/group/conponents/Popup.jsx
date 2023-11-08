import React, { Fragment} from 'react';
import { Modal } from 'react-bootstrap';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import Draggable from 'react-draggable';

import Errors from '../../../../shared/components/Errors';

const Popup = (props) => {
    const CreateExternalGroup = Yup.object({
        TeamName: Yup.string()
            .trim()
            .min(3, 'Length of external group must be 3 between and 100.')
            .max(100, 'Length of external group must be 3 between and 100.')
            .required('The TeamName field is required!'),
    });
    const { data, title, onHide, onSubmit } = props;
    const teamName = data ? data.teamName : '';
    const teamId = data ? data.id : '';
    const renderForm = ({ errors, isSubmitting }) => {
        return (
            <Fragment>
                <Form className="form-horizontal">
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            {title} external group
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Errors errors={errors} />
                        <div className="form-group" style={{ marginBottom: '0' }}>
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
                    </Modal.Body>
                    <Modal.Footer>
                        <button type="submit" className="btn btn-primary" disabled={isSubmitting}>Save</button>
                        <a className="btn btn-default" onClick={onHide}>Cancel</a>
                    </Modal.Footer>
                </Form>
            </Fragment>
        )
    }

    return (
        <Draggable handle=".modal-header" >
            <Modal
                {...props}
                keyboard={false}
                backdrop="static"
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
                    onSubmit={onSubmit}
                >
                    {renderForm}
                </Formik>
            </Modal>
        </Draggable>
    );
}

export default Popup;