import React, { Component } from 'react';
import PropTypes from 'prop-types';

class RowLayout extends Component {
    static propTypes = {
        columnLayout: PropTypes.arrayOf(PropTypes.oneOfType([PropTypes.number, PropTypes.string])).isRequired
    };

    constructor(props) {
        super(props);
    }

    _getRowLayout() {
        const { columnLayout } = this.props;

        return columnLayout.map((cellWidth, index) => {
            const width = isNaN(cellWidth) ? cellWidth : `${cellWidth}px`;

            return (<td key={`cellLayout${index}`} style={{ width: width }}></td>);
        });
    }

    render() {
        return (
            <tr className="first-row">
                {this._getRowLayout()}
            </tr>
        );
    }
}

export default RowLayout;