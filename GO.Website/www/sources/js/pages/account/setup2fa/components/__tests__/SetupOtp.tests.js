import React from 'react';
import { render} from '@testing-library/react';
import '@testing-library/jest-dom';
import SetupOtp from '../SetupOtp';

describe('SetupOtp', () => {
    afterEach(() => {
        jest.clearAllMocks();
    });

    it('renders correctly', () => {
        var setupOtp = {
            textCode: '',
            otp: '',
            email: '',
            appName: '',
            qrCodeText: ''
        };
        expect(render(<SetupOtp verifySetupOtp={jest.fn()} setupOtp={setupOtp} isShowMessageVerifyOtp={false} isVerifyOtp={false} messageError={null}/>)).toMatchSnapshot();
    });
});