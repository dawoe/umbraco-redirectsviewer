(function () {
    "use strict";

    function CreateOverlayController($scope) {
        var vm = this;

        vm.model = $scope.model.data;

        vm.properties = {
            'OldUrl': { 'label': 'Old url', 'description': 'Enter the old url with out the domain and the leading /', 'propertyErrorMessage': 'This is a required field' }
        };
    };

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.CreateOverlayController", ['$scope', CreateOverlayController]);

})();