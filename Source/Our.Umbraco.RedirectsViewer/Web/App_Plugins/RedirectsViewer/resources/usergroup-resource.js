(function () {
    "use strict";

    function UserGroupResource($http, umbRequestHelper) {

        var apiUrl = Umbraco.Sys.ServerVariables["Our.Umbraco.RedirectsViewer"].UserGroupApi;

        var resource = {
            getAll: getAllUserGroups,
            getSettings: getRedirectSettings,
            saveRedirectSettings: saveRedirectSettings
        };

        return resource;

        function getAllUserGroups() {
            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + "GetUserGroups"), "Failed to get list");
        }

        function getRedirectSettings() {
            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + "GetConfig"), "Failed to get settings");
        }

        function saveRedirectSettings(redirectSettings) {

            return umbRequestHelper.resourcePromise(
                $http.post(apiUrl + "SaveConfig", redirectSettings),
                "Failed so save settings"
            );
        }


    }

    angular.module("umbraco.resources").factory("Our.Umbraco.RedirectsViewer.UserGroupResource", UserGroupResource);

})();