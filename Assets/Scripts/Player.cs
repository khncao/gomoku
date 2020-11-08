using UnityEngine;

namespace knco.Gomoku {
public class Player : MonoBehaviour
{
    public string playerName;
    public bool isComputer;
    public Color playerColor;
    public Player opp;
    public int id;

    public void Init(Color color, int id, string name) {
        this.playerColor = color;
        this.id = id;
        this.playerName = name;
        this.opp = id == 1 ? Game.White : Game.Black;
    }

    public void SetHuman() {
        // playerName = "Player " + id.ToString();
        isComputer = false;
    }
    public void SetComputer() {
        // playerName = "Computer";
        isComputer = true;
    }
}
}