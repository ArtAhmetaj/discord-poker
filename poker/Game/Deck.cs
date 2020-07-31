using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Deck
{
    private Stack<Card> deck { get; set; } = new Stack<Card>(52);


    public Deck()
    {
        this.generateDeck();
    }
    public void generateDeck()
    {
        List<Card> cards = new List<Card>();
        foreach (var rank in Enum.GetValues(typeof(Rank)))
        {
            foreach (var suit in Enum.GetValues(typeof(Suit)))
            {
                cards.Add(new Card((Suit)suit, (Rank)rank));
            }
        }

        // shuffle cards
        cards = cards.OrderBy(a => Guid.NewGuid()).ToList();

        foreach (var card in cards)
        {
            deck.Push(card);
        }
    }


    // public void clearDeck()
    // {
    //     while (deck.Count != 0)
    //     {
    //         deck.Pop();
    //     }
    // }
    public Card drawOne()
    {
        return deck.Pop();
    }
    public void printDeck()
    {
        foreach (var Card in deck)
        {
            Console.WriteLine(Card);
        }
    }



}