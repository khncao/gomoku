using UnityEngine;
using UnityEngine.UI;

namespace knco.Gomoku {

public class UI : MonoBehaviour
{
    public Text gameStatusText;
    public Text logText;
    public Image playerColorImg;
    public GameObject optionsWindow;
    public GameObject computingWindow;
    public GameObject gameCompleteWindow;
    public Text gameCompleteText;
    public GameObject lastMoveMarker;
    public Text depthSliderValText;

    public void Init() {
        ToggleOptionsWindow(true);
        OnDepthSliderChange(Game.Instance.maxComputeDepth);
    }
    
    public void SetGameStatusText(string newStatusText, Color playerColor) {
        gameStatusText.text = newStatusText;
        gameStatusText.color = playerColor;
    }
    public void SetLogText(string text) {
        logText.text = text;
    }

    public void ToggleOptionsWindow(bool enabled) {
        optionsWindow.SetActive(enabled);
    }

    public void ToggleComputingWindow(bool enabled) {
        computingWindow.SetActive(enabled);
    }

    public void SetPlayerColorImage(Color color) {
        playerColorImg.color = color;
    }

    public void ShowGameComplete(string text) {
        gameCompleteText.text = text;
        gameCompleteWindow.SetActive(true);
    }

    public void SetLastMoveMarker(Tile tile) {
        if(tile == null) {
            lastMoveMarker.SetActive(false);
            return;
        }
        lastMoveMarker.SetActive(true);
        lastMoveMarker.transform.SetParent(tile.transform, false);
    }

    public void OnDepthSliderChange(System.Single value) {
        Game.Instance.OnDepthSliderChange((int)value);
        depthSliderValText.text = "Max depth: " + ((int)value).ToString();
    }
}
}