using System.Collections.Generic;
public static class GameFactory
{

    public static Dictionary<string, Game> games;

    public static string createGame(string id, int buyIn, string channelId)
    {
        if (games == null)
            games = new Dictionary<string, Game>();

        Player player = new Player(id);
        if (player.money < buyIn)
            return null;
        Game game = new Game(buyIn, id);
        if (games.ContainsKey(channelId))
            return null;
        games[channelId] = game;
        return games[channelId].id;

        // mire 

    }

    public static Game GetGame(string id)
    {
        if (games == null)
            games = new Dictionary<string, Game>();
        if (games.ContainsKey(id))
            return games[id];

        return null;

    }

}