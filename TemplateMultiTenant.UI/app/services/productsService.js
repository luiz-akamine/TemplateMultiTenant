//Serviço com métodos relacionados a nossa entidade fictícia de exemplo "Produtcts"

'use strict';
app.factory('productsService', ['$http', 'ngTemplateMultiTenantSettings', function ($http, ngTemplateMultiTenantSettings) {

    //Server onde está hospedado as WEB APIs
    var serviceBase = ngTemplateMultiTenantSettings.apiServiceBaseUri;

    //Variável para acessarmos este serviço
    var productsServiceFactory = {};

    //Método que retorna lista fictícia de "Orders"
    var _getProducts = function () {

        //Chamando WEB API no server que retorna a lista de "Orders"
        return $http.post(serviceBase + 'api/product/ExecMethod', { "MethodName": "Get" }).then(function (results) {
            return results;
        });
    };

    //Definindo métodos desta factory a serem chamadas por outros js
    productsServiceFactory.getProducts = _getProducts;

    return productsServiceFactory;
}]);