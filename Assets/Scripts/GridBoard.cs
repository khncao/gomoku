using UnityEngine;
using System.Collections.Generic;

namespace knco.Gomoku {
// [ExecuteInEditMode]
public class GridBoard : MonoBehaviour
{
    public static Tile[][] Board;
    public Tile[] tiles;
    public int boardDimension = 15;
    
    public int playedPiecesCount;
    public int totalTiles { get { return boardDimension * boardDimension; }}

    public void InitializeMatrix() {
        if(tiles.Length < 1) 
            tiles = GetComponentsInChildren<Tile>();

        int tileCounter = 0;
        Board = new Tile[boardDimension][];

        for(int r = 0; r < Board.Length; ++r) {
            Board[r] = new Tile[boardDimension];

            for(int c = 0; c < Board[r].Length; ++c) {
                Board[r][c] = tiles[tileCounter];
                Board[r][c].x = c;
                Board[r][c].y = r;
                tileCounter++;
            }
        }
    }

    public void ResetBoardTiles() {
        playedPiecesCount = 0;
        for(int i = 0; i < tiles.Length; ++i) {
            tiles[i].ResetTileState();
        }
    }

    public Tile[] GetEmptyTiles() {
        return System.Array.FindAll(tiles, x=>x.playerId == 0);
    }
    
    // private void Start() {
    //     tiles = GetComponentsInChildren<Tile>();
    // }
}
}