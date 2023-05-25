import React, { Fragment } from 'react';
import LangHelper from '@jsroot/common/langHelper';

const THead = ({ data }) => {
    return (
        <thead>
            <tr>
                <td rowSpan={2} className="col-job-level">{LangHelper.getResource('JobLevel')}</td>
                {
                    data.map(country => <td key={country.name} colSpan={3}>{country.name}</td>)
                }
            </tr>
            <tr>
                {
                    data.map(country => <Fragment key={country.name}>
                        <td className="col-total">PU Total</td>
                        <td className="col-total">BU Total</td>
                        <td className="col-total">SU Total</td>
                    </Fragment>)
                }
            </tr>
        </thead>
    );
};

export default THead;