(function () {
    "use strict";

    function RightsPrevalueEditorController($scope) {
        var vm = this;

        vm.allowed = false;

        if ($scope.model.value) {
            console.log("we have a value");
            vm.allowed = $scope.model.value.allowed;
        }

        //sync things up on save
        $scope.$on("formSubmitting", function (ev, args) {
            console.log(ev);
            console.log(args);
            $scope.model.value = {
                allowed : vm.allowed
            }
        });

        console.log($scope.model);
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.RightsPrevalueEditorController", RightsPrevalueEditorController);

})();