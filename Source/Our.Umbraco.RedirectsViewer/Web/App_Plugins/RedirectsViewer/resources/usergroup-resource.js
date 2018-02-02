(function () {
    "use strict";

    function UserGroupResource($http, umbRequestHelper) {

        var resource = {
            getAll: getAllUserGroups
        };

        return resource

        function getAllUserGroups() {
            return umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("Umbraco.Sys.ServerVariables.Our.Umbraco.RedirectsViewer", "GetUserGroups")), "Failed to get list");
        }

    }

    angular.module("umbraco.resources").factory("Our.Umbraco.RedirectsViewer.UserGroupResource", UserGroupResource);

})();