import React, { useState } from 'react';

export const Form = (props) => {
    const { setValues, handleSubmit, validations, data, btn } = props;
    const { code, name, isActive, unitId, units } = data

    const handleGoBack = () => {
        history.back();
    }

    const [selectedOption, setSelectedOption] = useState('');

    const handleSelectChange = (event) => {
        setSelectedOption(event.target.value);
    };

    const handleChange = (e) => {
        setValues({ ...data, [e.target.name]: e.target.type === 'checkbox' ? e.target.checked : e.target.value.trim() })
    }
    return (
        <form className="form-horizontal" onSubmit={handleSubmit}>
            <div className="card">
                {
                    validations && <div className="validation-summary-errors">
                        <ul>
                            <li>{validations.code}</li>
                            <li>{validations.name}</li>
                        </ul>
                    </div>
                }

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Code</label>
                    </div>
                    <div className="input-form">
                        <input type="text" className="form-control" name="code" onChange={handleChange} autoComplete='off' value={code} />
                    </div>
                </div>

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Name</label>
                    </div>
                    <div className="input-form">
                        <input type="text" className="form-control" name="name" onChange={handleChange} autoComplete='off' value={name} />
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Unit</label>
                    </div>
                    <div className="select-form">
                        <select className="form-control" value={selectedOption} onChange={handleSelectChange}>
                            <option value="" />
                            {units.map((unit) => (
                                <option key={unit.id} value={unit.id}>
                                    {unit.name}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Is Active</label>
                    </div>
                    <div className="input-form">
                        <input type="checkbox" name="isActive" checked={isActive} onChange={handleChange} />
                    </div>
                </div>
                <div className="form-group">
                    <button type="submit" className="btn btn-primary" id="btnSubmit">{btn}</button>
                    <a className="btn btn-default" onClick={handleGoBack}>Back</a>
                </div>
            </div>
        </form>
    )
}