using System.Collections.Generic;
using UnityEngine;

public class HardAIPlayer
{
    private int depthLimit = 3;
    public Player player;
    

    public HardAIPlayer(Player player)
    {
        this.player = player;
    }

    
    private int Minimax(GameState state, int depth, int alpha, int beta, bool isMaximizingPlayer)
    {
        // Base case: evaluate the game state if maximum depth is reached or the game is over
        if (depth == depthLimit || state.GameOver)
        {
            return Evaluate(state);
        }

        // Recursive case
        List<Position> legalMoves = new List<Position>(state.AvailableMoves.Keys);
        int bestScore;

        if (isMaximizingPlayer)
        {
            bestScore = int.MinValue;

            foreach (Position move in legalMoves)
            {
                GameState clonedState = state.Clone();
                clonedState.MakeMove(move, out MovementInformation movementInfo); // Make the move and get the movement info

                int score = Minimax(clonedState, depth + 1, alpha, beta, false); // Recursive call with the new state
                bestScore = Mathf.Max(bestScore, score); // Update best score so far
                alpha = Mathf.Max(alpha, bestScore); // Update alpha

                if (beta <= alpha)
                {
                    // Beta cutoff
                    break;
                }
            }
        }
        else
        {
            bestScore = int.MaxValue;

            foreach (Position move in legalMoves)
            {
                GameState clonedState = state.Clone();
                clonedState.MakeMove(move, out MovementInformation movementInfo); // Make the move and get the movement info

                int score = Minimax(clonedState, depth + 1, alpha, beta, true); // Recursive call with the new state
                bestScore = Mathf.Min(bestScore, score); // Update best score so far
                beta = Mathf.Min(beta, bestScore); // Update beta

                if (beta <= alpha)
                {
                    // Alpha cutoff
                    break;
                }
            }
        }

        return bestScore;
    }

    private int Evaluate(GameState state)
    {
        
        int corners_Score = Evaluate_Corners_Captured(state);

        int stability_Score = Evaluate_Stability(state);

        int mobility_Score = Evaluate_Mobility(state);
        
        int coinParity_Score = Evaluate_CoinParity(state);
        
        //int patternScore = Evaluate_Pattern_Recognition(state);

        
        int totalScore = 3*mobility_Score + 2*coinParity_Score + 10*corners_Score + 10*stability_Score; //+ 5*patternScore;
        return totalScore;
    }

    private int Evaluate_Mobility(GameState state)
    {
        List<Position> legalMoves = new List<Position>(state.AvailableMoves.Keys);
        int currentPlayerMoves = legalMoves.Count;

        // Count the opponent's legal moves
        Player opponentPlayer = Get_Opponent(state.CurrentPlayer);
        state.ChangePlayer();
        List<Position> opponentLegalMoves = new List<Position>(state.AvailableMoves.Keys);
        int opponentMoves = opponentLegalMoves.Count;
        state.ChangePlayer();

        // Calculate the mobility score as the difference in the number of moves
        int mobility_Score = currentPlayerMoves - opponentMoves;

        return mobility_Score;
    }

    private int Evaluate_CoinParity(GameState state)
    {
        int currentPlayerDisks = state.CoinCount[state.CurrentPlayer];
        int opponentPlayerDisks = state.CoinCount[Get_Opponent(state.CurrentPlayer)];

    
        int coinParity_Score = currentPlayerDisks - opponentPlayerDisks;

        return coinParity_Score;
    }

    private int Evaluate_Corners_Captured(GameState state)
    {
        int currentPlayerCorners = Count_Corners_Captured(state, state.CurrentPlayer);
        int opponentPlayerCorners = Count_Corners_Captured(state, Get_Opponent(state.CurrentPlayer));


        int corners_Score = currentPlayerCorners - opponentPlayerCorners;

        return corners_Score;
    }

    private int Count_Corners_Captured(GameState state, Player player)
    {
        int cornersCaptured = 0;

        if (state.Board[0, 0] == player)
            cornersCaptured++;
        if (state.Board[0, GameState.Cols - 1] == player)
            cornersCaptured++;
        if (state.Board[GameState.Rows - 1, 0] == player)
            cornersCaptured++;
        if (state.Board[GameState.Rows - 1, GameState.Cols - 1] == player)
            cornersCaptured++;

        return cornersCaptured;
    }

    private Player Get_Opponent(Player player)
    {
        return player == Player.Black ? Player.White : Player.Black;
    }

    private int Evaluate_Stability(GameState state)
    {
        int stability_Score = 0;

        
        List<Position> stablePositions = new List<Position>
        {
            new Position(0, 0), new Position(0, GameState.Cols - 1),
            new Position(GameState.Rows - 1, 0), new Position(GameState.Rows - 1, GameState.Cols - 1)
        };

        
        foreach (Position position in state.OccupiedPositions())
        {
            Player player = state.Board[position.R, position.Col];

            if (player == state.CurrentPlayer)
            {
                int stability = 1;

                
                if (position.R == 0 || position.R == GameState.Rows - 1 ||
                    position.Col == 0 || position.Col == GameState.Cols - 1)
                {
                    stability++;
                }

                
                if (stablePositions.Contains(position))
                {
                    stability += 3;
                }

                stability_Score += stability;
            }
        }

        return stability_Score;
    }

    // Optional
    private int Evaluate_Pattern_Recognition(GameState state)
    {
        int patternScore = 0;

        // Define patterns and their corresponding scores
        Dictionary<string, int> patterns = new Dictionary<string, int>
        {
            { "X.X", 5 },   // Pattern with two AI player's disks and an empty space in between
            { "XX.", 10 },  // Pattern with two AI player's disks and an empty space at the end
            { ".XX", 10 },  // Pattern with two AI player's disks and an empty space at the beginning
            { "XXX", 20 },  // Pattern with three AI player's disks in a row
            { "XXXX", 30 }, // Pattern with four AI player's disks in a row
        };

        // Adjust pattern scores for capturing corners
        if (state.Board[0, 0] == state.CurrentPlayer)
            patternScore += 100;  // Capture top-left corner
        if (state.Board[0, GameState.Cols - 1] == state.CurrentPlayer)
            patternScore += 100;  // Capture top-right corner
        if (state.Board[GameState.Rows - 1, 0] == state.CurrentPlayer)
            patternScore += 100;  // Capture bottom-left corner
        if (state.Board[GameState.Rows - 1, GameState.Cols - 1] == state.CurrentPlayer)
            patternScore += 100;  // Capture bottom-right corner

        // Check horizontal patterns
        for (int row = 0; row < GameState.Rows; row++)
        {
            string rowString = "";
            for (int col = 0; col < GameState.Cols; col++)
            {
                rowString += state.Board[row, col] == state.CurrentPlayer ? "X" : ".";
            }
            patternScore += GetPatternScore(rowString, patterns);
        }

        // Check vertical patterns
        for (int col = 0; col < GameState.Cols; col++)
        {
            string colString = "";
            for (int row = 0; row < GameState.Rows; row++)
            {
                colString += state.Board[row, col] == state.CurrentPlayer ? "X" : ".";
            }
            patternScore += GetPatternScore(colString, patterns);
        }

        // Check diagonal patterns (top-left to bottom-right)
        for (int startRow = 0; startRow < GameState.Rows; startRow++)
        {
            string diagonalString = "";
            int row = startRow;
            int col = 0;
            while (row < GameState.Rows && col < GameState.Cols)
            {
                diagonalString += state.Board[row, col] == state.CurrentPlayer ? "X" : ".";
                row++;
                col++;
            }
            patternScore += GetPatternScore(diagonalString, patterns);
        }

        // Check diagonal patterns (top-right to bottom-left)
        for (int startRow = 0; startRow < GameState.Rows; startRow++)
        {
            string diagonalString = "";
            int row = startRow;
            int col = GameState.Cols - 1;
            while (row < GameState.Rows && col >= 0)
            {
                diagonalString += state.Board[row, col] == state.CurrentPlayer ? "X" : ".";
                row++;
                col--;
            }
            patternScore += GetPatternScore(diagonalString, patterns);
        }

        return patternScore;
    }

    private int GetPatternScore(string pattern, Dictionary<string, int> patterns)
    {
        int score = 0;
        foreach (KeyValuePair<string, int> kvp in patterns)
        {
            if (pattern.Contains(kvp.Key))
            {
                score += kvp.Value;
            }
        }
        return score;
    }

}
