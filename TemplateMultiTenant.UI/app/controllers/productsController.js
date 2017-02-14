'use strict';
app.controller('productsController', ['$scope', 'productsService', 'authService', function ($scope, productsService, authService) {

    $scope.products = [];
    $scope.authService = authService;

    //Adquirindo lista de Ordens pelo serviço que chama a API
    productsService.getProducts().then(function (results) {

        $scope.products = results.data;

    }, function (error) {        
        alert(error.data.Message);
    });

}]);