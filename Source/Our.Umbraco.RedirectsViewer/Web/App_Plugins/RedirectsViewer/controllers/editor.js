(function() {
    "use strict";

    function EditController($scope, $routeParams) {
        var vm = this;

        vm.isCreate = false;
        vm.isLoading = true;


    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.EditController",
        ['$scope', '$routeParams', EditController]);
})();