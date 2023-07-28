import React from 'react';
import { AppContainer } from '../AppContainer';
import { HeaderCell, Row, Cell } from '@jsroot/components/react-grid/index';
import { shallow } from 'enzyme';

describe('DataGrid', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    describe('buildHeader', () => {
        it('should render expected header', () => {
            // Act
            const container = shallow(<AppContainer />).instance();
            // Assert
        });
    });

});