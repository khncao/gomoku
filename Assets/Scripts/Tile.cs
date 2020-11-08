using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace knco.Gomoku {
public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Image piece;
    public int x, y;
    public int playerId = 0;
    public Player player { 
        get { return _player; } 
        set { _player = value; 
        playerId = value ? value.id : 0; 
    }}
    public bool occupied { get { return playerId > 0; }}
    Player _player;

    public void EnableAndSetTilePiece(Player player) {
        this.player = player;
        // this.playerId = player.id;

        piece.gameObject.SetActive(true);
        piece.color = playerId == 1 ? Color.black : Color.white;
    }
    public void ResetTileState() {
        playerId = 0;

        piece.gameObject.SetActive(false);
    }
    
    public void HoverStateToggle(Player player, bool hovered) {
        if(occupied) return;
        if(hovered) {
            piece.gameObject.SetActive(true);
            var c = player.playerColor;
            c.a = 0.5f;
            piece.color = c;
        }
        else {
            piece.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Game.Instance.SetHoverTile(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Game.Instance.SetHoverTile(null);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Game.Instance.PlayerMove(this);
    }
}
}