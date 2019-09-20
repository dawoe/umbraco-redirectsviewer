﻿(function () {
    "use strict";


    function SettingsController($rootScope, $scope, userGroupResource, notificationsService, angularHelper, localizationService, editorService, redirectsHub) {
        var vm = this;

        vm.loading = true;
        vm.createGroups = [];
        vm.deleteGroups = [];
        vm.selectedGroups = [];
        vm.settings = {};
        $rootScope.importFinished = false;
        $rootScope.statusImportItems = undefined;


        vm.properties = {
            'AllowPermission': { 'label': 'Set specific permissions', 'description': 'Toggle this to set specific permissions per user group. If not all users will have all rights' },
            'GroupPermissions': { 'label': 'Group permissions', 'description': 'Check the groups you want to allow this permissions for. Admins will always have this permissions' },
        };

       
        function init() {

            localizationService
                .localizeMany([
                    'redirectsviewer_createAllowLabel', 'redirectsviewer_createAllowDescription',
                    'redirectsviewer_groupPermissionsLabel', 'redirectsviewer_prevalueUsergroups'
                ]).then(function(data) {
                    vm.properties.AllowPermission.label = data[0];
                    vm.properties.AllowPermission.description = data[1];
                    vm.properties.GroupPermissions.label = data[2];
                    vm.properties.GroupPermissions.description = data[3];
                });
            InitHub();
            userGroupResource.getAll().then(
                //groups from umbraco clean
                function (data) {

                    vm.createGroups = angular.copy(data);

                    vm.deleteGroups = angular.copy(data);

                    //get previously saved settings via webapi controller that uses keyvalue service
                    userGroupResource.getSettings().then(
                        function (data) {
                            vm.settings = data;

                            applySelection(vm.settings.create.usergroups, vm.createGroups);

                            applySelection(vm.settings.delete.usergroups, vm.deleteGroups);

                            vm.loading = false;
                        }
                    );

                    vm.loading = false;
                }
            );


        }

        function applySelection(selectedGroups, allGroups) {
            for (var i = 0; i < allGroups.length; i++) {
                var isChecked = _.contains(selectedGroups, allGroups[i].alias);
                allGroups[i].checked = isChecked;
            }
        };

        function saveSettings() {

            var settingsCreate = buildSetting(vm.settings.create.allowed, vm.createGroups, "createAllowed");

            var settingsDelete = buildSetting(vm.settings.delete.allowed, vm.deleteGroups, "deleteAllowed");

            var settings = {
                create: settingsCreate,
                delete: settingsDelete
            };

            function buildSetting(allowed, groups, key) {

                var selectedGroups = _.filter(groups, function (x) {
                    return x.checked === true
                });

                var selectedAliases = _.map(selectedGroups, function (x) {
                    return x.alias
                });

                var settingsTmp = {
                    allowed: allowed,
                    usergroups: selectedAliases,
                    key: key
                };

                return settingsTmp;
            }

            userGroupResource.saveRedirectSettings(settings).then(
                function (data) {
                    var currForm = angularHelper.getCurrentForm($scope);
                    currForm.$setPristine();
                    notificationsService.success("Save", "Settings saved");
                },
                function (err) {
                    console.error(err);
                    notificationsService.error("Save", "Error saving settings");
                }
            );

        };

        vm.import = function (options) {
            console.log("test");
            if (!options) options = {};
            if (typeof (options) == 'function') options = {callback: options};

            var d = editorService.open({
                title: "Import",
                view: '/App_Plugins/RedirectsViewer/Views/Dialogs/Import.html',
                submit: function (model) {
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                },
                callback: function (value) {
                    if (options.callback) options.callback(value);
                }
            });

        };

        vm.saveSettings = saveSettings;
        
        function tableToArray(table) {
            var result = []
            var rows = table.rows;
            var cells, t;

            // Iterate over rows
            for (var i = 1, iLen = rows.length; i < iLen; i++) {
                if ((' ' + rows[i].className + ' ').indexOf(' success ') === -1) {
                    cells = rows[i].cells;
                    t = [];

                    // Iterate over cells
                    for (var j = 1, jLen = cells.length; j < jLen; j++) {
                        t.push(cells[j].textContent.trim().replace(",", " "));
                    }
                    result.push(t);
                }

            }
            return result;
        }

        vm.calcPercentage = function (status) {
            if (status !== undefined) {
                return (100 * status) / $rootScope.total;
            }
            return 1;
        }

        vm.export = function () {
            var csv = 'Source,Target,Issue\n';
            var data = tableToArray(document.getElementById("redirectStatusTable"));

            data.forEach(function (row) {
                csv += row.join(',');
                csv += "\n";
            });

            console.log(csv);
            var hiddenElement = document.createElement('a');
            hiddenElement.href = 'data:text/csv;charset=utf-8,' + encodeURI(csv);
            hiddenElement.target = '_blank';
            hiddenElement.download = 'redirects.csv';
            hiddenElement.click();
        }

        function InitHub() {
            redirectsHub.initHub(function (hub) {

                vm.hub = hub;

                vm.hub.on('update', function (data) {
                    console.log(data);
                    $rootScope.statusImportItems = data.Message;

                    $rootScope.importedCount = data.Count;
                    $rootScope.total = data.Total;

                });

                vm.hub.start();
            });
        }


        function toggleValue(value) {
            return !value;
        }

        function toggleCreateAllowed() {
            vm.settings.create.allowed = toggleValue(vm.settings.create.allowed);
        }

        vm.toggleCreateAllowed = toggleCreateAllowed;

        function toggleDeleteAllowed() {
            vm.settings.delete.allowed = toggleValue(vm.settings.delete.allowed);
        }

        vm.toggleDeleteAllowed = toggleDeleteAllowed;

        function toggleCreatePermission(index) {
            vm.createGroups[index].checked = toggleValue(vm.createGroups[index].checked);
        }

        vm.toggleCreatePermission = toggleCreatePermission;

        function toggleDeletePermission(index) {
            vm.deleteGroups[index].checked = toggleValue(vm.deleteGroups[index].checked);
        }

        vm.toggleDeletePermission = toggleDeletePermission;

        init();

    }


    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.SettingsController", ['$rootScope','$scope', 'Our.Umbraco.RedirectsViewer.UserGroupResource', 'notificationsService','angularHelper','localizationService','editorService', "redirectsHub", SettingsController]);

})();