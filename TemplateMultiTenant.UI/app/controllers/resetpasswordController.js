'use strict';
app.controller('resetpasswordController', ['$scope', '$location', '$timeout', 'authService', function ($scope, $location, $timeout, authService) {

    $scope.resetSuccessfully = false;
    $scope.message = "";
    $scope.errors = [];
    $scope.resetpwdData = {};

    //Função que reseta senha
    $scope.resetPassword = function (form) {

        $scope.errors = [];
        $scope.message = '';

        if (form.$valid) {
            //Chamando serviço authSerivce que chama WEB API no server que realiza login
            authService.resetPassword($scope.resetpwdData).then(function (response) {

                //Redirecionando para página "principal" que necessita do usuário logado
                $scope.resetSuccessfully = true;
                $scope.message = "Senha resetada com sucesso! Redirecionando para página inicial em 2 segundos...";
                //Redirecionando para página com delay de 2 segundos
                startTimer();

            },
             //Ocorreu erro no reset
             function (response) {
                 debugger;
                 for (var key in response.data.modelState) {
                     for (var i = 0; i < response.data.modelState[key].length; i++) {
                         $scope.errors.push(response.data.modelState[key][i]);
                     }
                 }
                 $scope.message = "Falha no reset de senha: " + response.data.message;
             });
        }
    };

    //Método que redireciona para outra página
    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            //Chama outra página pelo $location
            $location.path('/login');
        }, 2000);
    }
}]);