(function () {
    "use strict";

    function SettingsController(userGroupResource) {
        var vm = this;

        vm.loading = true;
        vm.allowed = false;
        vm.groups = [];
        vm.selectedGroups = [];
        vm.settings = {};

       
        function init() {
            //if ($scope.model.value) {
            //    vm.allowed = $scope.model.value.allowed;
            //    vm.selectedGroups = $scope.model.value.usergroups;
            //}

            userGroupResource.getAll().then(
                function(data) {                   
                    vm.groups = data;
                   // applySelection();
                    vm.loading = false;
                }
            );

            userGroupResource.getSettings($scope.model.alias).then(
                function (data) {
                    vm.settings = data;
                    vm.loading = false;
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

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.SettingsController", ['Our.Umbraco.RedirectsViewer.UserGroupResource', SettingsController]);

})();