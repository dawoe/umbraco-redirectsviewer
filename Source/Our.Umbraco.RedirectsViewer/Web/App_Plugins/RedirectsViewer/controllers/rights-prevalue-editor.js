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
            $scope.model.value = {
                allowed: vm.allowed,
                selectedRights : vm.selectedRights
            }
        });

       
        function init() {
            if ($scope.model.value) {
                vm.allowed = $scope.model.value.allowed;
                vm.selectedRights = $scope.model.value.usergroups;
            }

            userGroupResource.getAll().then(
                function(data) {                   
                    vm.groups = data;
                    vm.loading = false;
                }
            );
        }

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.RightsPrevalueEditorController", ['$scope', 'Our.Umbraco.RedirectsViewer.UserGroupResource', RightsPrevalueEditorController]);

})();