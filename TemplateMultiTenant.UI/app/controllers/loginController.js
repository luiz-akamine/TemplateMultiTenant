'use strict';
app.controller('loginController', ['$scope', '$location', 'authService', 'ngAuthSettings', 'shareDataService', function ($scope, $location, authService, ngAuthSettings, shareDataService) {

    //1 = Pessoa Física | 2 - Pessoa Juridica
    $scope.loginType = 0;
    if (shareDataService.getValue() > 0) {
        $scope.loginType = shareDataService.getValue();
        shareDataService.setValue(0);
    }

    //objeto JSON que será enviado para a WEB API no server que realiza login
    $scope.loginData = {
        userName: "",
        password: "",
        client_id: "",
        useRefreshTokens: true
    };

    $scope.message = "";

    //Função que tenta realizar login
    $scope.login = function (form) {

        if (form.$valid) {

            //Tratamento específico pessoa física
            if ($scope.loginType == 1) {
                $scope.loginData.client_id = $scope.loginData.userName;                
            };

            //Chamando serviço authSerivce que chama WEB API no server que realiza login
            authService.login($scope.loginData).then(function (response) {

                //Redirecionando para página "principal" que necessita do usuário logado
                $location.path('/products');

            },
             function (err) {
                 debugger;
                 $scope.message = err.data.error_description;
            });
        }
    };

    $scope.resetPassword = function () {
        $location.path('/resetpassword');
    };


    /******************************************************************************************
                                    IMPLEMENTAÇÃO LOGIN EXTERNO
    ******************************************************************************************/
    
    //Autenticação externa (Facebook)
    $scope.authExternalProvider = function (provider) {

        //Montando URI de redirecionamento
        var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';

        //Chamando Login externo (Facebook)
        var externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/Account/ExternalLogin?provider=" + provider
                                                                    + "&response_type=token&client_id=" + ngAuthSettings.clientId
                                                                    + "&redirect_uri=" + redirectUri;
        window.$windowScope = $scope;

        //Abrindo janela para login externo
        var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=500");
    };

    //função abaixo será chamado pela authComplete.html que decodificará a informação da URL para realizar fim da autenticação externa
    $scope.authCompletedCB = function (fragment) {

        $scope.$apply(function () {
            debugger;
            if (fragment.haslocalaccount == 'False') {

                authService.logOut();
                
                authService.externalAuthData = {
                    provider: fragment.provider,
                    userName: fragment.external_user_name,
                    email: fragment.email,
                    externalAccessToken: fragment.external_access_token
                };

                //Redirecionando para view que vincula a conta já existente no sistema
                $location.path('/associate');
            }
            else {
                //Já tem conta local. Obtém token de acesso e redireciona para view de ordens
                var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
                authService.obtainAccessToken(externalData).then(function (response) {

                    $location.path('/products');

                },
             function (err) {
                 $scope.message = err.error_description;
             });
            }

        });
    }
}]);