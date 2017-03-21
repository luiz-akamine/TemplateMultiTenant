app.service('shareDataService', function () {
    var myList = [];
    var value = null;

    var addList = function (newObj) {
        myList.push(newObj);
    }

    var setValue = function (newValue) {
        value = newValue;
    }

    var getList = function () {
        return myList;
    }

    var getValue = function () {
        return value;
    }

    return {
        addList: addList,
        getList: getList,
        setValue: setValue,
        getValue: getValue
    };
});