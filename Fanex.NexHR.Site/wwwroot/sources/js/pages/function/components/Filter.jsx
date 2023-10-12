import React from 'react';
import { Field } from 'formik';

export const Filter = (props) => {
    const { setChoose, submit } = props;

    return (
        <form className="filter" onSubmit={submit}>
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Role</label>
                    </div>
                    <div className="select-form">
                        <select name="unit" as="select" className="form-control">
                            {/*{units.map((unit) => (*/}
                            {/*    <option key={unit.id} value={unit.id}>*/}
                            {/*        {unit.name}*/}
                            {/*    </option>*/}
                            {/*))}*/}
                        </select>
                    </div>
                </div>
            </div>
        </form>
    )
};