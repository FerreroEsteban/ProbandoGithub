var app = angular.module('betsApp', []);


app.controller('matchsController', function ($scope, $http, $sce) {


    $scope.matchs = null;
    $scope.detailMatch = null;

    $scope.walletFounds = 45;
    $scope.token = "";
    $scope.bets = [];
    $scope.pendingBets = [];
    $scope.showRegion = "matchs";
    $scope.showBets = "pending";
    $scope.composedBetAmount = 0;
    $scope.matchDetailIdx = null;
    $scope.breadcrumbPath;

    getPendingBets();



    $scope.getItems = function (leagueId, path) {
        getItems(leagueId, path);
        getPendingBets();
    };

    $scope.getPendingBets = function () {
        getPendingBets();
        $scope.showBets = "pending";
    };

    $scope.getBreadcrumb = function () {
        if ($scope.breadcrumbPath == null) {
            return "";
        }
        var breadcrumbItems = $scope.breadcrumbPath.split(',');
        var breadcrumb = "";
        if (breadcrumbItems.length) {
            for (var i = 0; i < breadcrumbItems.length; i++) {
                breadcrumb += '<span>' + breadcrumbItems[i] + '</span>';
            }
        }
        return $sce.trustAsHtml(breadcrumb);
    }

    $scope.setBreadcrumbPath = function (path) {
        $scope.breadcrumbPath = path;
    }

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
        removeSimpleBet(betId);
    }

    $scope.removeComposedBet = function (betId) {
        removeComposedBet(betId);
    }
    
    $scope.betsAvailable = function () {
        if ($scope.pendingBets != null) {
            if ($scope.pendingBets.length > 0) {
                return true;
            }
        }
        if ($scope.bets != null) {
            if ($scope.bets.length > 0) {
                return true;
            }
        }

        return false;
    }

    $scope.areBets = function (simple) {
        if ($scope.bets == null || $scope.bets.length == 0) {
            return false;
        }

        if (simple) {
            for (var i = 0; i < $scope.bets.length; i++) {
                if ($scope.bets[i].simple) {
                    return $scope.bets[i].simple;
                }
            }
        } else {
            for (var i = 0; i < $scope.bets.length; i++) {
                if ($scope.bets[i].composed) {
                    return $scope.bets[i].composed;
                }
            }
        }

        return false;
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

    $scope.ViewMathcBets = function (matchCode, matchIndex) {
        $http({ method: 'GET', url: 'api/Events/GetEventOdds/' + matchCode, headers: { 'Content-Type': 'text/plain; charset=utf-8' } })
             .success(function (data, status) {
                 $scope.showRegion = "odds";
                 $scope.matchDetailIdx = matchIndex;
                 $scope.detailMatch = $scope.matchs[matchIndex];
                 $scope.detailMatch.apuestasDisponibles = data.data;
             })
             .error(function (data, status) {
                 $scope.showRegion = "matchs";
             });
    }

    $scope.removeAllBets = function () {
        $scope.bets = [];
        showPendingBets();
    }

    $scope.hasFounds = function () {
        return betsAmount() <= $scope.walletFounds;
    }

    $scope.canConfirmBets = function () {
        return betsAmount() <= $scope.walletFounds && betsAmount() > 0;
    }

    $scope.availablefounds = function () {
        return ($scope.walletFounds - betsAmount()).toFixed(2);
    }

    $scope.betsAmount = function () {
        return betsAmount().toFixed(2);
    }

    $scope.betsQuota = function () {
        if ($scope.showBets == "simple") {
            var winnings = 0, amount = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    winnings = winnings + bet.odd.price * bet.amount;
                    amount += bet.amount;
                }
            });
            return amount != 0 ? (winnings / amount).toFixed(2) : 0;
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
                    winnings = winnings + bet.odd.price * bet.amount;
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
            return ($scope.composedBetAmount * quota).toFixed(2);
        }
    }

    $scope.ConfirmBets = function () {

        var betsData = [];
        var simpleBetsToRemove = [];
        var composedBetsToRemove = [];

        if ($scope.showBets == "simple") {
            $.each($scope.bets, function (index, bet) {
                if (bet.simple && bet.amount > 0) {
                    betsData.push({ ID: bet.betId, Amount: bet.amount, BetType: bet.odd.code });
                    simpleBetsToRemove.push(bet.betId);
                }
            });
        } else if ($scope.showBets == "composed") {
            $.each($scope.bets, function (index, bet) {
                if (bet.composed && $scope.composedBetAmount > 0) {
                    betsData.push({ ID: bet.betId, Amount: $scope.composedBetAmount, BetType: bet.odd.code });
                    composedBetsToRemove.push(bet.betId);
                }
            });
        }

        if (betsData.length > 0) {
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
                    if (responseData != null && responseData.lastError == "") {
                        $scope.walletFounds = responseData.balance;
                        $scope.userName = responseData.userName;
                        if (responseData.data.length > 0) {
                            $scope.pendingBets = responseData.data;
                            $.each(composedBetsToRemove, function (index, betID) {
                                removeComposedBet(betID);
                            });
                            $.each(simpleBetsToRemove, function (index, betID) {
                                removeSimpleBet(betID);
                            });
                            showPendingBets();
                        }
                    }
                },
                error: function (responseData, textStatus, errorThrown) {
                    alert(responseData.lastError);
                }
            });
        }
    }

    $scope.OptionText = function (match, oddType, oddCode) {
        switch (oddType) {
            case "three way":
            case "three way - ht":
            case "three way - 2nd hf":
                {
                    switch (oddCode) {
                        case "tw_home":
                            return match.local;
                        case "tw_draw":
                            return "Empate";
                        case "tw_away":
                            return match.visitante;
                    }
                }
            case "odd/even":
                {
                    switch (oddCode) {
                        case "Odd":
                            return "Par";
                        case "Even":
                            return "Impar";
                    }
                }
            case "draw no bet":
                {
                    switch (oddCode) {
                        case "Home":
                            return match.local;
                        case "Away":
                            return match.visitante;
                    }
                }
            case "double chance (1x/x2/12)":
                {
                    switch (oddCode) {
                        case "1x":
                            return match.local + "-Empate";
                        case "x2":
                            return "Empate-" + match.visitante;
                        case "12":
                            return match.local + "-" + match.visitante;
                    }
                }
            default:
                return 'Default case';
                break;
        }
    }

    $scope.getBetKind = function (oddType) {
        switch (oddType) {
            case "three way": return "Ganador del partido";
            case "three way - ht": return "Ganador del primer tiempo";
            case "three way - 2nd hf": return "Ganador del segundo tiempo";
            case "odd/even": return "Cantidad de goles";
            case "double chance (1x/x2/12)": return "Doble chance";
            case "draw no bet": return "Victoria sin empate";
        }
        return "Unknown bet type";
    }

    $scope.OddClass = function (oddType) {
        switch (oddType) {
            case "three way":
            case "three way - ht":
            case "three way - 2nd hf":
            case "double chance (1x/x2/12)":
                {
                    return "tres";
                }
            case "odd/even":
            case "draw no bet":
                {
                    return "dos";
                }
            default:
                return '';
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
    function getItems(leagueId, path) {
        $scope.breadcrumbPath = path;
        $http({ method: 'GET', url: 'api/Events/GetActiveEvents/' + leagueId, headers: { 'Content-Type': 'text/plain; charset=utf-8' } })
            .success(function (data, status) {
                if (data != null && data.lastError == "") {
                    $scope.walletFounds = data.balance;
                    $scope.userName = data.userName;
                    $scope.matchs = data.data;
                    $scope.matchDetailIdx = null;
                    $scope.showRegion = "matchs";
                }
                else {
                    alert(data.lastError);
                }
            })
            .error(function (data, status) {
                alert(data.lastError);
            });
    }

    function getPendingBets() {
        $http({ method: 'GET', url: 'api/bet/getuserbet/' + $scope.token, headers: { 'Content-Type': 'text/plain; charset=utf-8' } })
            .success(function (data, status) {
                if (data != null && data.lastError == "") {
                    $scope.walletFounds = data.balance;
                    $scope.userName = data.userName;
                    if (data.data.length > 0) {
                        $scope.pendingBets = data.data;
                    }
                }
            })
            .error(function (data, status) {
                alert(data.lastError);
            });
    }

    function RemoveOtherSelectedOptions(matchIndex, betIndex) {
        $.each($scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddCollection, function (index, option) {
            if (option.selected == "selected") {
                unselectOption(matchIndex, betIndex, index);
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

    function betsAmount() {
        if ($scope.showBets == "simple") {
            var total = 0;
            $.each($scope.bets, function (index, bet) {
                if (bet.simple) {
                    total += bet.amount;
                }
            });
            return parseFloat(total);
        }
        else if ($scope.showBets == "composed") {
            return $scope.composedBetAmount;
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

    function GetBetIndexFromPendingBets(betID) {
        return betIndex = $scope.pendingBets.map(function (el) {
            return el.betId;
        }).indexOf(betID);
    }

    function addBet(matchIndex, betIndex, oddIndex) {
        var bet = {
            betId: $scope.matchs[matchIndex].apuestasDisponibles[betIndex].ID,
            oddType: $scope.matchs[matchIndex].apuestasDisponibles[betIndex].oddType,
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
            amount: 0
        };
        
        $scope.bets.push(bet);
        if ($scope.bets.length == 1) {
            showBets();
        }
    }

    function removeBet(betID) {
        var betIndex = GetBetIndexFromBets(betID);
        $scope.bets.splice(betIndex, 1);
        if ($scope.bets.length == 0) {
            showPendingBets();
        }
        else {
            showBets();
        }
    }

    function removeSimpleBet(betId) {
        var betIndex = GetBetIndexFromBets(betId);
        if (betIndex > -1) {
            if ($scope.bets[betIndex].composed == true) {
                $scope.bets[betIndex].simple = false;
            }
            else {
                $scope.bets.splice(betIndex, 1);
                if ($scope.bets.length == 0) {
                    showPendingBets();
                }
                else {
                    showBets();
                }
            }
        }
    }

    function removeComposedBet(betId) {
        var betIndex = GetBetIndexFromBets(betId);
        if (betIndex > -1) {
            if ($scope.bets[betIndex].simple == true) {
                $scope.bets[betIndex].composed = false;
                showBets();
            }
            else {
                $scope.bets.splice(betIndex, 1);
                if ($scope.bets.length == 0) {
                    showPendingBets();
                }
                else {
                    showBets();
                }
            }
        }
    }

    function showPendingBets() {
        $("#WaitingResultBets")[0].click()
        $scope.showBets = "pending";
    }

    function showBets() {

        if ($scope.bets != null) {
            if ($scope.bets.length > 0) {
                var simpleBets = false;
                var composedBets = false;

                for (var i = 0; i < $scope.bets.length; i++) {
                    if ($scope.bets[i].simple) {
                        simpleBets = true;
                    }
                    if ($scope.bets[i].composed) {
                        composedBets = true;
                    }
                }
                
                if ($scope.showBets == "simple" && !simpleBets && composedBets) {                    
                    $("#ComposedBets")[0].click()
                    $scope.showBets = "composed";
                    return;
                }

                if ($scope.showBets == "composed" && simpleBets && !composedBets) {
                    $("#SimpleBets")[0].click()
                    $scope.showBets = "simple";
                    return;
                }
            }
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
                    value = 0;
                }
                return parseFloat(value);
            });
        }
    };
});

