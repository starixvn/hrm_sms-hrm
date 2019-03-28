const jsSourcePath = './sources/js/pages';
const jsDistPath = 'js';
const styleSourcePath = './sources/styles';
const styleDistPath = 'styles';

module.exports = {
    client: {
        [`${styleDistPath}/main`]: `${styleSourcePath}/main.scss`,
        [`${styleDistPath}/main_new`]: `${styleSourcePath}/main_new.scss`,
        [`${styleDistPath}/schedulearrange`]: `${styleSourcePath}/pages/calendar/schedulearrange.scss`,
        [`${styleDistPath}/shiftstatistic`]: `${styleSourcePath}/pages/calendar/shiftstatistic.scss`,
        [`${styleDistPath}/workingcalendars`]: `${styleSourcePath}/pages/calendar/workingcalendars.scss`,
        [`${styleDistPath}/employee.details`]: `${styleSourcePath}/pages/employee/employee.details.scss`,
        [`${styleDistPath}/employee.edit`]: `${styleSourcePath}/pages/employee/employee.edit.scss`,
        [`${styleDistPath}/employee.shift.manager`]: `${styleSourcePath}/pages/employee/employee.shift.manager.scss`,
        [`${styleDistPath}/employee.request`]: `${styleSourcePath}/pages/request/employee.request.scss`,
        [`${styleDistPath}/report.attendance`]: `${styleSourcePath}/pages/report/report.attendance.scss`,

        //
        //[`${jsDistPath}/jquery`]: './node_modules/jquery/dist/jquery.js',
        [`${jsDistPath}/main`]: './sources/js/main.js',
        [`${jsDistPath}/employee-request`]: `${jsSourcePath}/request/employee.request.js`
    }
};