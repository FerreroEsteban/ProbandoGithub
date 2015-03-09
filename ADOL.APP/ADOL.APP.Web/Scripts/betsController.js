var app = angular.module('betsApp', []);


app.controller('matchsController', function ($scope) {

    $scope.matchs = [
       { betPerformed: false, date: "25 feb", time: "18:00hs", options: [{ team: "Independiente", value: 1.30, selected: "" }, { team: "Empate", value: 2.00, selected: "" }, { team: "Racing", value: 4.30, selected: "" }] },
       { betPerformed: false, date: "25 feb", time: "19:00hs", options: [{ team: "River", value: 1.20, selected: "" }, { team: "Empate", value: 3.00, selected: "" }, { team: "Boca", value: 3.30, selected: "" }] },
{ betPerformed: false, date: "25 feb", time: "20:00hs", options: [{ team: "Velez", value: 1.40, selected: "" }, { team: "Empate", value: 1.00, selected: "" }, { team: "San Lorenzo", value: 2.30, selected: "" }] }];

    $scope.lastBetId = 1;
    $scope.bets = [];
    $scope.showBets = "simple";
    $scope.composedBetAmmount = 0;
       
    $scope.selectMatchOption = function (matchIndex, optionIndex) {
        if ($scope.matchs[matchIndex].options[optionIndex].selected != "selected") {
            RemoveOtherSelectedOptions(matchIndex);
            addBet(matchIndex, optionIndex);
            selectOption(matchIndex, optionIndex);
        }
        else {
            removeBet(matchIndex, optionIndex);
            unselectOption(matchIndex, optionIndex);
        }
    };

    $scope.removeSimpleBet = function (betId) {
        var betIndex = findBetIndexByID(betId);
        if (betIndex > -1) {
            if ($scope.bets[betIndex].composed == true) {
                $scope.bets[betIndex].simple = false;
            }
            else {
                unselectOption($scope.bets[betIndex].match, $scope.bets[betIndex].option);
                $scope.bets.splice(betIndex, 1);
            }
        }
    }

    $scope.removeComposedBet = function (betId) {
        var betIndex = findBetIndexByID(betId);
        if (betIndex > -1) {
            if ($scope.bets[betIndex].simple == true) {
                $scope.bets[betIndex].composed = false;
            }
            else {
                unselectOption($scope.bets[betIndex].match, $scope.bets[betIndex].option);
                $scope.bets.splice(betIndex, 1);
            }
        }
    }

    function RemoveOtherSelectedOptions(matchIndex) {
        $.each($scope.matchs[matchIndex].options, function (index, option) {
            if (option.selected = "selected") {
                unselectOption(matchIndex, index);
                removeBet(matchIndex, index);
            }            
        });
    }

    function addBet(matchIndex, optionIndex) {
        var bet = { betId: $scope.lastBetId++, match: matchIndex, option: optionIndex, simple: true, composed: true, ammount: 1 };
        $scope.bets.push(bet);
    }

    function removeBet(matchIndex, optionIndex) {
        var betIndex = findBetIndexByValues(matchIndex, optionIndex);
        if (betIndex > -1) {
            $scope.bets.splice(betIndex, 1);
        }
    }


    function selectOption(matchIndex, optionIndex) {
        $scope.matchs[matchIndex].options[optionIndex].selected = "selected";
    }

    function unselectOption(matchIndex, optionIndex) {
        $scope.matchs[matchIndex].options[optionIndex].selected = "";
    }


    function findBetIndexByValues(matchIndex, optionIndex) {
        for (var i = 0; i < $scope.bets.length; i++) {
            if ($scope.bets[i].match == matchIndex && $scope.bets[i].option == optionIndex) {
                return i;
            }
        }
        return -1;
    }

    function findBetIndexByID(betId) {
        for (var i = 0; i < $scope.bets.length; i++) {
            if ($scope.bets[i].betId == betId) {
                return i;
            }
        }
        return -1;
    }

    $scope.filterSimpleBets = function (bet) {
        return bet.simple;
    }

    $scope.filterComposedBets = function (bet) {
        return bet.composed;
    }

    $scope.simpleBetsEnabled = function () {
        $scope.showBets = "simple";
    }

    $scope.composedBetsEnabled = function () {        
        $scope.showBets = "composed";
    }

    $scope.betsAmmount = function () {
        if ($scope.showBets == "simple") {
            var total = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    total += bet.ammount;
                }
            });
            return total.toFixed(2);
        }
        else if ($scope.showBets == "composed") {
            return $scope.composedBetAmmount.toFixed(2);
        }
    }

    $scope.betsQuota = function() {
        if ($scope.showBets == "simple") {
            var winnings = 0, ammount = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    winnings = winnings + $scope.matchs[bet.match].options[bet.option].value * bet.ammount;
                    ammount += bet.ammount;
                }
            });
            return (winnings / ammount).toFixed(2);
        }
        else if ($scope.showBets == "composed") {
            var quota = 1;
            $.each($scope.bets, function (index, bet) {
                if (bet.composed) {
                    quota = quota * $scope.matchs[bet.match].options[bet.option].value;
                }
            });
            return quota.toFixed(2);
        }
    }

    $scope.betsWinnings = function() {
        if ($scope.showBets == "simple") {
            var winnings = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    winnings = winnings + $scope.matchs[bet.match].options[bet.option].value * bet.ammount;
                }
            });
            return winnings.toFixed(2);
        }
        else if ($scope.showBets == "composed") {
            var quota = 1;
            $.each($scope.bets, function (index, bet) {
                if (bet.composed) {
                    quota = quota * $scope.matchs[bet.match].options[bet.option].value;
                }
            });
            return ($scope.composedBetAmmount * quota).toFixed(2);
        }
    }

});

app.directive('numericsaving', function () {
    return {
        restrict: 'A',
        require: '?ngModel',
        scope: {
            model: '=ngModel'
        },
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }
            ngModelCtrl.$parsers.push(function (value) {
                if (!value || value==='' || isNaN(parseInt(value)) || parseInt(value)!=value) {
                    value=0;
                }
                return parseInt(value);
            });
        }
    };
});

