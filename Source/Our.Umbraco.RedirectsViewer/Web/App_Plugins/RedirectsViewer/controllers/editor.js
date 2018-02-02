(function() {
    "use strict";

    function EditController($scope, $routeParams, editorState, redirectUrlsResource, redirectsResource) {
        var vm = this;

        vm.isCreate = $routeParams.create;
        vm.isLoading = true;
        vm.isEnabled = false;
        vm.isAdmin = false;
        vm.redirects = [];

        function checkEnabled() {
            return redirectUrlsResource.getEnableState().then(function(data) {
                vm.isEnabled = data.enabled === true;
                vm.isAdmin = data.userIsAdmin;

                if (vm.isEnabled === false) {
                    vm.isLoading = false;
                }               
            });
        };

        vm.checkEnabled = checkEnabled;

        function loadRedirects(contentKey) {
            vm.loading = true;


            return redirectsResource.getRedirects(contentKey).then(function (data) {
                    vm.redirects = data;
                    vm.isLoading = false;
                },
                function (err) {
                    vm.isLoading = false;
                });
        }

        vm.loadRedirects = loadRedirects;

        function init() {
            vm.checkEnabled().then(function() {
               if (vm.isEnabled) {
                   // only load the redirects when enabled                  
                   vm.loadRedirects(editorState.current.key);
               }
            });
        };

        

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.EditController",
        ['$scope', '$routeParams', 'editorState', 'redirectUrlsResource', 'Our.Umbraco.RedirectsViewer.RedirectsResource', EditController]);
})();