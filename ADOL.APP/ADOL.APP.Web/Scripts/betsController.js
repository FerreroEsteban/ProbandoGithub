var app = angular.module('betsApp', []);


app.controller('matchsController', function ($scope) {

    $scope.matchs = [
       { betPerformed: false, date: "25 feb", time: "18:00hs", options: [{ team: "Independiente", value: "1.30", selected: "" }, { team: "Empate", value: "2.00", selected: "" }, { team: "Racing", value: "4.30", selected: "" }] },
       { betPerformed: false, date: "25 feb", time: "19:00hs", options: [{ team: "River", value: "1.20", selected: "" }, { team: "Empate", value: "3.00", selected: "" }, { team: "Boca", value: "3.30", selected: "" }] },
{ betPerformed: false, date: "25 feb", time: "20:00hs", options: [{ team: "Velez", value: "1.40", selected: "" }, { team: "Empate", value: "1.00", selected: "" }, { team: "San Lorenzo", value: "2.30", selected: "" }] }];

    $scope.lastBetId = 1;
    $scope.bets = [];

    $scope.selectMatchOption = function (matchIndex, optionIndex) {
        if ($scope.matchs[matchIndex].options[optionIndex].selected != "selected") {
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

    function addBet(matchIndex, optionIndex) {
        var bet = { betId: $scope.lastBetId++, match: matchIndex, option: optionIndex, simple: true, composed: true };
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
});



