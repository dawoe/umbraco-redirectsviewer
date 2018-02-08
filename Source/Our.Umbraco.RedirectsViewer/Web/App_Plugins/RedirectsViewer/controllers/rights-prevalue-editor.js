(function () {
    "use strict";

    function RightsPrevalueEditorController($scope, userGroupResource) {
        var vm = this;

        vm.loading = true;
        vm.allowed = false;
        vm.groups = [];
        vm.selectedGroups = [];


        if ($scope.model.value) {          
            vm.allowed = $scope.model.value.allowed;
        }

        //sync things up on save
        $scope.$on("formSubmitting", function (ev, args) {

            var selectedGroups = _.filter(vm.groups, function (x) { return x.checked === true });
           
            var selectedAliases = _.map(selectedGroups, function (x) { return x.alias });

            vm.selectedGroups = selectedAliases;

            $scope.model.value = {
                allowed: vm.allowed,
                usergroups: vm.selectedGroups
            }
        });

       
        function init() {
            if ($scope.model.value) {
                vm.allowed = $scope.model.value.allowed;
                vm.selectedGroups = $scope.model.value.usergroups;
            }

            userGroupResource.getAll().then(
                function(data) {                   
                    vm.groups = data;
                    applySelection();
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

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.RightsPrevalueEditorController", ['$scope', 'Our.Umbraco.RedirectsViewer.UserGroupResource', RightsPrevalueEditorController]);

})();