var app = angular.module('betsApp', []);


app.controller('matchsController', function ($scope, $http) {

    $scope.matchs = null;
    $scope.matchsBets = null;

    $scope.walletFounds = 45;
    $scope.token = "8c320d7d-dd58-4d4d-a908-f1806cc75c41";
    $scope.bets = [];
    $scope.showRegion = "matchs";
    $scope.showBets = "simple";
    $scope.composedBetAmmount = 0;
    $scope.matchDetailIdx = null;
    $scope.SelectedSportID;
    $scope.SelectedSportName;

    $scope.getItems = function (leagueId) {
        $http({ method: 'GET', url: 'api/Events/GetActiveEvents/' + leagueId, headers: { 'Content-Type': 'text/plain; charset=utf-8' } })
            .success(function (data, status) {              
                $scope.matchs = data;
                $scope.matchDetailIdx = null;
                $scope.showRegion = "matchs";
            })
            .error(function (data, status) {
                alert('error al obtener datos');
            });
    };

    $scope.betMatchResult = function (matchId, betId, oddCode) {
        
        var oldBetIndex = GetBetIndexFromBets(betId);
        var matchIndex = GetMatchIndex(matchId);
        var betIndex = GetBetIndex(matchIndex, betId);
        var oddIndex = GetOddIndex(matchIndex, betIndex, oddCode);

        if (oldBetIndex >= 0) { //Habia apuestas para el partido.
            var newOdd = $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection[oddIndex];
            if (newOdd.code != $scope.bets[oldBetIndex].odd.code) {
                $scope.bets[oldBetIndex].odd = $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection[oddIndex];
            }
            else {
                removeBet(betId);
            }
        }
        else {
            addBet(matchIndex, betIndex, oddIndex);
        }
    };
        
    $scope.removeSimpleBet = function (betId) {
        var betIndex = GetBetIndexFromBets(betID);
        if (betIndex > -1) {
            if ($scope.bets[betIndex].composed == true) {
                $scope.bets[betIndex].simple = false;
            }
            else {
                $scope.bets.splice(betIndex, 1);
            }
        }
    }

    $scope.removeComposedBet = function (bet) {
        var betIndex = GetBetIndexFromBets(bet.betId);
        if (betIndex > -1) {
            if ($scope.bets[betIndex].simple == true) {
                $scope.bets[betIndex].composed = false;
            }
            else {
                $scope.bets.splice(betIndex, 1);
            }
        }
    }
    
    $scope.getBetKind = function (betId) {
        var betIndex = GetBetIndexFromBets(betId);

        switch ($scope.bets[betIndex].oddtype) {
            case "three way": return "Ganador del partido";                
        }
        return "Unknown bet type";
    }
    
    function RemoveOtherSelectedOptions(matchIndex, betIndex) {
        $.each($scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection, function (index, option) {
            if (option.selected == "selected") {
                unselectOption(matchIndex, betIndex, index);
                //removeBet(matchIndex, index);
            }            
        });
    }

    function selectOption(matchIndex, betIndex, oddIndex) {
        $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection[oddIndex].selected = "selected";
    }

    function unselectOption(matchIndex, betIndex, oddIndex) {
        $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection[oddIndex].selected = "";
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
        $http({ method: 'GET', url: 'ADOL.APP.Web/api/Events/GetEventOdds/' + matchCode, headers: { 'Content-Type': 'text/plain; charset=utf-8' } })
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
        $scope.bets = [];
    }

    $scope.hasFounds = function () {
        return betsAmmount() <= $scope.walletFounds;
    }

    $scope.availablefounds = function () {
        return ($scope.walletFounds - betsAmmount()).toFixed(2);
    }

    $scope.betsAmmount = function () {
        return betsAmmount().toFixed(2);
    }

    $scope.betsQuota = function () {
        if ($scope.showBets == "simple") {
            var winnings = 0, ammount = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    winnings = winnings + bet.odd.price * bet.ammount;
                    ammount += bet.ammount;
                }
            });
            return ammount != 0 ? (winnings / ammount).toFixed(2) : 0;
        }
        else if ($scope.showBets == "composed") {
            var quota = 1;
            $.each($scope.bets, function (index, bet) {
                if (bet.composed) {
                    quota = quota * bet.odd.price;
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
                    winnings = winnings + bet.odd.price * bet.ammount;
                }
            });
            return winnings.toFixed(2);
        }
        else if ($scope.showBets == "composed") {
            var quota = 1;
            $.each($scope.bets, function (index, bet) {
                if (bet.composed) {
                    quota = quota * bet.odd.price;
                }
            });
            return ($scope.composedBetAmmount * quota).toFixed(2);
        }
    }
    
    $scope.ConfirmBets = function () {

        var betsData = [];
        $.each($scope.bets, function (index, bet) {            
            betsData.push({ ID: bet.betId, Amount: bet.ammount, BetType: bet.odd.code });
        });
        var data = {
            token: $scope.token,
            betsType: $scope.showBets,
            uibets: betsData
        };
        $.ajax({
            type: 'POST',
            url: 'api/bet/AddUserBet',
            crossDomain: true,
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            success: function (responseData, textStatus, jqXHR) {
                alert('Apuestas realizadas.');
            },
            error: function (responseData, textStatus, errorThrown) {
                alert(responseData);
            }
        });
    }
    
    $scope.OptionText = function (match, bet, odd) {
        switch (bet.oddtype) {
            case "three way":
                {
                    switch (odd.code) {
                        case "tw_home":
                            return match.local;
                        case "tw_draw":
                            return "Empate";
                        case "tw_away":
                            return match.visitante;
                    }
                }
            default:
                return 'Default case';
                break;
        }        
    }

    $scope.isOddSelected = function (match, bet, odd) {
        var betIndex = GetBetIndexFromBets(bet.ID);
        if (betIndex >= 0) {
            return $scope.bets[betIndex].odd.code == odd.code;
        }
        else {
            return false;
        }
    }
    
    //Helper functions
    function betsAmmount() {
        if ($scope.showBets == "simple") {
            var total = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    total += bet.ammount;
                }
            });
            return parseFloat(total);
        }
        else if ($scope.showBets == "composed") {
            return $scope.composedBetAmmount;
        }
    }

    function GetMatchIndex(matchId) {
        return index = $scope.matchs.map(function (el) {
            return el.ID;
        }).indexOf(matchId);
    }
    
    function GetBetIndex(matchIndex, betId) {
        return index = $scope.matchs[matchIndex].apuestasDisponibles.map(function (el) {
            return el.ID;
        }).indexOf(betId);
    }

    function GetOddIndex(matchIndex, betIndex, oddCode) {
        return index = $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection.map(function (el) {
            return el.code;
        }).indexOf(oddCode);
    }

    function GetBetIndexFromBets(betID) {
        return betIndex = $scope.bets.map(function (el) {
            return el.betId;
        }).indexOf(betID);
    }

    function addBet(matchIndex, betIndex, oddIndex) {
        var bet = {
            betId: $scope.matchs[matchIndex].apuestasDisponibles[betIndex].ID,
            oddtype: $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddtype,
            match: {
                ID: $scope.matchs[matchIndex].ID,
                code: $scope.matchs[matchIndex].code,
                nombre: $scope.matchs[matchIndex].nombre,
                local: $scope.matchs[matchIndex].local,
                visitante: $scope.matchs[matchIndex].visitante,
                date: $scope.matchs[matchIndex].date,
                time: $scope.matchs[matchIndex].time
            },
            odd: $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection[oddIndex],
            simple: true,
            composed: true,
            ammount: 0
        };
        $scope.bets.push(bet);
    }

    function removeBet(betID) {
        var betIndex = GetBetIndexFromBets(betID);
        $scope.bets.splice(betIndex, 1);
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
                return parseFloat(value);
            });
        }
    };
});

