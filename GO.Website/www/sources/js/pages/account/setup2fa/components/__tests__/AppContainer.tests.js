import { toast } from 'react-toastify';
import React from 'react';
import { render, waitFor, fireEvent, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import AppContainer from '../AppContainer';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

describe('AppContainer', () => {
    beforeEach(() => {
        LangHelper.getResource = jest.fn(key => key);
        Site.resolveClientUrl = jest.fn(url => url);
        Site.blockUI = jest.fn(url => url);
        Site.unblockUI = jest.fn(url => url);
        toast.success = jest.fn();
        toast.error = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('renders correctly', () => {
        expect(render(<AppContainer />)).toMatchSnapshot();
    });

    test('show modal dialog render successfully ', async () => {
        // ARRANGE
        const response = {
            data: {
                userId: 1,
                siteId: 1,
                secretKey: 'test',
                recoveryCode: 'test',
                enabled: true,
                isRegistered: true
            }
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        fireEvent.click(screen.getByText('ChangeSecretKey'));
        const elementCancel = screen.getByText('Cancel');
        const elementOK = screen.getByText('OK');

        // ASSERT
        expect(elementCancel).toBeInTheDocument();
        expect(elementOK).toBeInTheDocument();
    });

    test('close modal dialog render successfully ', async () => {
        // ARRANGE
        const response = {
            data: {
                userId: 1,
                siteId: 1,
                secretKey: 'test',
                recoveryCode: 'test',
                enabled: true,
                isRegistered: true
            }
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        fireEvent.click(screen.getByText('ChangeSecretKey'));
        fireEvent.click(screen.getByText('Cancel'));
        const elementChangeSecretKey = screen.getByText('ChangeSecretKey');

        // ASSERT
        expect(elementChangeSecretKey).toBeInTheDocument();
    });

    test('reset setup otp render successfully ', async () => {
        // ARRANGE
        const responseAccountOTP = {
            data: {
                userId: 1,
                siteId: 1,
                secretKey: 'test',
                recoveryCode: 'test',
                enabled: true,
                isRegistered: true
            }
        };
        const responseResetSetupOtp = {
            data: true
        };
        const promiseAccountOTP = Promise.resolve(responseAccountOTP);
        const ResetSetupOtp = Promise.resolve(responseResetSetupOtp);
        DataService.postDataAsync = jest.fn();
        DataService.postDataAsync.mockReturnValueOnce(promiseAccountOTP).mockReturnValueOnce(ResetSetupOtp).mockReturnValue(promiseAccountOTP);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        fireEvent.click(screen.getByText('ChangeSecretKey'));
        fireEvent.click(screen.getByText('OK'));

        // ASSERT
        expect(DataService.postDataAsync).toBeCalledTimes(2);
    });

    test('switch enable using otp render successfully ', async () => {
        // ARRANGE
        const response = {
            data: {
                userId: 1,
                siteId: 1,
                secretKey: 'test',
                recoveryCode: 'test',
                enabled: true,
                isRegistered: true
            }
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn();
        DataService.postDataAsync.mockReturnValueOnce(promise).mockReturnValue(promise);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        fireEvent.click(screen.getByTestId('customSwitchesChecked'));
        const elementCustomSwitchesChecked = screen.getByTestId('customSwitchesChecked');

        // ASSERT
        expect(elementCustomSwitchesChecked.defaultChecked).toEqual(true);
    });

    test('setup otp render successfully ', async () => {
        // ARRANGE
        const responseAccountOTP = {
            data: null
        };
        const responseSetupOtp = {
            data: {
                userId: 1,
                siteId: 1,
                secretKey: 'test',
                recoveryCode: 'test',
                enabled: false,
                isRegistered: false
            }
        };
        const promiseAccountOTP = Promise.resolve(responseAccountOTP);
        const promiseSetupOtp = Promise.resolve(responseSetupOtp);
        DataService.postDataAsync = jest.fn();
        DataService.postDataAsync.mockReturnValueOnce(promiseAccountOTP).mockReturnValue(promiseSetupOtp);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        // ASSERT
        expect(DataService.postDataAsync).toBeCalledTimes(2);
    });

    test('click verify otp return true render successfully ', async () => {
        // ARRANGE
        const responseAccountOTP = {
            data: null
        };
        const responseSetupOtp = {
            data: {
                textCode: 'test',
                otp: '123456',
                tmail: 'test',
                appName: 'test',
                qrCodeText: 'test'
            }
        };
        const responseVerifySetupOtp = {
            data: true
        };
        const promiseAccountOTP = Promise.resolve(responseAccountOTP);
        const promiseSetupOtp = Promise.resolve(responseSetupOtp);
        const promiseVerifySetupOtp = Promise.resolve(responseVerifySetupOtp);
        DataService.postDataAsync = jest.fn();
        DataService.postDataAsync.mockReturnValueOnce(promiseAccountOTP).mockReturnValueOnce(promiseSetupOtp).mockReturnValue(promiseVerifySetupOtp);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        fireEvent.click(screen.getByText('Verify'));

        // ASSERT
        expect(DataService.postDataAsync).toBeCalledTimes(3);
    });

    test('click verify otp return false render successfully ', async () => {
        // ARRANGE
        const responseAccountOTP = {
            data: null
        };
        const responseSetupOtp = {
            data: {
                textCode: 'test',
                otp: '123456',
                tmail: 'test',
                appName: 'test',
                qrCodeText: 'test'
            }
        };
        const responseVerifySetupOtp = {
            data: false
        };
        const promiseAccountOTP = Promise.resolve(responseAccountOTP);
        const promiseSetupOtp = Promise.resolve(responseSetupOtp);
        const promiseVerifySetupOtp = Promise.resolve(responseVerifySetupOtp);
        DataService.postDataAsync = jest.fn();
        DataService.postDataAsync.mockReturnValueOnce(promiseAccountOTP).mockReturnValueOnce(promiseSetupOtp).mockReturnValue(promiseVerifySetupOtp);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        fireEvent.click(screen.getByText('Verify'));

        // ASSERT
        expect(DataService.postDataAsync).toBeCalledTimes(3);
    });

    test('click verify otp is empty render successfully ', async () => {
        // ARRANGE
        const responseAccountOTP = {
            data: null
        };
        const responseSetupOtp = {
            data: {
                textCode: 'test',
                otp: '',
                tmail: 'test',
                appName: 'test',
                qrCodeText: 'test'
            }
        };
        const responseVerifySetupOtp = {
            data: false
        };
        const promiseAccountOTP = Promise.resolve(responseAccountOTP);
        const promiseSetupOtp = Promise.resolve(responseSetupOtp);
        const promiseVerifySetupOtp = Promise.resolve(responseVerifySetupOtp);
        DataService.postDataAsync = jest.fn();
        DataService.postDataAsync.mockReturnValueOnce(promiseAccountOTP).mockReturnValueOnce(promiseSetupOtp).mockReturnValue(promiseVerifySetupOtp);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        fireEvent.click(screen.getByText('Verify'));

        // ASSERT
        expect(DataService.postDataAsync).toBeCalledTimes(2);
    });
});