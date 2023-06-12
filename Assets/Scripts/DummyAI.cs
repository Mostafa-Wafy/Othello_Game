using System.Collections.Generic;
using UnityEngine;

public class DummyAIPlayer
{
    public Player player;
    private System.Random random;

    public DummyAIPlayer(Player player)
    {
        this.player = player;
        random = new System.Random();
    }

    public Position MakeaMove(GameState gameState)
    {
        List<Position> legalMoves = new List<Position>(gameState.LegalMoves.Keys);
        int randomIndex = random.Next(legalMoves.Count);
        Position randomMove = legalMoves[randomIndex];
        return randomMove;
    }
}






