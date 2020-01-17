(function () {
    "use strict";

    function RedirectsResource($http, umbRequestHelper) {

        var apiUrl = Umbraco.Sys.ServerVariables["Our.Umbraco.RedirectsViewer"].RedirectsApi;

        var resource = {
            getRedirects: getRedirectsForContent,
            deleteRedirect: deleteRedirect,
            createRedirect: createRedirect
        };

        return resource;

        function getRedirectsForContent(contentKey, culture) {
            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + "GetRedirectsForContent?contentKey=" + contentKey + "&culture=" + culture),
                "Failed to load redirects for content"
            );
        }

        function deleteRedirect(id) {
            return umbRequestHelper.resourcePromise(
                $http.delete(apiUrl + "DeleteRedirect?id=" + id),
                "Failed delete redirect"
            );
        }

        function createRedirect(url, id, culture) {
            var data = JSON.stringify({ url: url, contentKey: id, culture: culture });

            return umbRequestHelper.resourcePromise(
                $http.post(apiUrl + "CreateRedirect", data),
                "Failed delete redirect"
            );
        }
    }

    angular.module("umbraco.resources").factory("Our.Umbraco.RedirectsViewer.RedirectsResource", RedirectsResource);

})();