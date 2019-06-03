const jsSourcePath = './sources/js/pages';
const jsDistPath = 'js';
const styleSourcePath = './sources/styles';
const styleDistPath = 'styles';

module.exports = {
    client: {
        [`${styleDistPath}/main`]: `${styleSourcePath}/main.scss`,
        [`${styleDistPath}/main_new`]: `${styleSourcePath}/main_new.scss`,
        [`${styleDistPath}/error`]: `${styleSourcePath}/pages/error.scss`,
        [`${styleDistPath}/schedule.arrange`]: `${styleSourcePath}/pages/calendar/schedulearrange.scss`,
        [`${styleDistPath}/shiftstatistic`]: `${styleSourcePath}/pages/calendar/shiftstatistic.scss`,
        [`${styleDistPath}/workingcalendars`]: `${styleSourcePath}/pages/calendar/workingcalendars.scss`,
        [`${styleDistPath}/employee.details`]: `${styleSourcePath}/pages/employee/employee.details.scss`,
        [`${styleDistPath}/employee.edit`]: `${styleSourcePath}/pages/employee/employee.edit.scss`,
        [`${styleDistPath}/employee.shift.manager`]: `${styleSourcePath}/pages/employee/employee.shift.manager.scss`,
        [`${styleDistPath}/employee.request`]: `${styleSourcePath}/pages/request/employee.request.scss`,
        [`${styleDistPath}/employee.postnatal`]: `${styleSourcePath}/pages/postnatal/postnatal.scss`,
        [`${styleDistPath}/report.attendance`]: `${styleSourcePath}/pages/report/report.attendance.scss`,
        [`${styleDistPath}/function.index`]: `${styleSourcePath}/pages/function/index.scss`,
        [`${styleDistPath}/account.editprofile`]: `${styleSourcePath}/pages/account/profile-edit.scss`,
        [`${styleDistPath}/unpaidleave.index`]: `${styleSourcePath}/pages/unpaidleave/unpaid-leave.scss`,

        //
        [`${jsDistPath}/main`]: './sources/js/main.js',
        [`${jsDistPath}/employee.request`]: `${jsSourcePath}/request/employee.request.js`,
        [`${jsDistPath}/schedule.arrange`]: `${jsSourcePath}/schedule-arrange/index.jsx`,
        [`${jsDistPath}/function.index`]: `${jsSourcePath}/function/index.js`,
        [`${jsDistPath}/employee.postnatal`]: `${jsSourcePath}/postnatal/index.js`,
        [`${jsDistPath}/employee.paternity`]: `${jsSourcePath}/paternity/index.js`,
        [`${jsDistPath}/account.edit`]: `${jsSourcePath}/account/profile-edit.js`
    }
};