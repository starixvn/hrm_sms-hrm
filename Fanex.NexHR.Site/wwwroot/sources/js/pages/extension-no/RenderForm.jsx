import React from 'react';
import { Form, Field } from 'formik';

import Autocomplete from '../../shared/components/Autocomplete';
import Errors from '../../shared/components/Errors';

const RenderFrom = ({ errors, isInternal, onChangeInternal, selectExtension, employee, getEmployees, onChangeEmployee , isSubmitting }) => {
    const handleGoBack = () => {
        history.back();
    };
    return (
        <div className="card">
            <Errors errors={errors} />
            <Form className="form-horizontal">
                <input name="EmpId" hidden />
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Is Internal</label>
                    </div>
                    <div className="input-form">
                        <Field name="IsInternal" className="form-control">
                            {({
                                field
                            }) => (
                                <input
                                    checked={isInternal}
                                    type="checkbox"
                                    {...field}
                                    onChange={onChangeInternal}
                                />
                            )}
                        </Field>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Group</label>
                    </div>
                    <div className="input-form">
                        <Field as="select" name="TeamId" className="form-control">
                            {
                                selectExtension && selectExtension.map(item => (
                                    <option defaultValue={item.value} value={item.value} key={item.value}>{item.text}</option>
                                ))
                            }
                        </Field>
                    </div>
                </div>
                {
                    isInternal && <div className="form-group">
                        <div className="label-form">
                            <label className="control-label">Employee</label>
                        </div>
                        <div className="select-form">
                            <Field type="text" name="Employee" className="form-control">
                                {({
                                    field, form
                                }) => (
                                    <Autocomplete
                                        className='form-control'
                                        sources={employee}
                                        onInput={getEmployees}
                                        field={field}
                                        form={form}
                                        nameInput='Employee'
                                        onChange={onChangeEmployee}
                                    />
                                )}
                            </Field>
                        </div>
                    </div>
                }

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Full Name</label>
                    </div>
                    <div className="input-form">
                        <Field name="FullName">
                            {({
                                field
                            }) => (
                                <input
                                    autoComplete='off'
                                    className="form-control"
                                    type="text"
                                    {...field}
                                    readOnly={isInternal || false}
                                />
                            )}
                        </Field>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Nickname</label>
                    </div>
                    <div className="input-form">
                        <Field name="NickName" className="form-control">
                            {({
                                field
                            }) => (
                                <input
                                    autoComplete='off'
                                    className="form-control"
                                    type="text"
                                    {...field}
                                    readOnly={isInternal || false}
                                />
                            )}
                        </Field>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Extension No</label>
                    </div>
                    <div className="input-form">
                        <Field type="text" name="ExtensionNo" className="form-control" autoComplete='off' />
                    </div>
                </div>
                <div className="form-group">
                    <button type="submit" id="btnSubmit" className="btn btn-primary" disabled={isSubmitting} style={{ marginRight: "5px" }}>Update</button>
                    <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                </div>
            </Form>
        </div>
    );
};

export default RenderFrom;