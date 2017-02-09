'use strict';
app.controller('productsController', ['$scope', 'productsService', function ($scope, productsService) {

    $scope.products = [];

    //Adquirindo lista de Ordens pelo serviço que chama a API
    productsService.getProducts().then(function (results) {

        $scope.products = results.data;

    }, function (error) {        
        alert(error.data.Message);
    });

}]);