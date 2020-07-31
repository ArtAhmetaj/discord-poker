using System.Collections.Generic;

public class Player
{

    public Player(string id)
    {
        // if( exists in db veq nxerre me 1 funksion statik garnat qe kthen player)
        // else veq krijoje 1 user te ri ne db me qit discordname
        this.id = id;
        this.money = 1000;
        // this.wins = 0;
    }

    public List<Card> hand { get; set; } = new List<Card>(2);

    public bool folded { get; set; }

    public string id { get; }
    public double money { get; set; }
    public int currentGameMoney { get; set; }
    public int wins { get; set; }
}