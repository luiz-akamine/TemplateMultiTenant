'use strict';
app.controller('indexController', ['$scope', '$location', '$timeout', 'authService', 'ModalService', function ($scope, $location, $timeout, authService, ModalService) {

    $scope.navbarExpanded = false;

    $scope.toggle = function () {
        $scope.navbarExpanded = !$scope.navbarExpanded;
        debugger;
        if ($scope.navbarExpanded) {
            toggleClose();
        };
    };

    //timer de redirecionamento para ordens
    var toggleClose = function () {
        var timer = $timeout(function () {
            debugger;
            $('#btnToggle').click();
        }, 2000);
    }

    //botão de logout
    $scope.logOut = function () {

        ModalService.showModal({
            templateUrl: "app/views/modalDialog.html",
            controller: "modalController"
        }).then(function (modal) {
            modal.element.modal();
            modal.close.then(function (result) {
                if (result) {
                    //realizando logout limpando localStorage
                    authService.logOut();
                    //redirecionando para home
                    $location.path('/home');
                }
            });
        });        
    }

    //Passando para o scope as informações que o serviço "authService" fornece de autenticação. Podendo assim,
    //termos controle do que é exibido por exemplo, na barra de menu botão Login, Logout, Bem-vindo, etc.
    $scope.authentication = authService.authentication;
}]);