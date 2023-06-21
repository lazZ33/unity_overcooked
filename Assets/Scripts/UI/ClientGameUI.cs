using Unity;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientGameUI : NetworkBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private ClientGameManager _gameManager;
    [SerializeField] private string _scoreLabelName;
    [SerializeField] private string _timeLabelName;
    [SerializeField] private string _orderListName;
    
    private Label _scoreText;
    private Label _timerText;
    private ScrollView _orderList;

    void Start(){
        this._scoreText = this._uiDocument.rootVisualElement.Q<Label>(this._scoreLabelName);
        this._timerText = this._uiDocument.rootVisualElement.Q<Label>(this._timeLabelName);
        this._orderList = this._uiDocument.rootVisualElement.Q<ScrollView>(this._orderListName);

        this._gameManager.OnScoreChange += this.OnScoreChange;

        this._timerText.text = ((int)(ServerGameManager.TimeLeft / 60)).ToString() + ": " + ((int)(ServerGameManager.TimeLeft % 60)).ToString();
    }

    void FixedUpdate(){
        this._timerText.text = ((int)(ServerGameManager.TimeLeft / 60)).ToString() + ": " + ((int)(ServerGameManager.TimeLeft % 60)).ToString();
    }

    void OnScoreChange(object sender, ScoreChangeEventArgs args) {
        this._scoreText.text = "Score: " + args.CurrentScore.ToString();
    }

}
