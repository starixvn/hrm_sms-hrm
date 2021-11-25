const path = require('path');

const projectRoot = path.resolve(__dirname, '../../../');

const cssPaths = {
    build: 'wwwroot/css',

    main: ['./www/sources/css/main.scss'],
    error: ['./www/sources/css/pages/error.scss'],
    auth_login: ['./www/sources/css/pages/auth/login.scss'],
    site_index: ['./www/sources/css/pages/site/index.scss'],
    user_index: ['./www/sources/css/pages/user/index.scss'],
    user_assign_site: ['./www/sources/css/pages/user/assign-site.scss'],
    user_permission: ['./www/sources/css/pages/user/permission.scss']
};

const scriptPaths = {
    build: 'wwwroot/js',

    language: ['./www/sources/js/common/languages/ViewResources.js'],
    main: ['./www/sources/js/main.js'],
    auth_login: ['./www/sources/js/pages/auth/login/index.js'],
    site_index: ['./www/sources/js/pages/site/index.js'],
    user_index: ['./www/sources/js/pages/user/index.js'],
    user_assign_site: ['./www/sources/js/pages/user/assign-site/index.js'],
    user_permission: ['./www/sources/js/pages/user/permission/index.js']
};

module.exports = {
    projectRoot: projectRoot,
    cssPaths: cssPaths,
    scriptPaths: scriptPaths
};