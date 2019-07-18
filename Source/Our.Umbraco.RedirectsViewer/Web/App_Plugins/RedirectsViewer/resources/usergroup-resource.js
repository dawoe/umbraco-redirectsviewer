(function () {
    "use strict";

    function UserGroupResource($http, umbRequestHelper) {

        var apiUrl = Umbraco.Sys.ServerVariables["Our.Umbraco.RedirectsViewer"].UserGroupApi;

        var resource = {
            getAll: getAllUserGroups,
            getSettings: getRedirectSettings,
            saveSettings: saveRedirectSettings
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

            var data = JSON.stringify({ settings: redirectSettings});

            return umbRequestHelper.resourcePromise(
                $http.post(apiUrl + "SaveConfig", data),
                "Failed so save settings"
            );
        }

    }

    angular.module("umbraco.resources").factory("Our.Umbraco.RedirectsViewer.UserGroupResource", UserGroupResource);

})();