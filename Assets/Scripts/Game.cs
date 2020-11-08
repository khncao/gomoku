using System.Collections.Generic;
using UnityEngine;

namespace knco.Gomoku {
// for move history, for undo move/state manage
public struct PlayerMove {
    public Player player;
    public Tile moveTile;
    public int turnNumber;
}

public class Game : MonoBehaviour
{
    public static Game Instance;
    public static Player Black, White, CurrentTurn;
    public UI ui;
    public GridBoard gridBoard;
    public GameObject playerPrefab;
    public Minimax gomoku;
    public int maxComputeDepth = 6;
    public int timeoutMs = 20000;
    
    Stack<PlayerMove> playerMoves = new Stack<PlayerMove>();
    bool playerGoesFirst = false;
    bool computerCalculating = false;
    int currentTurnNum;
    Tile hoveredTile;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance) {
            Debug.LogError("Another game instance exists");
            Destroy(this);
            return;
        }
        Instance = this;
        gomoku = new Minimax();

        Black = Instantiate(playerPrefab).GetComponent<Player>();
        White = Instantiate(playerPrefab).GetComponent<Player>();
        Black.Init(Color.black, 1, "Black");
        White.Init(Color.white, 2, "White");
        ui.Init();
        gridBoard.InitializeMatrix();
        TogglePlayerColor();
        // SetTwoPlayer();
    }
    // also used on UI event to start game
    public void InitializeGame() {
        currentTurnNum = 1;
        gridBoard.ResetBoardTiles();
        gomoku.InitializeAdjacent(GridBoard.Board, gomoku.adjTiles);
        CurrentTurn = Black;
        ui.SetLastMoveMarker(null);
        playerMoves.Clear();
        CheckIfComputerTurn();
        UpdateGameStatus();
    }
    // used for UI event; display transparent piece on hovered tile(cosmetic)
    public void SetHoverTile(Tile tile) {
        if(hoveredTile != null)
            hoveredTile.HoverStateToggle(CurrentTurn, false);
        hoveredTile = tile;
        if(hoveredTile)
            hoveredTile.HoverStateToggle(CurrentTurn, true);
    }
    // used by UI to choose human player turn
    public void TogglePlayerColor() {
        playerGoesFirst = !playerGoesFirst;

        if(playerGoesFirst) {
            Black.SetHuman();
            White.SetComputer();
            ui.SetPlayerColorImage(Color.black);
        }
        else {
            Black.SetComputer();
            White.SetHuman();
            ui.SetPlayerColorImage(Color.white);
        }
    }
    public void ToggleTwoPlayer(bool enabled) {
        if(enabled) {
            SetTwoPlayer();
            ui.playerColorImg.transform.parent.gameObject.SetActive(false);
        }
        else {
            TogglePlayerColor();
            ui.playerColorImg.transform.parent.gameObject.SetActive(true);
        }
    }
    public void SetTwoPlayer() {
        Black.SetHuman();
        White.SetHuman();
    }
    public void OnDepthSliderChange(int value) {
        maxComputeDepth = value;
    }
    // used by UI to undo last 2 moves; blocks on computer calculation
    public void UndoMove() {
        if(playerMoves.Count < 2 || computerCalculating) 
            return;

        RevertPlayerMove(playerMoves.Pop());
        if(CurrentTurn.isComputer)
            RevertPlayerMove(playerMoves.Pop());
        gomoku.InitializeAdjacent(GridBoard.Board, gomoku.adjTiles);
        ui.SetLastMoveMarker(playerMoves.Peek().moveTile);
    }
    public void PlayRandomMove() {
        Tile[] emptyTiles = gridBoard.GetEmptyTiles();
        Tile rand = emptyTiles[Random.Range(0, emptyTiles.Length)];
        
        PlayPiece(GridBoard.Board[rand.y][rand.x]);
    }
    // called by tile on UI event(click)
    public void PlayerMove(Tile tile) {
        if(tile.occupied) {
            Debug.Log("Tile occupied");
            return;
        }
        if(CurrentTurn.isComputer) {
            Debug.Log("Computer turn");
            return;
        }

        PlayPiece(tile);
    }
    // Check if piece is victory move, or fills the board creating a tie
    // calls ui function to display result panel with given text
    public bool CheckResult(int val) {
        if(Mathf.Abs(val) >= 100000) {
            ui.ShowGameComplete(CurrentTurn.playerName + " wins");
            return true;
        }
        else if(gridBoard.playedPiecesCount >= gridBoard.totalTiles) {
            ui.ShowGameComplete("Tie Game");
            return true;
        }
        return false;
    }

    // called by function that determines if currentTurnPlayer is computer
    // if computer goes first, plays random tile near center of board
    bool ComputerMove() {
        // PlayRandomMove();

        if(currentTurnNum == 1) 
            PlayPiece(GridBoard.Board[Random.Range(6, 8)][Random.Range(6, 8)]);
        else
            PlayPiece(gomoku.GetBestMove(CurrentTurn));

        ui.ToggleComputingWindow(false);
        computerCalculating = false;

        return true;
    }

    // used by both human/ai players to occupy tile, add PlayerMove, 
    // increment counters, check win/tie state, change player turn
    void PlayPiece(Tile tile) {
        tile.EnableAndSetTilePiece(CurrentTurn);
        ui.SetLastMoveMarker(tile);

        // Move data struct for state change management
        var move = new PlayerMove {
            player = CurrentTurn,
            moveTile = tile,
            turnNumber = currentTurnNum,
        };
        playerMoves.Push(move);
        
        // increment played pieces and turn number
        gridBoard.playedPiecesCount++;
        currentTurnNum++;

        // checks for wins/tie; return if win/tie detected
        var score = Evaluation.StaticEvaluation(GridBoard.Board, tile);
        Debug.Log(string.Format("{0}: [{1}, {2}]; {3}", CurrentTurn.playerName, tile.x, tile.y, score));
        if(CheckResult(score))
            return;
        
        gomoku.AddAdjacentTiles(GridBoard.Board, tile, gomoku.adjTiles);

        // Go to next player's turn
        CurrentTurn = CurrentTurn == Black ? White : Black;

        UpdateGameStatus();
        CheckIfComputerTurn();
    }
    // used by UndoMove to revert game state by one move
    void RevertPlayerMove(PlayerMove playerMove) {
        playerMove.moveTile.ResetTileState();
        CurrentTurn = playerMove.player == Black ? Black : White;

        gridBoard.playedPiecesCount--;
        currentTurnNum--;

        UpdateGameStatus();
    }
    // check if currentPlayerTurn is computer; call ComputerMove to play a move
    void CheckIfComputerTurn() {
        if(CurrentTurn.isComputer) {
            computerCalculating = true;
            ui.ToggleComputingWindow(true);
            Invoke("ComputerMove", 0.05f); // unity SetActive workaround
        }
    }
    
    // UI update
    public void UpdateGameStatus() {
        string gameStatusText = CurrentTurn.playerName;
        gameStatusText += " turn";
        gameStatusText += "\nTurn: " + currentTurnNum.ToString();
        ui.SetGameStatusText(gameStatusText, CurrentTurn.playerColor);
    }

    public void UpdateLog(float ms, int adjTiles, int maxEvalAdj, int evalCount, int score) {
        string logText = "Last computer move\n";

        logText += string.Format("{0}ms\n{1} possible moves\n{2} max eval adj\n{3} evals\n{4} score", ms, adjTiles, maxEvalAdj, evalCount, score);

        ui.SetLogText(logText);
    }
}
}