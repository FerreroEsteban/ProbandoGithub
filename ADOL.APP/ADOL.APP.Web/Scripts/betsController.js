﻿var app = angular.module('betsApp', []);


app.controller('matchsController', function ($scope, $http) {

    $scope.matchs = null;
    $scope.matchsBets = null;

    $scope.walletFounds = 45;
    $scope.bets = [];
    $scope.showRegion = "matchs";
    $scope.showBets = "simple";
    $scope.composedBetAmmount = 0;
    $scope.matchDetailIdx = null;
    $scope.SelectedSportID;
    $scope.SelectedSportName;

    $scope.getItems = function (leagueId) {
        $http({ method: 'GET', url: 'http://localhost:55737/api/Events/GetActiveEvents/'+leagueId, headers: { 'Content-Type': 'text/plain; charset=utf-8' } })
            .success(function (data, status) {
                
                if ($scope.matchs != null) {
                    $.each($scope.matchs, function (mIndex, match) {
                        $.each(match.ApuestasDisponibles[0].OddCollection, function (oIndex, option) {
                            if (option.selected == "selected") {
                                data[mIndex].ApuestasDisponibles[0].OddCollection[oIndex].selected = "selected";
                            }
                        });
                    });
                }
                $scope.matchs = data;
                $scope.matchDetailIdx = null;
                $scope.showRegion = "matchs";
            })
            .error(function (data, status) {
                alert('error al obtener datos');
            });
    };

    $scope.selectMatchOption = function (matchIndex, optionIndex, kindOfBet, optionLabel) {
        if ($scope.matchs[matchIndex].ApuestasDisponibles[0].OddCollection[optionIndex].selected != "selected") {
            RemoveOtherSelectedOptions(matchIndex);
            addBet(matchIndex, optionIndex, kindOfBet, optionLabel);
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
                alert('quita');
                $scope.bets[betIndex].simple = false;
            }
            else {
                alert('no encuentra');
                unselectOption($scope.bets[betIndex].match, $scope.bets[betIndex].option);
                $scope.bets.splice(betIndex, 1);
            }
        }
    }

    $scope.removeComposedBet = function (betId) {
        var betIndex = findBetIndexByID(betId);
        if (betIndex > -1) {
            if ($scope.bets[betIndex].simple == true) {
                alert('quita');
                $scope.bets[betIndex].composed = false;
            }
            else {
                alert('no encuentra');
                unselectOption($scope.bets[betIndex].match, $scope.bets[betIndex].option);
                $scope.bets.splice(betIndex, 1);
            }
        }
    }

    function RemoveOtherSelectedOptions(matchIndex) {
        $.each($scope.matchs[matchIndex].ApuestasDisponibles[0].OddCollection, function (index, option) {
            if (option.selected == "selected") {
                unselectOption(matchIndex, index);
                removeBet(matchIndex, index);
            }            
        });
    }

    function addBet(matchIndex, optionIndex, kindOfBet, optionLabel) {
        var bet = { betId: $scope.matchs[matchIndex].ApuestasDisponibles[0].ID, match: matchIndex, option: optionIndex, kind: kindOfBet, label: optionLabel, simple: true, composed: true, ammount: 0 };
        $scope.bets.push(bet);
    }

    function removeBet(matchIndex, optionIndex) {
        var betIndex = findBetIndexByValues(matchIndex, optionIndex);
        if (betIndex > -1) {
            $scope.bets.splice(betIndex, 1);
        }
    }
    
    function selectOption(matchIndex, optionIndex) {
        $scope.matchs[matchIndex].ApuestasDisponibles[0].OddCollection[optionIndex].selected = "selected";
    }

    function unselectOption(matchIndex, optionIndex) {
        $scope.matchs[matchIndex].ApuestasDisponibles[0].OddCollection[optionIndex].selected = "";
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

    $scope.SelectSport = function (sportId, spName) {
        $scope.SelectedSportID = sportId;
        $scope.SelectedSportName = spName;
    }

    $scope.ViewMathcBets = function (matchCode, matchIndex) {
        $http({ method: 'GET', url: 'http://localhost:55737/api/Events/GetEventOdds/' + matchCode, headers: { 'Content-Type': 'text/plain; charset=utf-8' } })
             .success(function (data, status) {
                 $scope.showRegion = "odds";
                 $scope.matchDetailIdx = matchIndex;
                 $scope.matchsBets = data;
             })
             .error(function (data, status) {
                 $scope.showRegion = "matchs";
             });
    }

    $scope.removeAllBets = function () {

        if ($scope.matchs != null) {
            $.each($scope.matchs, function (mIndex, match) {
                $.each(match.ApuestasDisponibles[0].OddCollection, function (oIndex, option) {
                    if (option.selected == "selected") {
                        $scope.matchs[mIndex].ApuestasDisponibles[0].OddCollection[oIndex].selected = "";
                    }
                });
            });
        }
        $scope.bets = [];
    }

    $scope.hasFounds = function () {
        return betsAmmount() <= $scope.walletFounds;
    }

    $scope.availablefounds = function () {
        return $scope.walletFounds - betsAmmount();
    }

    $scope.betsAmmount = function () {
        return betsAmmount().toFixed(2);
    }

    $scope.betsQuota = function () {
        if ($scope.showBets == "simple") {
            var winnings = 0, ammount = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    winnings = winnings + $scope.matchs[bet.match].ApuestasDisponibles[0].OddCollection[bet.option].Price * bet.ammount;
                    ammount += bet.ammount;
                }
            });
            return ammount != 0 ? (winnings / ammount).toFixed(2) : 0;
        }
        else if ($scope.showBets == "composed") {
            var quota = 1;
            $.each($scope.bets, function (index, bet) {
                if (bet.composed) {
                    quota = quota * $scope.matchs[bet.match].ApuestasDisponibles[0].OddCollection[bet.option].Price;
                }
            });
            return quota.toFixed(2);
        }
    }

    $scope.betsWinnings = function () {
        if ($scope.showBets == "simple") {
            var winnings = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    winnings = winnings + $scope.matchs[bet.match].ApuestasDisponibles[0].OddCollection[bet.option].Price * bet.ammount;
                }
            });
            return winnings.toFixed(2);
        }
        else if ($scope.showBets == "composed") {
            var quota = 1;
            $.each($scope.bets, function (index, bet) {
                if (bet.composed) {
                    quota = quota * $scope.matchs[bet.match].ApuestasDisponibles[0].OddCollection[bet.option].Price;
                }
            });
            return ($scope.composedBetAmmount * quota).toFixed(2);
        }
    }

    function betsAmmount(){
        if ($scope.showBets == "simple") {
            var total = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    total += bet.ammount;
                }
            });
            return total;
        }
        else if ($scope.showBets == "composed") {
            return $scope.composedBetAmmount;
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
                if (!value || value === '' || isNaN(parseFloat(value)) || parseFloat(value) != value) {
                    value=0;
                }
                return parseFloat(value).toFixed(2);
            });
        }
    };
});

