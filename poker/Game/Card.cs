using System;

public class Card
{
    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
        // duhet me fix qita 
        imagePath = "./" + suit.ToString() + rank.ToString() + ".png";
        

    }

    public string imagePath { get; set; }
    public Rank rank { get; private set; }
    public Suit suit { get; private set; }

    public override string ToString()
    {
        return rankToNumber(this.rank) + this.suit.ToString();
    }

    // public Card(String rank, String suit)
    // {
    //   (stringToSuit(suit),numberToRank(rank));

    // }

    public static Boolean equal(Card compare, Card comparable)
    {
        return (compare.rank == comparable.rank && compare.suit == comparable.suit);

    }
    public static Card getFromApi(String text)
    {

        return new Card(stringToSuit(text[0].ToString()), numberToRank(text[1].ToString()));

    }


    private static Suit stringToSuit(string suit)
    {
        return suit switch
        {
            "D" => Suit.D,
            "H" => Suit.H,
            "S" => Suit.S,
            "C" => Suit.C
        };

    }

    private static Rank numberToRank(string number)
    {
        return number switch
        {
            "2" => Rank.Deuce,
            "3" => Rank.Three,
            "4" => Rank.Four,
            "5" => Rank.Five,
            "6" => Rank.Six,
            "7" => Rank.Seven,
            "8" => Rank.Eight,
            "9" => Rank.Nine,
            "10" => Rank.Ten,
            "J" => Rank.Jack,
            "Q" => Rank.Queen,
            "K" => Rank.King,
            "A" => Rank.Ace

        };
    }
    private static string rankToNumber(Rank rank)
    {
        return rank switch
        {
            Rank.Deuce => "2",
            Rank.Three => "3",
            Rank.Four => "4",
            Rank.Five => "5",
            Rank.Six => "6",
            Rank.Seven => "7",
            Rank.Eight => "8",
            Rank.Nine => "9",
            Rank.Ten => "10",
            Rank.Jack => "J",
            Rank.Queen => "Q",
            Rank.King => "K",
            Rank.Ace => "A"
        };
    }
}