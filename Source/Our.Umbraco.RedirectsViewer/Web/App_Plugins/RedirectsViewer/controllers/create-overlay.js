(function () {
    "use strict";

    function CreateOverlayController($scope, localizationService) {
        var vm = this;       

        vm.model = $scope.model.data;

        vm.properties = {
            'OldUrl': { 'label': 'Old url', 'description': 'Enter the old url without the domain', 'propertyErrorMessage': 'This is a required field' }
        };

        function init() {
            vm.model = $scope.model.data;
            localizationService.localizeMany([
                'redirectsviewer_oldUrlLabel', 'redirectsviewer_oldUrlDescription', 'redirectsviewer_oldUrlError'
            ]).then(function(data) {
                vm.properties = {
                    'OldUrl': { 'label': data[0], 'description': data[1], 'propertyErrorMessage': data[2]}
                };
            });
        };

        init();
    };

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.CreateOverlayController", ['$scope', 'localizationService', CreateOverlayController]);

})();