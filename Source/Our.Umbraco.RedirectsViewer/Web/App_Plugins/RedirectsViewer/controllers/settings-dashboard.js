(function () {
    "use strict";

    function SettingsDashboardController($scope, userGroupResource) {
        var vm = this;

        vm.loading = true;
        vm.allowed = false;
        vm.groups = [];
        vm.selectedGroups = [];
        vm.settings = {};

        vm.saveSettings = saveSettings;

        //sync things up on save
        function saveSettings(key) {

            var selectedGroups = _.filter(vm.groups, function (x) { return x.checked === true });
           
            var selectedAliases = _.map(selectedGroups, function (x) { return x.alias });

            vm.selectedGroups = selectedAliases;

            var settings = {
                allowed: vm.allowed,
                usergroups: vm.selectedGroups
            };

            settings.key = key;

            userGroupResource.saveRedirectSettings(settings).then(
                function (data) {

                }
            );

        };

       
        function init() {
           
            userGroupResource.getAll().then(
                function(data) {                   
                    vm.groups = data;
                    vm.loading = false;
                }
            );

            userGroupResource.getSettings("redirects_allowcreate").then(
                function (data) {
                    vm.settings = data;
                    vm.loading = false;
                    vm.allowed = data.allowed;

                    if (data.usergroups.length>0) {
                            vm.selectedGroups = data.usergroups;
                            applySelection();
                    }
                }
            );
        }

        function applySelection() {
            for (var i = 0; i < vm.groups.length; i++) {
                var isChecked = _.contains(vm.selectedGroups, vm.groups[i].alias);
                vm.groups[i].checked = isChecked;
            }
        };       

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.SettingsDashboardController", ['$scope', 'Our.Umbraco.RedirectsViewer.UserGroupResource', SettingsDashboardController]);

})();