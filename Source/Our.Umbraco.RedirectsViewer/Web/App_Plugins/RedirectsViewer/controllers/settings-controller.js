(function () {
    "use strict";

    function SettingsController($scope, userGroupResource, notificationsService) {
        var vm = this;

        vm.loading = true;
        vm.createGroups = [];
        vm.deleteGroups = [];
        vm.selectedGroups = [];
        vm.settings = {};

       
        function init() {
            
            userGroupResource.getAll().then(
                //groups from umbraco clean
                function (data) {                   

                    vm.createGroups = angular.copy(data);

                    vm.deleteGroups = angular.copy(data);

                    //get previously saved settings via webapi controller that uses keyvalue service
                    userGroupResource.getSettings().then(
                        function (data) {
                            vm.settings = data;

                            applySelection(vm.settings[0].usergroups, vm.createGroups);

                            applySelection(vm.settings[1].usergroups, vm.deleteGroups);

                            vm.loading = false;
                        }
                    );

                    vm.loading = false;
                }
            );


        }

        function applySelection(selectedGroups,allGroups) {
            for (var i = 0; i < allGroups.length; i++) {
                var isChecked = _.contains(selectedGroups, allGroups[i].alias);
                allGroups[i].checked = isChecked;
            }
        };       

        function saveSettings() {

            var settingsCreate = buildSetting(vm.settings[0].allowed, vm.createGroups, "createAllowed");

            var settingsDelete = buildSetting(vm.settings[1].allowed, vm.deleteGroups, "deleteAllowed");

            var settings = [settingsCreate, settingsDelete]; //array this passed to controller to save

            function buildSetting(allowed, groups, key) {

                var selectedGroups = _.filter(groups, function (x) { return x.checked === true });

                var selectedAliases = _.map(selectedGroups, function (x) { return x.alias });

                var settingsTmp = {
                    allowed: allowed,
                    usergroups: selectedAliases,
                    key: key
                };

                return settingsTmp;
            }

            userGroupResource.saveRedirectSettings(settings).then(
                function (data) {
                    notificationsService.success("Save", "Settings saved");
                },
                function (err) {
                    console.error(err);
                    notificationsService.error("Save", "Error saving settings");
                }
            );

        };

        vm.saveSettings = saveSettings;

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.SettingsController", ['$scope', 'Our.Umbraco.RedirectsViewer.UserGroupResource','notificationsService', SettingsController]);

})();