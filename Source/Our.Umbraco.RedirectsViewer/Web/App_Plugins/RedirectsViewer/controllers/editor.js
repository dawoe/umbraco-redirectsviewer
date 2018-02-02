(function() {
    "use strict";

    function EditController($scope, $routeParams, redirectUrlsResource) {
        var vm = this;

        vm.isCreate = $routeParams.create;
        vm.isLoading = true;
        vm.isEnabled = false;
        vm.isAdmin = false;

        function checkEnabled() {
            return redirectUrlsResource.getEnableState().then(function(data) {
                vm.isEnabled = data.enabled === true;
                vm.isAdmin = data.userIsAdmin;

                if (vm.isEnabled === false) {
                    vm.isLoading = false;
                }

                console.log(vm.isAdmin);
            });
        };

        vm.checkEnabled = checkEnabled;

        function init() {
            vm.checkEnabled().then(function() {
                console.log("Enabled check completed");
            });
        };

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.EditController",
        ['$scope', '$routeParams', 'redirectUrlsResource', EditController]);
})();