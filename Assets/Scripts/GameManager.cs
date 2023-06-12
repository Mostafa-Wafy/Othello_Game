using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private HardAIPlayer hardAiPlayer;
    private DummyAIPlayer dummyAiPlayer;

    private GameState gameState = new GameState();
    private CoinScript[,] Coins = new CoinScript[8,8];

    [SerializeField]
    private UIManager uiManager;
    
    [SerializeField]
    private CoinScript CoinBlackUp;

    [SerializeField]
    private CoinScript CoinWhiteUp;

    private Dictionary<Player, CoinScript> CoinPrefabs = new Dictionary<Player, CoinScript>();


    private bool Can_Move = true;
    private List<GameObject> Highlights = new List<GameObject>();



    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask boardLay;

    [SerializeField]
    private GameObject hlPrefab;



    
    private void Start()
    {

        CoinPrefabs[Player.Black] = CoinBlackUp;
        CoinPrefabs[Player.White] = CoinWhiteUp;

        StartingCoins();
        GameMode();
        ShowAvailableMoves();
        uiManager.SetPlayerText(gameState.CurrentPlayer);
        
    }

    
    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape)){

            Application.Quit();
        
        }

        if(Input.GetMouseButtonDown(0)){

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hitInfo)){

                Vector3 impact = hitInfo.point;
                Position boardPos = SceneToBoardPosition(impact);
                OnBoardClick(boardPos);
            }
        }
    }

    public void GameMode () {

        
        if(DropDownMenu.gameMode == 1){
            dummyAiPlayer = new DummyAIPlayer(Player.White);
        }
        else if(DropDownMenu.gameMode == 2){
            hardAiPlayer = new HardAIPlayer(Player.White);
        }
        
    }

    private void ShowAvailableMoves(){

        foreach(Position boardPos in gameState.LegalMoves.Keys){
            Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.01f;
            GameObject Highlight = Instantiate(hlPrefab, scenePos, Quaternion.identity);
            Highlights.Add(Highlight);
        }
    }

    private void HideAvailableMoves(){

        Highlights.ForEach(Destroy);
        Highlights.Clear();
    }

    private void OnBoardClick (Position boardPos) {

        if(!Can_Move){
            return;
        }

        if (gameState.MakeaMove(boardPos, out MovementInformation moveInformation)){
            
            StartCoroutine(OnMoveMade(moveInformation));
        }    
    }

    private IEnumerator OnMoveMade(MovementInformation moveInformation){

        Can_Move = false;
        HideAvailableMoves();
        yield return ShowMove(moveInformation);
        yield return TurnOutCome(moveInformation);

        if (hardAiPlayer != null && gameState.CurrentPlayer == hardAiPlayer.playerType)
        {
            // AI player's turn
            Position aiMove = hardAiPlayer.MakeaMove(gameState);
            yield return new WaitForSeconds(1f); // Delay for visual effect

            if (gameState.MakeaMove(aiMove, out MovementInformation aiMoveInfo))
            {
                yield return OnMoveMade(aiMoveInfo);
            }
        }
        if (dummyAiPlayer != null && gameState.CurrentPlayer == dummyAiPlayer.playerType)
        {
            // AI player's turn
            Position aiMove = dummyAiPlayer.MakeaMove(gameState);
            yield return new WaitForSeconds(1f); // Delay for visual effect

            if (gameState.MakeaMove(aiMove, out MovementInformation aiMoveInfo))
            {
                yield return OnMoveMade(aiMoveInfo);
            }
        }
        ShowAvailableMoves();
        Can_Move = true;

    }

    private Position SceneToBoardPosition(Vector3 scenepos) {

        int col = (int)(scenepos.x - 0.25f);
        int row = 7 - (int)(scenepos.z - 0.25f);

        return new Position(row, col);
        
    }

    private Vector3 BoardToScenePos(Position boardPos) 
    {
        return new Vector3(boardPos.Col + 0.75f, 0, 7 - boardPos.R + 0.75f);

    }

    private void SpawnCoin( CoinScript prefab, Position boardPos) {

        Vector3 scenePos = BoardToScenePos(boardPos) + Vector3.up * 0.2f;
        Coins[boardPos.R, boardPos.Col] = Instantiate(prefab, scenePos, Quaternion.identity);
        
    }
    

    private void StartingCoins () {

        foreach(Position boardPos in gameState.TakenPositions()) 
        {

            Player player = gameState.Board[boardPos.R, boardPos.Col];
            SpawnCoin(CoinPrefabs[player], boardPos);
            
        }
        
    }

    private void FlipCoins(List<Position> positions){

        foreach(Position boardPos in positions){
            Coins[boardPos.R, boardPos.Col].Flip();
        }
    }

    private IEnumerator ShowMove(MovementInformation moveInformation){

        SpawnCoin(CoinPrefabs[moveInformation.Player], moveInformation.Position);
        yield return new WaitForSeconds(0.33f);
        FlipCoins(moveInformation.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator TurnSkipped(Player SkippedPlayer){

        uiManager.SetSkippedText(SkippedPlayer);
        yield return uiManager.AnimateUpperText();
    }

    public IEnumerator ShowGameOver(Player winner){

        uiManager.SetUpperText("Both Players Can't Move");
        yield return uiManager.AnimateUpperText();

        yield return uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);

        yield return ShowCount();

        uiManager.SetWinnerText(winner);
        yield return uiManager.ShowEndGameScreen();

    }

    private IEnumerator TurnOutCome(MovementInformation moveInformation){

        if(gameState.GameOver){
            yield return ShowGameOver(gameState.winner);
            yield break;
        }

        Player currentPlayer = gameState.CurrentPlayer;

        if(currentPlayer == moveInformation.Player){
            yield return TurnSkipped(currentPlayer.Opponent());
        }

        uiManager.SetPlayerText(currentPlayer);
    }

    private IEnumerator ShowCount(){

        int black = 0, white = 0;

        foreach(Position pos in gameState.TakenPositions()){

            Player player = gameState.Board[pos.R, pos.Col];

            if(player == Player.Black){
                black = black +1 ;
                uiManager.SetBlackScoreText(black);
            }

            if(player == Player.White){
                white = white + 1 ;
                uiManager.SetWhiteScoreText(white);
            }

            Coins[pos.R, pos.Col].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }


    public IEnumerator RestartGame(){

        yield return uiManager.HideEndGameScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void OnPlayAgianClicked(){

        StartCoroutine(RestartGame());

    }

    public void OnReturnButtonClicked () {
        SceneManager.LoadScene("Main Menu");
    }
    
}
