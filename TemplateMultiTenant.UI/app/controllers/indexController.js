'use strict';
app.controller('indexController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    //botão de logout
    $scope.logOut = function () {
        //realizando logout limpando localStorage
        authService.logOut();
        //redirecionando para home
        $location.path('/home');
    }

    //Passando para o scope as informações que o serviço "authService" fornece de autenticação. Podendo assim,
    //termos controle do que é exibido por exemplo, na barra de menu botão Login, Logout, Bem-vindo, etc.
    $scope.authentication = authService.authentication;
}]);