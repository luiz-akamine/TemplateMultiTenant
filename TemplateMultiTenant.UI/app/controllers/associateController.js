//Controller para associação de conta local com login externo
'use strict';
app.controller('associateController', ['$scope', '$location','$timeout','authService', function ($scope, $location,$timeout, authService) {

    $scope.savedSuccessfully = false;
    $scope.message = "";

    //JSON para registro externo
    $scope.registerData = {
        userName: authService.externalAuthData.userName,
        provider: authService.externalAuthData.provider,
        externalAccessToken: authService.externalAuthData.externalAccessToken
    };

    //Registro de login externo
    $scope.registerExternal = function () {

        authService.registerExternal($scope.registerData).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.message = "User has been registered successfully, you will be redicted to orders page in 2 seconds.";
            startTimer();

        },
          function (response) {
              var errors = [];
              for (var key in response.modelState) {
                  errors.push(response.modelState[key]);
              }
              $scope.message = "Failed to register user due to:" + errors.join(' ');
          });
    };

    //timer de redirecionamento para ordens
    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/orders');
        }, 2000);
    }

}]);