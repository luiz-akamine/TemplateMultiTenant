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
            $scope.tokenResponse = response.data;
            
            //exemplo redirecionamento
            $location.path('/products');
        },
         function (err) {
             $location.path('/login');
         });
    };

    $scope.refreshToken();

}]);