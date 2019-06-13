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
        [`${styleDistPath}/employee.request`]: `${styleSourcePath}/pages/request/employee.request.scss`,
        [`${styleDistPath}/employee.postnatal`]: `${styleSourcePath}/pages/postnatal/postnatal.scss`,
        [`${styleDistPath}/report.attendance`]: `${styleSourcePath}/pages/report/report.attendance.scss`,
        [`${styleDistPath}/function.index`]: `${styleSourcePath}/pages/function/index.scss`,
        [`${styleDistPath}/account.editprofile`]: `${styleSourcePath}/pages/account/profile-edit.scss`,
        [`${styleDistPath}/unpaidleave.index`]: `${styleSourcePath}/pages/unpaidleave/unpaid-leave.scss`,
        [`${styleDistPath}/section.supervisor.index`]: `${styleSourcePath}/pages/section/supervisor.index.scss`,
        [`${styleDistPath}/section.edit.supervisor`]: `${styleSourcePath}/pages/section/edit.supervisor.scss`,
        [`${styleDistPath}/section.edit.discipline.manager`]: `${styleSourcePath}/pages/section/edit.discipline.manager.scss`,

        //
        [`${jsDistPath}/main`]: './sources/js/main.js',
        [`${jsDistPath}/employee.request`]: `${jsSourcePath}/request/employee.request.js`,
        [`${jsDistPath}/schedule.arrange`]: `${jsSourcePath}/schedule-arrange/index.jsx`,
        [`${jsDistPath}/schedule.arrange.iframe`]: `${jsSourcePath}/schedule-arrange/iframe/index.jsx`,
        [`${jsDistPath}/function.index`]: `${jsSourcePath}/function/index.js`,
        [`${jsDistPath}/section.edit.supervisor`]: `${jsSourcePath}/section/supervisor/index.jsx`,
        [`${jsDistPath}/section.edit.discipline.manager`]: `${jsSourcePath}/section/discipline-manager/index.jsx`,
        [`${jsDistPath}/section.edit.shift.manager`]: `${jsSourcePath}/section/shift-manager/index.jsx`,
        [`${jsDistPath}/employee.edit.supervisor`]: `${jsSourcePath}/employee/supervisor/index.jsx`,
        [`${jsDistPath}/employee.edit.discipline.manager`]: `${jsSourcePath}/employee/discipline-manager/index.jsx`,
        [`${jsDistPath}/employee.edit.shift.manager`]: `${jsSourcePath}/employee/shift-manager/index.jsx`,
        [`${jsDistPath}/employee.postnatal`]: `${jsSourcePath}/postnatal/index.js`,
        [`${jsDistPath}/employee.paternity`]: `${jsSourcePath}/paternity/index.js`,
        [`${jsDistPath}/account.edit`]: `${jsSourcePath}/account/profile-edit.js`
    }
};