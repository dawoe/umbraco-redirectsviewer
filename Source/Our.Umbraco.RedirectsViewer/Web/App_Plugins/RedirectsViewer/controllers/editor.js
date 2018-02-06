(function() {
    "use strict";

    function EditController($scope, $routeParams, editorState, redirectUrlsResource, redirectsResource, authResource, notificationsService) {
        var vm = this;

        vm.isCreate = $routeParams.create;
        vm.isLoading = true;
        vm.isEnabled = false;
        vm.isAdmin = false;
        vm.redirects = [];
        vm.canDelete = false;
        vm.canAdd = false;        

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

        function checkUserPermissions() {
            return authResource.getCurrentUser().then(function (user) {

                //admin can always add and delete
                if (vm.isAdmin) {
                    vm.canDelete = true;
                    vm.canAdd = true;
                } else {
                    var groups = user.userGroups;
                    
                    if ($scope.model.config.allowdelete.allowed) {
                        // we need to check if the user has rights to delete
                       for (var i = 0; i < groups.length; i++) {
                            vm.canDelete = _.contains($scope.model.config.allowdelete.usergroups, groups[i]);

                            if (vm.canDelete) {
                                break;
                            }
                        }
                    }

                    if ($scope.model.config.allowcreate.allowed) {
                        // we need to check if the user has rights to add
                       
                        for (var i = 0; i < groups.length; i++) {
                            vm.canAdd = _.contains($scope.model.config.allowcreate.usergroups, groups[i]);

                            if (vm.canAdd) {
                                break;
                            }
                        }
                    }
                }
            });
        };

        vm.checkUserPermissions = checkUserPermissions;

        function loadRedirects() {
            vm.loading = true;


            return redirectsResource.getRedirects(editorState.current.key).then(function (data) {
                    vm.redirects = data;
                    vm.isLoading = false;
                },
                function (err) {
                    vm.isLoading = false;
                });
        };

        vm.loadRedirects = loadRedirects;

        function showPrompt(item) {
            item.deletePrompt = true;
        };

        vm.showPrompt = showPrompt;

        function confirmAction(index, item) {
            vm.isLoading = true;
            item.deletePrompt = false;
            redirectsResource.deleteRedirect(item.redirectId).then(function (data) {
                    notificationsService.showNotification(data.notifications[0]);
                    loadRedirects();
                },
                function (err) {
                    notificationsService.showNotification(err.data.notifications[0]);
                    vm.isLoading = false;
                });
        };

        vm.confirmAction = confirmAction;

        function hidePrompt(item) {
            item.deletePrompt = false;
        };

        vm.hidePrompt = hidePrompt;        

        function init() {
            vm.checkEnabled().then(function() {
                if (vm.isEnabled) {
                    vm.checkUserPermissions().then(function() {
                        vm.loadRedirects();                        
                    });                                                                      
               }
            });
        };

        

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.RedirectsViewer.EditController",
        ['$scope', '$routeParams', 'editorState', 'redirectUrlsResource', 'Our.Umbraco.RedirectsViewer.RedirectsResource', 'authResource', 'notificationsService', EditController]);
})();