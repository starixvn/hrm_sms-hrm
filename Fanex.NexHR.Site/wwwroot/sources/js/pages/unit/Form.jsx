import React from 'react';

export const Form = (props) => {
    const { setValues , handleSubmit, validations, data, btn } = props;

    const { name, code, isActive } = data
    const handleGoBack = () => {
        history.back();
    }

    const handleChange = (e) => {
        setValues({ ...data, [e.target.name]: e.target.type === 'checkbox' ? e.target.checked : e.target.value.trim() })
    }
    return (
        <form className="form-horizontal" onSubmit={handleSubmit }>
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
                        <input type="text" className="form-control" name="code" onChange={handleChange} autoComplete='off' value={code ? code : ''} />
                    </div>
                </div> 

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Name</label>
                    </div>
                    <div className="input-form">
                        <input type="text" className="form-control" name="name" onChange={handleChange} autoComplete='off' value={ name ? name : '' } />
                    </div>
                </div> 

                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Is Active</label>
                    </div>
                    <div className="input-form">
                        <input type="checkbox" name="isActive" checked={isActive ? isActive : '' } onChange={handleChange} />
                    </div>
                </div> 

                <div className="form-group">
                    <button type="submit" className="btn btn-primary" id="btnSubmit">{ btn }</button>
                    <a className="btn btn-default" onClick={ handleGoBack }>Back</a>
                </div>
            </div>
        </form>
    )
}