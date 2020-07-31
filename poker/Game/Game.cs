using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class Game
{
    public const int maxCapacity = 8;
    public string id { get; set; }
    public Deck deck { get; set; } = new Deck();
    public Player owner { get; set; }
    public List<Player> players = new List<Player>(maxCapacity);
    public int buyIn { get; set; }
    public int smallBlind { get; set; }
    public int bigBlind { get; set; }

    public int[] blindLocation = new int[2] { 0, 1 };

    // public int counter = 0;
    public int gamePot { get; set; }
    public List<Card> tableCards { get; set; } = new List<Card>();
    public Player currentPlayerTurn { get; set; }
    public bool openBet { get; set; } = false;

    public int callAmount { get; set; }


    public int round { get; set; } = 1;


    public void changeBlind()
    {
        blindLocation[0] = (blindLocation[0] + 1) % players.Count;
        blindLocation[1] = (blindLocation[1] + 1) % players.Count;
    }
    public Player getWinnerByCards(Card[] cards)
    {
        return players.Find(p => Card.equal(p.hand[0], cards[0]) && Card.equal(p.hand[1], cards[1]));
    }


    public string getWinnerCards()
    { //TODO fix this currently working but allah allah runa 
        using (var client = new HttpClient())
        {
            string result = "";
            string apiInput = "";
            foreach (var card in tableCards)
            {
                apiInput += card + ",";

            }
            apiInput = apiInput.Substring(0, apiInput.Length - 1);
            foreach (var player in players)
            {

                apiInput += "&pc[]=" + player.hand[0] + "," + player.hand[1];
            }
            var url = "https://api.pokerapi.dev/v1/winner/texas_holdem?cc=" + apiInput;
            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {

                var responseContent = response.Content;


                string responseString = responseContent.ReadAsStringAsync().Result;
                string pattern = @"[A-Z0-9]{2,3}";
                Regex rgx = new Regex(pattern);
                foreach (Match match in rgx.Matches(responseString))
                    result += match.Value + " ";
                return result.Split(" ")[0] + " " + result.Split(" ")[1];

            }

        }
        return null;
    }



    public void distributeCards()
    {// e kom hjek distributecards realistically method se problem with math 
        foreach (var player in players)
        {
            List<Card> playercards = new List<Card>();
            playercards.Add(deck.drawOne());
            playercards.Add(deck.drawOne());
            player.hand = playercards;

        }
    }

    // public void getTableCards()
    // {
    //     for (int i = 0; i < 5; i++)
    //     {
    //         this.tableCards.Add(deck.drawOne());

    //     }
    // }
    public Game(int buyIn, string ownerId)
    {
        id = Guid.NewGuid().ToString();
        // deck.generateDeck();
        owner = new Player(ownerId);
        players.Add(owner);
        currentPlayerTurn=players[0];
        smallBlind = (int)(buyIn * 0.05);
        bigBlind = (int)(buyIn * 0.10);
    }


    public Result addPlayer(string playerId)
    {
        var player = new Player(playerId);
        if (players.Exists(p => p.id == playerId))
            return new Result { success = false, message = "You have already joined" };


        if (player.money < buyIn)
            return new Result { success = false, message = "Your balance is too low to join this game" };

        player.currentGameMoney = buyIn;
        player.money -= buyIn;

        players.Add(player);

        return new Result { success = true, message = "You have joined this game" };
    }


    // public Boolean isGameEnding()
    // {
    //     return (round == 5 && currentPlayerTurn == players[players.Count]);


    // }
    public Result completeRound()
    {
        if (round == 5 && currentPlayerTurn == players[players.Count])
        {
            string winningCards = getWinnerCards();
            String[] cards = winningCards.Split(" ");

            Card[] winningHand = new Card[] { Card.getFromApi(cards[0]), Card.getFromApi(cards[1]) };
            var player = getWinnerByCards(winningHand);
            player.currentGameMoney += gamePot;
            gamePot = 0;
            callAmount = 0;
            round = 1;
            openBet = false;
            currentPlayerTurn = players[0];
            return new Result { success = true, message = $"The player who won is {player.id}" };

        }
        return new Result { success = false, message = "Game hasn't ended yet" };


    }

    public Boolean isLastPlayer()
    {
        int index = players.IndexOf(currentPlayerTurn);
        return index == players.Count - 1;

    }



    public void normaliseRound()
    {// i know qe unk eshte good practice
     // small blind big blind shit  
        if (isLastPlayer())
        {
            if (round == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    tableCards.Add(deck.drawOne());
                }
            }
            else if (round == 3 || round == 4)
            {
                tableCards.Add(deck.drawOne());

            }
            currentPlayerTurn = players[0];
            changeBlind();

            round++;
            if (round == 5)
                round = 0;
        }

    }
    public Result check(string playerId)
    {
        var validated = validatePlayerTurn(playerId);

        if (!validated.success)
            return validated;

        if (openBet)
        {
            currentPlayerTurn = getNextPlayer();
            normaliseRound();

            var res = completeRound();
            if (res.success)
                return res;
            return new Result { success = true, message = "Current player has checked" };
        }

        return new Result { success = false, message = "Cant check now " };

    }


    public Result call(string playerId)
    {
        var validated = validatePlayerTurn(playerId);

        if (!validated.success)
            return validated;

        if (!openBet)
            return new Result { success = false, message = "No open bet currently" };

        if (currentPlayerTurn.currentGameMoney < callAmount)
            return new Result { success = false, message = "You dont have the cash baby" };

        currentPlayerTurn.currentGameMoney -= callAmount;
        gamePot += callAmount;
        currentPlayerTurn = getNextPlayer();
        normaliseRound();
        var res = completeRound();
        if (res.success)
            return res;
        return new Result { success = true, message = "Current player has called" };

    }


    public Result raise(string playerId, int amount)
    {
        var validated = validatePlayerTurn(playerId);

        if (!validated.success)
            return validated;

        if (!openBet)
            return new Result { success = false, message = "Cant raise without someone betting you bitch" };

        if (currentPlayerTurn.currentGameMoney < amount)
            return new Result { success = false, message = "You dont have the cash baby" };

        currentPlayerTurn.currentGameMoney -= amount;
        gamePot += amount;
        callAmount = amount;
        currentPlayerTurn = getNextPlayer();
        normaliseRound();
        var res = completeRound();
        if (res.success)
            return res;
        return new Result { success = true, message = "Current player has raised" };



    }

    public Result bet(string playerId, int amount)
    {
        var validated = validatePlayerTurn(playerId);

        if (!validated.success)
            return validated;

        if (currentPlayerTurn.currentGameMoney < amount)
            return new Result { success = false, message = "You don't have the cash man" };

        if (openBet)
            return new Result { success = false, message = "You can call but not bet" };


        currentPlayerTurn.currentGameMoney -= amount;
        gamePot += amount;
        openBet = true;
        currentPlayerTurn = getNextPlayer();
        normaliseRound();
        var res = completeRound();
        if (res.success)
            return res;
        if (currentPlayerTurn.currentGameMoney == 0)
            return new Result { success = true, message = "ALL IN" };
        // e di qe ka string literals c# po pritova mi lyp qysh jon 
        else return new Result { success = true, message = "Current user has raised:" + amount + "$" };



    }



    public Result fold(string playerId)
    {
        var validated = validatePlayerTurn(playerId);

        if (!validated.success)
            return validated;

        var player = players.FirstOrDefault(p => p.id == playerId);

        player.folded = true;

        if (players.Where(p => !p.folded).ToList().Count == 1)
        {
            player.currentGameMoney += gamePot;
            gamePot = 0;
            callAmount = 0;
            round = 1;
            openBet = false;
            currentPlayerTurn = players[0];

            return new Result { success = true, message = $"Te gjithe kane fold,loja perfundoi" };

        }

        currentPlayerTurn = getNextPlayer();
        normaliseRound();
        var res = completeRound();
        if (res.success)
            return res;
        return new Result { success = true };
    }



    public Player getNextPlayer()
    {

        int index = this.players.IndexOf(currentPlayerTurn);
        return this.players[(index + 1) % this.players.Count];


    }

    private Result validatePlayerTurn(string playerId)
    {
        if (!players.Exists(p => p.id == playerId))
            return new Result { success = false, message = "You're not in this game" };

        if (playerId != currentPlayerTurn.id)
            return new Result { success = false, message = "It's not your turn" };

        return new Result { success = true };
    }
}