import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';

import Autocomplete from 'react-autocomplete';

export default class ShiftAutocomplete extends PureComponent {
    static propTypes = {
        value: PropTypes.string,
        shiftOptions: PropTypes.arrayOf(PropTypes.object),
        name: PropTypes.string,
        className: PropTypes.string,
        placeholder: PropTypes.string,
        isDisabled: PropTypes.bool,
        onChange: PropTypes.func
    };

    static defaultProps = {
        value: '',
        shiftOptions: [],
        name: '',
        className: '',
        placeholder: '',
        isDisabled: false,
        onChange: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            value: props.value === null ? '' : props.value,
            options: [],
            isPasted: false
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
        const isPasted = this.state.isPasted;
        const shiftOptions = this.props.shiftOptions;
        let options = [];

        if (value.trim().length > 0 && !isPasted) {
            options = shiftOptions.filter(item => item.code.toLowerCase().replace(/ /g, '').startsWith(value.trim().toLowerCase().replace(/ /g, '')));
        }

        this.setState({
            value,
            options,
            isPasted: false
        }, () => {
            if (value.trim().length === 0 && oldvalue.length > 0) {
                this.props.onChange(this.props.name, null);
            }
        });
    }

    handleSelect = (value, item) => {
        this.setState({ value });

        this.props.onChange(this.props.name, item);
    }

    handleInputFocus = event => {
        event.target.select();
        this.setState({ options: [] });
    }

    handleInputPaste = event => {
        this.setState({ isPasted: true });
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
        const { name, className, isDisabled, placeholder } = this.props;
        const { value, options } = this.state;

        return (
            <Autocomplete
                items={options}
                shouldItemRender={(item, value) => item.code.toLowerCase().replace(/ /g, '').startsWith(value.trim().toLowerCase().replace(/ /g, ''))}
                getItemValue={item => item.code}
                wrapperProps={{ className: 'shift-autocomplete-wrapper' }}
                wrapperStyle={{ display: 'block' }}
                renderInput={(props) => <input type="text" {...props} />}
                renderMenu={(items, value, style) => <div className="items" children={items} />}
                renderItem={(item) => <div className="item" key={item.id}>{item.code}</div>}
                inputProps={{
                    name: name,
                    className: className,
                    placeholder: placeholder,
                    disabled: isDisabled,
                    onFocus: this.handleInputFocus,
                    onPaste: this.handleInputPaste,
                    onBlur: this.handleInputBlur
                }}
                value={value}
                onChange={this.handleChange}
                onSelect={this.handleSelect}
            />
        );
    }
}