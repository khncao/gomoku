using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace knco.Gomoku {
// minimax algorithm with alpha beta pruning for gomoku ai
public class Minimax {
    public List<Tile> adjTiles = new List<Tile>();
    public int evalCount = 0, maxEvalAdj = 0;
    Stopwatch stopwatch;
    int maxCount = 0, minCount = 0, alphaBreak = 0, betaBreak = 0;

    // for managing adj tile game state on new game, undo, play piece
    public void AddAdjacentTiles(Tile[][] board, Tile tile, List<Tile> adj) {
        for(int r = tile.y - 2; r <= tile.y + 2; ++r) {
            for(int c = tile.x - 2; c <= tile.x + 2; ++c) 
            {
                if(r < 0 || c < 0 || r >= board.Length || c >= board.Length)
                    continue;
                if(!adj.Contains(board[r][c]) && !board[r][c].occupied) {
                    adj.Add(board[r][c]);
                }
            }
        }
    }
    public void InitializeAdjacent(Tile[][] board, List<Tile> adj) {
        adj.Clear();
        for(int r = 0; r < board.Length; ++r) {
            for(int c = 0; c < board.Length; ++c) 
            {
                if(!board[r][c].occupied)
                    continue;
                AddAdjacentTiles(board, board[r][c], adj);
            }
        }
    }

// calls minimaxab, iterates possible moves, returns best move for player input
    public Tile GetBestMove(Player player) {
        Tile bestMove = null;
        int bestMoveScore = player == Game.Black ? int.MinValue : int.MaxValue;
        stopwatch = Stopwatch.StartNew();

        for(int i = 0; i < adjTiles.Count; ++i) {
            if(adjTiles[i].occupied)
                continue;

            adjTiles[i].player = player;
            
            List<Tile> adj = new List<Tile>(adjTiles);
            AddAdjacentTiles(GridBoard.Board, adj[i], adj);

            int score = MinimaxAB(adj, adjTiles[i], player.opp, 1, int.MinValue, int.MaxValue);

            adjTiles[i].player = null;

            if((player == Game.Black && score > bestMoveScore) || 
            (player == Game.White && score < bestMoveScore)) {
                bestMoveScore = score;
                bestMove = adjTiles[i];
            }
        }
        stopwatch.Stop();
        Game.Instance.UpdateLog(stopwatch.ElapsedMilliseconds, adjTiles.Count, maxEvalAdj, evalCount, bestMoveScore);
        // UnityEngine.Debug.Log(string.Format("{0} max, {1} min, {2} alpha, {3} beta", maxCount, minCount, alphaBreak, betaBreak));
        evalCount = minCount = maxCount = alphaBreak = betaBreak = 0;
        
        return bestMove;
    }
    
    public int MinimaxAB(List<Tile> adj, Tile move, Player player, int depth, int alpha, int beta) {
        evalCount++;
        int score = Evaluation.StaticEvaluation(GridBoard.Board, move); // check for wins
        if(Math.Abs(score) >= 100000 || 
            depth >= Game.Instance.maxComputeDepth || 
            stopwatch.ElapsedMilliseconds > Game.Instance.timeoutMs) {
            return score;
        }
        // if(adj.Count > maxEvalAdj)
        //     maxEvalAdj = adj.Count;

        if(player == Game.Black) { // maximize
            // maxCount++;
            int max = int.MinValue;

            for(int i = 0; i < adj.Count; ++i) {
                if(adj[i].occupied)
                    continue;

                adj[i].player = player;
                max = Math.Max(max, MinimaxAB(adj, adj[i], player.opp, depth+1, alpha, beta));
                
                adj[i].player = null;

                alpha = Math.Max(alpha, max);
                if(alpha >= beta) {
                    // alphaBreak++;
                    break;
                }
            }
            return max;
        }
        else { // minimize
            // minCount++;
            int min = int.MaxValue;

            for(int i = 0; i < adj.Count; ++i) {
                if(adj[i].occupied)
                    continue;

                adj[i].player = player;
                min = Math.Min(min, MinimaxAB(adj, adj[i], player.opp, depth+1, alpha, beta));

                adj[i].player = null;

                beta = Math.Min(beta, min);
                if(beta <= alpha) {
                    // betaBreak++;
                    break;
                }
            }
            return min;
        }
    }
}
}