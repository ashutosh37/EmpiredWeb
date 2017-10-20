(function (app) {
    'use strict';

    app.controller('indexCtrl', indexCtrl);

    indexCtrl.$inject = ['$scope','apiService', 'notificationService'];

    function indexCtrl($scope, apiService, notificationService) {
        $scope.pageClass = 'page-home';
        $scope.loadingPatients = true;
        $scope.isReadOnly = true;
        $scope.loadData = loadData;
        $scope.patients = [];
        function loadData() {
            apiService.get('/api/patients', null,
                        patientsLoadCompleted,
                        patientLoadFailed);

        }

        function patientsLoadCompleted(result) {
            $scope.patients = result.data;
            $scope.loadingPatients = false;
        }


        function patientLoadFailed(response) {
            notificationService.displayError(response.data);
        }


        loadData();
    }

})(angular.module('EmpiredWeb'));