//Serviço "Interceptor" -> Realizará as interceptações em cada requisição http (antes ou depois das requisições)
//A ativação desse serviço ""interceptor" fica no app.js

'use strict';
app.factory('authInterceptorService', ['$q', '$injector', '$location', 'localStorageService', function ($q, $injector, $location, localStorageService) {

    //Variável para acessarmos este serviço
    var authInterceptorServiceFactory = {};

    //Método responsável por interceptar ANTES as requisições http e inserir o token no header caso haja usuário logado
    var _request = function (config) {        
        //criando headers caso não haja
        config.headers = config.headers || {};

        //Adquirindo usuário/token logados
        var authData = localStorageService.get('authorizationData');
        //Verificando se existe de fato usuário logado
        if (authData) {
            //Caso exista, inserir token no header da requisição
            config.headers.Authorization = 'Bearer ' + authData.token;
        }

        return config;
    }

    //Método executado DEPOIS de requisições http que ocorrem erro
    var _responseError = function (rejection) {        
        //Checando se deu erro "não autorizado"
        if (rejection.status === 401) {
            //Adquirindo authService
            var authService = $injector.get('authService');
            //Adquirindo se há dados no localStorage referente a autenticação do usuário
            var authData = localStorageService.get('authorizationData');

            if (authData) {
                //Caso haja dados da autenticação no localStorage, verificar se usuário está configurado para utilizar refresh token
                if (authData.useRefreshTokens) {
                    //Caso esteja, redirecionando para view de revalidação do refresh token
                    $location.path('/refresh');
                    return $q.reject(rejection);
                }
            }

            //deslogando
            authService.logOut();
            //Redirecionando para página de login
            $location.path('/login');
        }

        //rejeita erro
        return $q.reject(rejection);
    }

    //Definindo métodos desta factory a serem chamadas por outros js
    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);