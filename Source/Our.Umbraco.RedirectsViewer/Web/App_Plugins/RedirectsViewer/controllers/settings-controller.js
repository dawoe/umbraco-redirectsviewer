(function () {
    "use strict";

    function SettingsController(userGroupResource) {
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
                function(data) {                   
                    vm.createGroups = data; //groups from umbraco clean
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

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.SettingsController", ['$scope','Our.Umbraco.RedirectsViewer.UserGroupResource', SettingsController]);

})();