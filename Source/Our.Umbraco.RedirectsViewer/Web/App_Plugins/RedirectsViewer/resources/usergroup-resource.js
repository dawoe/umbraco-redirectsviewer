(function () {
    "use strict";

    function UserGroupResource($http, umbRequestHelper) {

        var apiUrl = Umbraco.Sys.ServerVariables["Our.Umbraco.RedirectsViewer"].UserGroupApi;

        var resource = {
            getAll: getAllUserGroups
        };

        return resource;

        function getAllUserGroups() {
            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + "GetUserGroups"), "Failed to get list");
        }

    }

    angular.module("umbraco.resources").factory("Our.Umbraco.RedirectsViewer.UserGroupResource", UserGroupResource);

})();