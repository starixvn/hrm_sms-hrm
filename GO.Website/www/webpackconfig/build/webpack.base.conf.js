const alias = require('../config/alias');
const pathConfig = require('../config/path');
const pluginGenerator = require('../config/plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

function generateConfig(environment) {
    const jsPath = `${pathConfig.scriptPaths.build}/${environment}`;
    const cssPath = `${pathConfig.cssPaths.build}/${environment}`;
    return {
        resolve: {
            alias: alias,
            extensions: ['.js', '.jsx']
        },
        entry: {
            /*js*/
            [`${jsPath}/lib/lang-en-US`]: pathConfig.scriptPaths.language,
            [`${jsPath}/main`]: pathConfig.scriptPaths.main,
            [`${jsPath}/pages/login`]: pathConfig.scriptPaths.login,
            [`${jsPath}/pages/home`]: pathConfig.scriptPaths.home,
            [`${jsPath}/pages/unit-head/weekly-schedule`]: pathConfig.scriptPaths.unit_head_weekly_schedule,
            [`${jsPath}/pages/report/manpower/index`]: pathConfig.scriptPaths.report_manpower,
            [`${jsPath}/pages/manpower/employee/index`]: pathConfig.scriptPaths.manpower_employee_index,
            [`${jsPath}/pages/manpower/period/index`]: pathConfig.scriptPaths.manpower_period_index,
            [`${jsPath}/pages/manpower/period/detail`]: pathConfig.scriptPaths.manpower_period_detail,
            [`${jsPath}/pages/manpower/flow/index`]: pathConfig.scriptPaths.manpower_flow_index,
            [`${jsPath}/pages/manpower/annual-request/my-request`]: pathConfig.scriptPaths.annual_manpower_forecast_request_mypage,
            [`${jsPath}/pages/manpower/annual-request/member-request`]: pathConfig.scriptPaths.annual_manpower_forecast_request_memberpage,
            [`${jsPath}/pages/manpower/annual-request/create`]: pathConfig.scriptPaths.annual_manpower_forecast_request_create,
            [`${jsPath}/pages/manpower/annual-request/detail`]: pathConfig.scriptPaths.annual_manpower_forecast_request_detail,
            [`${jsPath}/pages/manpower/annual-request/summary`]: pathConfig.scriptPaths.annual_manpower_forecast_request_summary,
            [`${jsPath}/pages/admin/role-permission/index`]: pathConfig.scriptPaths.admin_role_permission,

            /*css*/
            [`${cssPath}/main`]: pathConfig.cssPaths.main,
            [`${cssPath}/error`]: pathConfig.cssPaths.error,
            [`${cssPath}/pages/login`]: pathConfig.cssPaths.login,
            [`${cssPath}/pages/home`]: pathConfig.cssPaths.home,
            [`${cssPath}/pages/unit-head/weekly-schedule`]: pathConfig.cssPaths.unit_head_weekly_schedule,
            [`${cssPath}/pages/report/manpower/index`]: pathConfig.cssPaths.report_manpower,
            [`${cssPath}/pages/manpower/employee/index`]: pathConfig.cssPaths.manpower_employee_index,
            [`${cssPath}/pages/manpower/period/index`]: pathConfig.cssPaths.manpower_period_index,
            [`${cssPath}/pages/manpower/period/detail`]: pathConfig.cssPaths.manpower_period_detail,
            [`${cssPath}/pages/manpower/flow/index`]: pathConfig.cssPaths.manpower_flow_index,
            [`${cssPath}/pages/manpower/annual-request/my-request`]: pathConfig.cssPaths.annual_manpower_forecast_request_mypage,
            [`${cssPath}/pages/manpower/annual-request/member-request`]: pathConfig.cssPaths.annual_manpower_forecast_request_memberpage,
            [`${cssPath}/pages/manpower/annual-request/create`]: pathConfig.cssPaths.annual_manpower_forecast_request_create,
            [`${cssPath}/pages/manpower/annual-request/summary`]: pathConfig.cssPaths.annual_manpower_forecast_request_summary,
            [`${cssPath}/pages/admin/role-permission/index`]: pathConfig.cssPaths.admin_role_permission
        },
        output: {
            path: pathConfig.projectRoot,
            filename: "[name].chunk.js",
            chunkFilename: `${jsPath}/lib/[name].chunk.js`
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    loader: ["babel-loader"],
                    exclude: /node_modules/
                }
            ]
        },
        plugins: pluginGenerator(environment),
        stats: {
            assets: false,
            colors: true,
            version: false,
            hash: false,
            timings: true,
            chunks: false,
            chunkModules: false,
            children: true
        },
        optimization: {
            minimizer: [
                new UglifyJsPlugin({
                    sourceMap: true,
                    uglifyOptions: {
                        output: {
                            comments: false
                        }
                    }
                }),
                new OptimizeCssAssetsPlugin()
            ],
            splitChunks: {
                chunks: 'all',
                minSize: 30000,
                maxSize: 0,
                minChunks: 2,
                maxAsyncRequests: 5,
                maxInitialRequests: 3,
                name: false,
                cacheGroups: {
                    // disable default cache group
                    default: false,
                    vendors: false,
                    vendor: {
                        name: 'vendors',
                        test: /[\\/]node_modules[\\/](react|react-dom|react-bootstrap)[\\/]/,
                        minChunks: 2,
                        priority: -10
                    }
                }
            }
        }
    };
}

module.exports = generateConfig;