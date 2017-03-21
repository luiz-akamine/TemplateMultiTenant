'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService', 'shareDataService', function ($scope, $location, $timeout, authService, shareDataService) {

    //Variáveis para controle de mensagem de retorno do cadastro para usuário
    $scope.savedSuccessfully = false;
    $scope.message = "";
    $scope.errors = [];

    //1 = Pessoa Física | 2 - Pessoa Juridica
    $scope.signUpType = 0;

    //objeto JSON que será enviado para a API que realiza o registro/cadastro dos novos usuários
    $scope.registration = {
        userName: "",
        password: "",
        confirmPassword: "",
        subscriptionType: 0
    };

    //Função que tenta realizar o registro do novo usuário, chamando o serviço authService que realiza o registro chamando a WEB API no server
    $scope.signUp = function (form) {        
        if (form.$valid) {
            $scope.errors = [];
            $scope.message = '';

            //Tratamento específico pessoa física
            if ($scope.signUpType == 1) {
                $scope.registration.client_id = $scope.registration.userName;
                $scope.registration.cnpj = 0;
                $scope.registration.companyName = $scope.registration.userName;
            };

            //Realizando chamanda da WEB API de registro pelo serviço authService
            authService.saveRegistration($scope.registration).then(function (response) {            
                //Cadastro foi OK!
                $scope.savedSuccessfully = true;
                $scope.message = "Usuário cadastrado com sucesso! Redirecionando para página inicial em 2 segundos...";
                //Armazenando tipo de usuário para já carregar na página de login
                shareDataService.setValue($scope.signUpType);
                //Redirecionando para página com delay de 2 segundos
                startTimer();
            },
             //Ocorreu erro no cadastro
             function (response) {
                 debugger;
                 for (var key in response.data.modelState) {
                     for (var i = 0; i < response.data.modelState[key].length; i++) {
                         $scope.errors.push(response.data.modelState[key][i]);
                     }
                 }                 
                 $scope.message = "Falha no registro do usuário: " + response.data.message;
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