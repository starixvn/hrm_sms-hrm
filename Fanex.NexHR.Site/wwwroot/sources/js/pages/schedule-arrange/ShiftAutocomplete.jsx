import React, { Component } from 'react';
import PropTypes from 'prop-types';

import Autocomplete from 'react-autocomplete';

export default class ShiftAutocomplete extends Component {
    static propTypes = {
        value: PropTypes.string,
        shiftOptions: PropTypes.arrayOf(PropTypes.object),
        name: PropTypes.string,
        className: PropTypes.string,
        isDisabled: PropTypes.bool,
        onChange: PropTypes.func
    };

    static defaultProps = {
        value: '',
        shiftOptions: [],
        name: '',
        className: '',
        isDisabled: false,
        onChange: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            value: props.value === null ? '' : props.value
        };
    }

    componentDidUpdate(prevProps) {
        if (this.props.value !== prevProps.value) {
            this.setState({ value: this.props.value === null ? '' : this.props.value });
        }
    }

    handleChange = event => {
        const value = event.target.value;
        const oldvalue = this.state.value.trim();

        this.setState({ value }, () => {
            if (value.trim().length === 0 && oldvalue.length > 0) {
                this.props.onChange(this.props.name, null);
            }
        });
    }

    handleSelect = (value, item) => {
        this.props.onChange(this.props.name, item);
    }

    handleInputBlur = event => {
        const value = event.target.value.trim().toLowerCase().replace(/ /g, '');

        if (value.length > 0) {
            const index = this.props.shiftOptions.findIndex(item => item.code.toLowerCase().replace(/ /g, '') === value);
            if (index >= 0) {
                this.props.onChange(this.props.name, this.props.shiftOptions[index]);
            }
        }
    }

    render() {
        const { name, className, isDisabled, shiftOptions } = this.props;
        const { value } = this.state;

        return (
            <Autocomplete
                items={shiftOptions}
                shouldItemRender={(item, value) => item.code.toLowerCase().replace(/ /g, '').startsWith(value.trim().toLowerCase().replace(/ /g, ''))}
                getItemValue={item => item.code}
                wrapperStyle={{ display: 'block' }}
                renderInput={(props) => <input type="text" {...props} />}
                renderMenu={(items, value, style) => <div className="items" children={items} />}
                renderItem={(item) => <div className="item" key={item.id}>{item.code}</div>}
                inputProps={{ name: name, className: className, disabled: isDisabled, onBlur: this.handleInputBlur }}
                value={value}
                onChange={this.handleChange}
                onSelect={this.handleSelect}
            />
        );
    }
}