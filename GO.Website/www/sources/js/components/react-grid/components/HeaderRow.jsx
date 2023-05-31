import React, { Component } from 'react';
import PropTypes from 'prop-types';

export default class HeaderRow extends Component {
    static propTypes = {
        onSort: PropTypes.func
    };

    static defaultProps = {
        onSort: null
    };

    constructor(props) {
        super(props);
    }

    _renderChildren() {
        const { onSort, children, sortBy, sortOrder } = this.props;

        return React.Children.map(children, child => child && React.cloneElement(child, { onSort: onSort, sortOrder: sortBy === child.props.sortBy ? sortOrder : null }));
    }

    render() {
        const { onSort, sortBy, sortOrder, ...rest } = this.props;
        return (
            <tr {...rest}>{this._renderChildren()}</tr>
        );
    }
}