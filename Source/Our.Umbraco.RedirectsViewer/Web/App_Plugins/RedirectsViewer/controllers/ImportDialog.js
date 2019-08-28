angular.module('umbraco').controller('Our.Umbraco.RedirectsViewer.ImportDialog.Controller', function ($rootScope, $scope, $http, notificationsService, localizationService, editorService, Upload, $timeout) {

    // Get the Umbraco version
    var v = Umbraco.Sys.ServerVariables.application.version.split('.');
    $scope.gte81 = v[0] == 8 && v[1] >= 1;
    var apiUrl = Umbraco.Sys.ServerVariables["Our.Umbraco.RedirectsViewer"].RedirectsApi;

    $scope.invalidFileFormat = false;
    $scope.rebuildInput = 1;
    $scope.file = null;

    $scope.back = function () {
        $scope.invalidFileFormat = false;
        $scope.file = null;
        $scope.success = false;
        $scope.error = false;
        $scope.processing = false;
        $scope.processed = false;
        $scope.rebuildInput += 1;
    }



    

    $scope.upload = function (options) {
        if (!options) options = {};
        if (typeof (options) == 'function') options = { callback: options };

        if ($scope.file === null) {
            $scope.noFile = true;
            $timeout(function () {
                $scope.noFile = false;
            }, 1000);
            return;
        }
        $scope.fileName = $scope.file.name;
        $scope.processing = true;

        var client = getClientId();
        editorService.closeAll();
        return Upload.upload({
            file: $scope.file,
            fields: {
                'clientId': client
            },
            url:    apiUrl + "/import",
        
        }).then(function (response) {
            if (response) 
                //var fileName = response.data;

                $scope.error = false;
                $scope.processed = true;
                $scope.processing = false;

                //var file = new Blob([data], { type: 'text/csv' });
                //saveAs(file, 'redirects.csv');
            $rootScope.importFinished = true;
            $rootScope.statusImportItems =JSON.parse(response.data);             
            //return fileName;
        });
    }

    $scope.$on("filesSelected", function (event, args) {
        if (args.files.length <= 0 || $scope.processing) {
            $scope.file = null;
            return;
        }
        $scope.noFile = false;
        var file = args.files[0];
        var extension = file.name.substring(file.name.lastIndexOf(".") + 1, file.name.length).toLowerCase();
        console.log(extension);
        if (extension !== "csv" && extension !== "xlsx") {
            $scope.invalidFileFormat = true;

            $timeout(function () {
                $scope.rebuildInput += 1;
                $scope.file = null;
                $scope.invalidFileFormat = false;
            }, 1000);
            return;
        }
        $scope.file = file;
    });

    function initLabels() {
        $scope.labels = {
            errorNoUrl: { title: 'No URL', message: 'You must specify the original URL.' },
            errorInvalidUrl: { title: 'Invalid URL', message: 'The specified URL is not valid.' },
            errorNoLink: { title: 'No link', message: 'You must select a destination page or link.' },
            errorAddFailed: { title: 'Saving failed', message: 'The redirect could not be saved due to an error on the server.' },
            saveSuccessful: { title: 'Redirect added', message: 'Your redirect has successfully been added.' }
        };

        localizationService.localize('redirectsviewer_errorNoUrlTitle').then(function (value) { $scope.labels.errorNoUrl.title = value; });
        localizationService.localize('redirectsviewer_errorNoUrlMessage').then(function (value) { $scope.labels.errorNoUrl.message = value; });

        localizationService.localize('redirectsviewer_errorInvalidUrlTitle').then(function (value) { $scope.labels.errorInvalidUrl.title = value; });
        localizationService.localize('redirectsviewer_errorInvalidUrlMessage').then(function (value) { $scope.labels.errorInvalidUrl.message = value; });

        localizationService.localize('redirectsviewer_errorNoLinkTitle').then(function (value) { $scope.labels.errorNoLink.title = value; });
        localizationService.localize('redirectsviewer_errorNoLinkMessage').then(function (value) { $scope.labels.errorNoLink.message = value; });

        localizationService.localize('redirectsviewer_errorAddFailedTitle').then(function (value) { $scope.labels.errorAddFailed.title = value; });
        localizationService.localize('redirectsviewer_errorAddFailedMessage').then(function (value) { $scope.labels.errorAddFailed.message = value; });

        localizationService.localize('redirectsviewer_addSuccessfulTitle').then(function (value) { $scope.labels.saveSuccessful.title = value; });
        localizationService.localize('redirectsviewer_addSuccessfulMessage').then(function (value) { $scope.labels.saveSuccessful.message = value; });

    }
    function getClientId() {
        if ($.connection !== undefined && $.connection.hub !== undefined) {
            return $.connection.hub.id;
        }
        return "";
    }
    initLabels();

});