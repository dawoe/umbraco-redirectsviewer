(function () {
    "use strict";

    function SettingsController($scope, userGroupResource, notificationsService, angularHelper, editorService, redirectsHub) {
        var vm = this;

        vm.loading = true;
        vm.createGroups = [];
        vm.deleteGroups = [];
        vm.selectedGroups = [];
        vm.settings = {};


        function init() {
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

        function InitHub() {
            redirectsHub.initHub(function (hub) {

                vm.hub = hub;

                vm.hub.on('update', function (data) {
                    vm.statusImportItems =data.Message;
                    
                    vm.importedCount = data.Count;
                    vm.total = data.Total;

                });

                vm.hub.start();
            });
        }

        init();
      
    }
   
    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.SettingsController", ['$scope', 'Our.Umbraco.RedirectsViewer.UserGroupResource', 'notificationsService', 'angularHelper', 'editorService', "redirectsHub", SettingsController]);

})();