using System.Collections.Generic;

namespace knco.Gomoku {
public class Evaluation
{
    // TODO: named classifications with cache/hashtable for getting scores
    // TODO: classify based on known sequences, ie. -****- live four

    // threats/score descending
    // ***** win
    // -****- 2 open four
    // multi threats 
    // ****- 1 open four
    // ***-* // **-** four
    // --***-- // -***-- 2 open three
    // -**-*- 2open 1gap three
    public static int StaticEvaluation(Tile[][] board, Tile move) {
        Queue<Tile> evalTiles = new Queue<Tile>();
        int score = EvaluatePlayerMove(board, move, move.player, evalTiles) +
                    EvaluatePlayerMove(board, move, move.player.opp, evalTiles);

        return move.player == Game.Black ? score : -score;
    }
    
    static int EvaluatePlayerMove(Tile[][] board, Tile move, Player player, Queue<Tile> evalTiles) {
        int score = EvaluateLine(board, move, player, 0, 1, evalTiles) + //hori
                EvaluateLine(board, move, player, 1, 0, evalTiles) + //vert
                EvaluateLine(board, move, player, 1, 1, evalTiles) + //diag-fall topleft->botright
                EvaluateLine(board, move, player, -1, 1, evalTiles); //diag-rise botleft->topright
        if(player != Game.CurrentTurn)
            score /= 4;
        return score;
    }

    static int CalculateTileScore(int consec, int openEnds, int openSpaces, int gaps) {
        int score = 0;
        if(consec >= 5)
            score = 100000;

        else if((consec + openSpaces) < 5) // |--**| |****|
            return 0;
        
        if(consec == 4) {
            if(openEnds == 2) // -****-
                score = 10000;
            else if(openEnds == 1) // ****-
                score = 1000;
        }
        else if(consec == 3) {
            if(openEnds == 2) { // -***-
                if(openSpaces > 2) // --***- --***--
                    score = 1000;
                else
                    score = 10; // -***-
            }
        }
        if(gaps > 0)
            score /= 2;
        return score;
    }

    static int EvaluateTiles(Queue<Tile> tiles, int leftCount, Tile move, Player player) {
        int tempSpaces = 0;
        int consec = 1, openSpaces = 0, openEnds = 0, gaps = 0;
        int initCount = tiles.Count;
        bool endReached = false;

        while(tiles.Count > 0) {
            var tile = tiles.Dequeue();

            if(tile.playerId == player.id) {
                if(tempSpaces == 1) 
                    gaps++;
                if(!endReached)
                    consec++;
                tempSpaces = 0;
            }
            else if(!tile.occupied) {
                tempSpaces++;
                if(tempSpaces > 1)
                    endReached = true;
            }
            
            if(tiles.Count == initCount - leftCount || tiles.Count == 0) {
                openSpaces += tempSpaces;
                if(tempSpaces > 0)
                    openEnds++;
                tempSpaces = 0;
                endReached = false;
            }
        }
        int score = CalculateTileScore(consec, openEnds, openSpaces, gaps);
        // if(!Game.CurrentTurn.isComputer)
        //     UnityEngine.Debug.Log(string.Format("{0}: {1}consec, {2}openSpaces, {3}openEnds, {4}gaps", score, consec, openSpaces, openEnds, gaps));
        return score;
    }

    // count queue of linear sequenced tiles, order (* is move input): 321*456
    // queue contains empty tiles and self moves--opponent moves/board boundary
    static int EvaluateLine(Tile[][] board, Tile move, Player player, int slopeX, int slopeY, Queue<Tile> evalTiles) {

        int leftCount = 0;

        for(int i = 1; i < board.Length; ++i) 
        {
            int y = move.y - (i * slopeY), x = move.x - (i * slopeX);
            if(y < 0 || x < 0 || y == board.Length || x == board.Length)
                break;
            if(board[y][x].occupied && board[y][x].playerId != player.id)
                break;
            evalTiles.Enqueue(board[y][x]);
        }
        leftCount = evalTiles.Count;

        for(int i = 1; i < board.Length; ++i)
        {
            int y = move.y + (i * slopeY), x = move.x + (i * slopeX);
            if(y < 0 || x < 0 || y == board.Length || x == board.Length)
                break;
            if(board[y][x].occupied && board[y][x].playerId != player.id)
                break;
            evalTiles.Enqueue(board[y][x]);
        }
        // if(!Game.currTurnPlayer.isComputer)
        //     UnityEngine.Debug.Log(string.Format("{0} left, {1} total", leftCount, evalTiles.Count));
        return EvaluateTiles(evalTiles, leftCount, move, player);
    }

}
}