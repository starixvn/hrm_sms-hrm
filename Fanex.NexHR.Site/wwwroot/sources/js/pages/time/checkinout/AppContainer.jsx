import React, { Component } from 'react';
import { Button, Modal } from 'react-bootstrap';
import axios from 'axios';

import ToastLayout from '../../../shared/layouts/ToastLayout';
import { axiosError, resolveClientUrl } from '../../../shared/common.functions';

export default class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fields: {
                username: '',
                password: ''
            },
            modalDialog: {
                show: false,
                inOutType: 0,
                isSuccessfull: false,
                errMsg: ''
            }
        };
    }

    validate = () => {
        const fields = this.state.fields;
        let isValid = true;
        if (!fields['username'] || !fields['password']) {
            isValid = false;
        }

        return isValid;
    };

    handleCheckInOut = inOutType => {
        const { fields } = this.state;
        const isValid = this.validate();
        if (isValid) {
            axios.post(
                resolveClientUrl('Time/CheckInOut'),
                {
                    username: fields['username'],
                    password: btoa(unescape(encodeURIComponent(fields['password']))),
                    inOutType
                },
                {
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                }
            ).then(response => {
                const { data } = response;
                this.setState({
                    modalDialog: {
                        show: true,
                        inOutType,
                        isSuccessfull: data.isSuccessfull,
                        errMsg: data.errorMessage
                    }
                });
            }).catch(axiosError);
        }
    };

    handleCheckin = event => {
        event.preventDefault();
        this.handleCheckInOut(0);
    };

    handleCheckout = event => {
        event.preventDefault();
        this.handleCheckInOut(1);
    };

    handleChange = event => {
        const fields = { ...this.state.fields };
        fields[event.target.name] = event.target.value;
        this.setState({ fields });
    };

    handleCloseModalDialog = () => {
        const { modalDialog, fields } = this.state;

        this.setState({
            fields: {
                username: modalDialog.isSuccessfull ? '' : fields.username,
                password: ''
            },
            modalDialog: {
                show: false,
                inOutType: 0,
                isSuccessfull: false,
                errMsg: ''
            }
        });
    };

    render() {
        const { modalDialog, fields } = this.state;
        return (
            <div className='app-container'>
                <ToastLayout />
                <div className='checkinout-flex'>
                    <div className='checkinout-form'>
                        <form>
                            <div className='checkinout-logo'>
                                <div className='img' />
                            </div>
                            <div className='material-textfield'>
                                <input type='text' className='custom-input' value={fields.username}
                                    id='username' name='username' placeholder='' autoComplete='off'
                                    onChange={this.handleChange}
                                />
                                <label>Username</label>
                            </div>
                            <div className='material-textfield'>
                                <input type='password' className='custom-input' value={fields.password}
                                    id='password' name='password' placeholder='' autoComplete='off'
                                    onChange={this.handleChange}
                                />
                                <label>Password</label>
                            </div>
                            <div className='checkinout-btn'>
                                <input type='button' className='btn btn-primary' value='Check in' onClick={this.handleCheckin} />
                                <input type='button' className='btn btn-primary' value='Check out' onClick={this.handleCheckout} />
                            </div>
                        </form>
                    </div>
                </div>
                <Modal keyboard={false} backdrop='static' show={modalDialog.show}>
                    <Modal.Body>
                        <i className={`fa ${modalDialog.isSuccessfull ? 'fa-check-circle' : 'fa-exclamation-circle'}`} />
                        <p>{modalDialog.inOutType === 0 ? 'Check in' : 'Check out'} {modalDialog.isSuccessfull ? 'successfully!' : 'unsuccessfully!'}</p>
                        {
                            !modalDialog.isSuccessfull &&
                            <p>{modalDialog.errMsg}</p>
                        }
                    </Modal.Body>
                    <Modal.Footer>
                        <Button bsStyle='primary' type='button' onClick={this.handleCloseModalDialog}>Ok</Button>
                    </Modal.Footer>
                </Modal>
            </div>
        );
    }
}