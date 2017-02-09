'use strict';
app.controller('refreshController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    //$scope da view tendo acesso a informações do usuário logado
    $scope.authentication = authService.authentication;
    $scope.tokenRefreshed = false;
    $scope.tokenResponse = null;

    //método para revalidar refresh token
    $scope.refreshToken = function () {
        //Chamando API de refresh token pelo serviço
        authService.refreshToken().then(function (response) {
            $scope.tokenRefreshed = true;
            $scope.tokenResponse = response;
        },
         function (err) {
             $location.path('/login');
         });
    };

}]);