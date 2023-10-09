import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import Otp from '../Otp';

describe('Otp', () => {
    afterEach(() => {
        jest.clearAllMocks();
    });

    it('renders correctly', () => {
        expect(render(<Otp accountOTP={{ enabled: true }} onChangeStatusOTP={jest.fn()} onChangeSecretKey={jest.fn()} />)).toMatchSnapshot();
    });
});