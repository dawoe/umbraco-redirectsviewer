(function () {
    "use strict";

    function SettingsController($scope,userGroupResource) {
        var vm = this;

        vm.loading = true;
        vm.allowed = false;
        vm.createGroups = [];
        vm.deleteGroups = [];
        vm.selectedGroups = [];
        vm.settings = {};

       
        function init() {
            //if ($scope.model.value) {
            //    vm.allowed = $scope.model.value.allowed;
            //    vm.selectedGroups = $scope.model.value.usergroups;
            //}

            userGroupResource.getAll().then(
                //groups from umbraco clean
                function(data) {                   
                    vm.createGroups = data;
                    vm.deleteGroups = data;
                    
                    vm.loading = false;
                }
            );

            userGroupResource.getSettings().then(
                function (data) {
                    vm.settings = data;
                    applySelection(vm.settings[0].usergroups,vm.createGroups);
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

        vm.saveSettings = saveSettings;

        //sync things up on save
        function saveSettings() {

            var settingsCreate = buildSetting(vm.allowed, vm.createGroups, "createAllowed");

            //var settingsDelete = buildSetting(vm.deleteAllowed, vm.deleteGroups, "deleteAllowed");

            var settings = [settingsCreate];

            userGroupResource.saveRedirectSettings(settings).then(
                function (data) {
                    //todo notifications handler?
                }
            );

            function buildSetting(allowed, createGroups, key) {

                var selectedCreateGroups = _.filter(createGroups, function (x) { return x.checked === true });

                var selectedCreateAliases = _.map(selectedCreateGroups, function (x) { return x.alias });

                var settingsCreate = {
                    allowed: allowed,
                    usergroups: selectedCreateAliases,
                    key: key
                };

                return settingsCreate;
            }

        };

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.SettingsController", ['$scope','Our.Umbraco.RedirectsViewer.UserGroupResource', SettingsController]);

})();