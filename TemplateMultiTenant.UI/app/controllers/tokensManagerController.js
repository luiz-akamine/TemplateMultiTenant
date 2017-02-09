'use strict';
app.controller('tokensManagerController', ['$scope', 'tokensManagerService', function ($scope, tokensManagerService) {

    //Lista de Refresh Tokens exibidos na view
    $scope.refreshTokens = [];

    //Chamando serviço que adquire lista de refresh tokens pela API
    tokensManagerService.getRefreshTokens().then(function (results) {
        //setando lista
        $scope.refreshTokens = results.data;

    }, function (error) {
        alert(error.data.message);
    });

    //Método para apagar refresh tokens, consequentemente deslogando usuário do sistema
    $scope.deleteRefreshTokens = function (index, tokenid) {
        //codificando tokenid para segurança ao passar como parâmetro na API que deleta o refresh token
        tokenid = window.encodeURIComponent(tokenid);
        //Chamando API que deleta o refresh token pelo serviço
        tokensManagerService.deleteRefreshTokens(tokenid).then(function (results) {
            //Apagando da lista
            $scope.refreshTokens.splice(index, 1);

        }, function (error) {
            alert(error.data.message);
        });
    }

}]);