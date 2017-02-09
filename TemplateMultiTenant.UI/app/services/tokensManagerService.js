'use strict';
app.factory('tokensManagerService', ['$http','ngAuthSettings', function ($http, ngAuthSettings) {

    //Server onde está hospedado as WEB APIs
    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    
    //Variável para acessarmos este serviço
    var tokenManagerServiceFactory = {};

    //Função que retorna lista de refresh tokens
    var _getRefreshTokens = function () {
        //Chamando API
        return $http.get(serviceBase + 'api/refreshtokens').then(function (results) {
            return results;
        });
    };

    //Método que apaga Refresh Token
    var _deleteRefreshTokens = function (tokenid) {
        //Chamando API que apaga
        return $http.delete(serviceBase + 'api/refreshtokens/?tokenid=' + tokenid).then(function (results) {
            return results;
        });
    };

    //Definindo métodos desta factory a serem chamadas por outros js
    tokenManagerServiceFactory.deleteRefreshTokens = _deleteRefreshTokens;
    tokenManagerServiceFactory.getRefreshTokens = _getRefreshTokens;

    return tokenManagerServiceFactory;
}]);