using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace knco.Gomoku {
public class Tests : MonoBehaviour
{
    public bool TestEvaluatorLineAndPattern() {
        bool pass = false;

        // array of tests for sequence and pattern checking
        // sequentially add measured moves and check against actual measures
        return pass;
    }
    public bool TestStaticEvaluation() {
        bool pass = false;

        // array of tests for static evaluator score
        // sequentially add precalculated moves and check against actual score
        return pass;
    }
    public bool TestMinimaxAB() {
        bool pass = false;
        List<Tile> adj = new List<Tile>();
        // adj.Add();
        // Game.Instance.gomoku.MinimaxAB()
        return pass;
    }
}
}