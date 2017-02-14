//Serviço que possui métodos relacionados a autenticação da aplicação

'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    // $http               -> Injection para realizar uma requisições HTTP
    // $q                  -> Injection do serviço de Promises
    // localStorageService -> Injection contendo serviços para manipulação do localStorage do navegador
    // ngAuthSettings      -> Constante que contém configurações globais da aplicação

    //Server onde está hospedado as WEB APIs
    var serviceBase = ngAuthSettings.apiServiceBaseUri;

    //Variável para acessarmos este serviço
    var authServiceFactory = {};

    //Inicialização das informações referentes a autenticação do usuário
    var _authentication = {
        isAuth: false,          //Informa se há usuário autenticado   
        userName: "",           //Guarda nome de usuário
        clientID: "",           //Nome da companhia
        useRefreshTokens: true  //Verifica se usuário está configurado para utilizar refresh token
    };

    //Inicialização das informações referentes a autenticação do usuário externo (Facebook)
    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };

    //Monta objeto da request de registrar client
    var getClientRegistrationData = function (registration) {
        return {
            "Id": registration.client_id,
            "Name": registration.companyName,
            "CNPJ": registration.cnpj,
            "SubscriptionType": 0
        };
    };

    //Monta objeto da request de registrar usuario
    var getUserRegistrationData = function (registration) {
        return {
            "username": registration.userName,
            "password": registration.password,
            "confirmpassword": registration.confirmPassword,
            "ClientId": registration.client_id,
            "Email": registration.email
        };
    };
    
    //Monta objeto da request de reset de senha
    var getResetPasswordSimpleData = function (resetPassword) {
        return {
            "username": resetPassword.username,
            "email": resetPassword.email,
            "clientId": resetPassword.clientId,
            "CNPJ": resetPassword.CNPJ,
            "newPassword": resetPassword.newPassword,
            "confirmNewPassword": resetPassword.confirmNewPassword
        };
    };

    //Método que cadastra novo usuário chamando a WEB API no server
    var _saveRegistration = function (registration) {

        _logOut();
        
        //registration será o JSON contendo as informações necessárias para cadastro do novo usuário
        return $http.post(serviceBase + 'api/account/registerClient', getClientRegistrationData(registration))
            .then(function (response) {
                return $http.post(serviceBase + 'api/account/register', getUserRegistrationData(registration)).then(function (response2) {
                    return response2;
                });
            })
            .catch(function (response) {
                return $http.post(serviceBase + 'api/account/register', getUserRegistrationData(registration)).then(function (response3) {
                    return response3;
                });
            });
    };

    //Método que realiza logon de usuário
    var _login = function (loginData) {

        //Os dados a serem autenticados, devem estar no body da requisição no seguinte formato:        
        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password + "&client_id=" + loginData.client_id;

        //Verifica se usuário utiliza refresh token
        //if (loginData.useRefreshTokens) {
            //Caso utilize, é necessário enviar o client_id conforme arquitetura de uso do refreshtoken
            //data = data + "&client_id=" + ngAuthSettings.clientId;
        //}

        //Criando Promise a ser retornada por esta função
        var deferred = $q.defer();

        //Requisitando token para a WEB API no server enviando os dados do usuário em "data"
        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
            .then(function (response) {                
                //Aramazenando token/usuário no localStorage do browser
                //if (loginData.useRefreshTokens) {
                localStorageService.set('authorizationData', { token: response.data.access_token, userName: loginData.userName, clientId: loginData.client_id, refreshToken: response.data.refresh_token, useRefreshTokens: true });
                //}
                //else {
                //    localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });
                //}

                //Controle se usuário está logado ou não
                _authentication.isAuth = true;
                //Armazenando username logado
                _authentication.userName = loginData.userName;
                //Setando se usuário utiliza refresh token
                _authentication.useRefreshTokens = loginData.useRefreshTokens;
                //
                _authentication.clientID = loginData.client_id;

                deferred.resolve(response);
        },
        function (err, status) {            
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;
    };

    //Método que realiza reset de senha
    var _resetPassword = function (resetpwdData) {        
        
        //registration será o JSON contendo as informações necessárias para cadastro do novo usuário
        return $http.post(serviceBase + 'api/account/resetpasswordsimple', getResetPasswordSimpleData(resetpwdData))
            .then(function (response) {
                return response;
            });
    };

    //Método que realiza logout do usuário
    var _logOut = function () {

        //Limpando storageService que guarda token/usuário logado
        localStorageService.remove('authorizationData');

        //Atualizando variáveis de controle de usuário autenticado
        _authentication.isAuth = false;
        _authentication.userName = "";
        _authentication.useRefreshTokens = false;
        _authentication.clientID = "";
    };

    //Atualiza localStorage
    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
            _authentication.useRefreshTokens = authData.useRefreshTokens;
            _authentication.clientID = authData.clientId;
        }
    }

    //Método que realiza a renovação do refresh token
    var _refreshToken = function () {
        debugger;
        //Criando Promise a ser retornada pela função
        var deferred = $q.defer();

        //Adquirindo dados de autenticação no localStorage
        var authData = localStorageService.get('authorizationData');

        if (authData) {
            //Checando se utiliza Refresh Token
            //if (authData.useRefreshTokens) {
                //Caso utilize, montando url a ser enviado no body da requisição que renova o refresh token
            var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken + "&client_id=" + authData.clientId;
                //limpando localStorage para posterior atualização dos dados da autenticação
                localStorageService.remove('authorizationData');
                //Requisitando novo Refresh Token
                $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).then(
                    function (response) {
                        debugger;
                        //Requisição OK, guardando informações no localStorage
                        localStorageService.set('authorizationData', { token: response.data.access_token, userName: response.data.userName, clientId: authData.clientId, refreshToken: response.data.refresh_token, useRefreshTokens: true });

                        deferred.resolve(response);
                },  function (err, status) {
                    _logOut();
                    deferred.reject(err);
                });
            //}
        }

        return deferred.promise;
    };

    /******************************************************************************************
                                    IMPLEMENTAÇÃO LOGIN EXTERNO
    ******************************************************************************************/

    //Adquire token para acesso 
    var _obtainAccessToken = function (externalData) {

        var deferred = $q.defer();

        $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).then(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    //Registra login externo (Facebook)
    var _registerExternal = function (registerExternalData) {

        var deferred = $q.defer();

        $http.post(serviceBase + 'api/account/registerexternal', registerExternalData).then(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    //Definindo métodos desta factory a serem chamadas por outros js
    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.refreshToken = _refreshToken;
    authServiceFactory.resetPassword = _resetPassword;
    //Implmentação login externo (Facebook)
    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    authServiceFactory.externalAuthData = _externalAuthData;
    authServiceFactory.registerExternal = _registerExternal;

    return authServiceFactory;
}]);