using System.Collections.Generic;

public class GameState
{

   
   public const int rows = 8;
   public const int columns = 8;

   
   public Player[,] Board { get; private set; }


   public Dictionary<Player, int> CoinCount {get; private set;}

   
   public Player CurrentPlayer {get; set;}
   

   
   public Dictionary<Position, List<Position>> AvailableMoves {get; private set;}

   
   public bool GameOver {get; private set;}

   public Player winner {get; private set;}


   public GameState()
   {
        Board = new Player[rows, columns];
        Board[3,4] = Player.Black;
        Board[4,3] = Player.Black;
        Board[3,3] = Player.White;
        Board[4,4] = Player.White;

        CoinCount = new Dictionary<Player, int>(){

            {Player.Black, 2},
            {Player.White, 2},
            {Player.None, 0}

        };

        CurrentPlayer = Player.Black;
        AvailableMoves = FindAvailableMoves(CurrentPlayer);
   }

   public bool MakeaMove(Position pos, out MovementInformation moveinfo)
   {
        if(!AvailableMoves.ContainsKey(pos))
        {
            moveinfo = null;
            return false;
        }

        Player movingPlayer = CurrentPlayer;
        List<Position> outflanked = AvailableMoves[pos];
        Board[pos.R, pos.Col] = movingPlayer;

        FlipCoins(outflanked);
        UpdateCoinCount(movingPlayer, outflanked.Count);
        CheckPassTurn();

        moveinfo = new MovementInformation { Player = movingPlayer, Position = pos, Outflanked = outflanked};
        return true;
   }

   public IEnumerable<Position> TakenPositions(){

        for(int r = 0; r < rows; r++) {

            for(int c = 0; c < columns ; c++) {

                if(Board[r,c] != Player.None){

                    yield return new Position(r, c); 

                }
            }  
        }
   } 

   private void FlipCoins (List<Position> positions){

        foreach (Position pos in positions)
        {
            Board[pos.R, pos.Col] = Board[pos.R, pos.Col].Opponent();
        }

   }

   private void UpdateCoinCount (Player movingPlayer, int outflankedCount) {

        CoinCount[movingPlayer] += outflankedCount + 1;
        CoinCount[movingPlayer.Opponent()] -= outflankedCount;
    
   }

   public void ChangePlayer () {

        CurrentPlayer = CurrentPlayer.Opponent();
        AvailableMoves = FindAvailableMoves(CurrentPlayer);

   }

   private Player FindWinner () {

        if(CoinCount[Player.Black] > CoinCount[Player.White]){
            return Player.Black;
        }

        if(CoinCount[Player.Black] < CoinCount[Player.White]){
            return Player.White;
        }

        return Player.None;
    
   }

   private void CheckPassTurn () {

        ChangePlayer();

        if(AvailableMoves.Count > 0){
            return;
        }

        ChangePlayer();

        if(AvailableMoves.Count == 0){
            CurrentPlayer = Player.None;
            GameOver = true;
            winner = FindWinner();
        }
 
   }

    private bool IsInsideBoard(int r, int c)
    {
        return r>= 0 && r < rows && c>= 0 && c < columns;
    }

   private List<Position> OutFlankedDirected(Position pos, Player plr, int rDelta, int cDelta) 
   {
        List<Position> outflanked = new List<Position>();
        int r = pos.R + rDelta;
        int c = pos.Col + cDelta;

        while (IsInsideBoard(r,c) && Board[r, c] != Player.None){

            if (Board[r, c] == plr.Opponent())
            {
                outflanked.Add(new Position(r, c));
                r += rDelta;
                c += cDelta;
            }
            else {return outflanked;}
        }

        return new List<Position>();
   }

    private List<Position> Outflanked(Position pos, Player plr){

        List<Position> outflanked = new List<Position>();

        for (int rDelta = -1; rDelta <= 1; rDelta++){

            for (int cDelta = -1; cDelta <= 1; cDelta++){

                if(rDelta == 0 && cDelta == 0)
                {
                    continue;
                }

                outflanked.AddRange(OutFlankedDirected(pos, plr, rDelta, cDelta));

            }
        }
        return outflanked;
   }

   private bool IsMoveLegal(Player plr, Position pos, out List<Position> outflanked)
   {

    if(Board[pos.R, pos.Col] != Player.None){
        outflanked = null;
        return false;
    }
    outflanked = Outflanked(pos, plr);
    return outflanked.Count > 0;
   }

   private Dictionary<Position, List<Position>> FindAvailableMoves(Player plr){

    Dictionary<Position, List<Position>> legalMoves = new Dictionary<Position, List<Position>>();

    for(int r = 0; r < rows; r++) {
        for(int c = 0; c < columns; c++) {

            Position pos = new Position(r,c);

            if(IsMoveLegal(plr, pos, out List<Position> outflanked)) {
                legalMoves[pos] = outflanked;
            }    
        } 
    }

    return legalMoves;
   }

    public GameState Clone()
    {
        GameState clonedState = new GameState();

        // Copy the board
        clonedState.Board = (Player[,])Board.Clone();

        // Copy the disk count
        clonedState.CoinCount = new Dictionary<Player, int>(CoinCount);

        // Copy the current player
        clonedState.CurrentPlayer = CurrentPlayer;

        // Copy the legal moves
        clonedState.AvailableMoves = new Dictionary<Position, List<Position>>();
        foreach (KeyValuePair<Position, List<Position>> kvp in AvailableMoves)
        {
            Position key = kvp.Key;
            List<Position> value = kvp.Value;
            clonedState.AvailableMoves.Add(key, new List<Position>(value));
        }

        // Copy the game over and winner status
        clonedState.GameOver = GameOver;
        clonedState.winner = winner;

        return clonedState;
    }

}
