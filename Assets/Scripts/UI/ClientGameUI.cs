using Unity;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientGameUI : NetworkBehaviour
{
    [SerializeField] private UIDocument _gameUI;
    [SerializeField] private VisualTreeAsset _orderListItemTemplate;
    [SerializeField] private VisualTreeAsset _recipeImageListItemTemplate;
    [SerializeField] private ClientOrderManager _orderManager;
    [SerializeField] private ClientScoreManager _scoreManager;
    [SerializeField] private string _scoreLabelName;
    [SerializeField] private string _timeLabelName;
    [SerializeField] private string _orderListName;
    [SerializeField] private string _dishImageName;
    [SerializeField] private string _recipeImageListName;
    
    private Label _scoreText;
    private Label _timerText;
    private ScrollView _orderList;

    void Start(){
        this._scoreText = this._gameUI.rootVisualElement.Q<Label>(this._scoreLabelName);
        this._timerText = this._gameUI.rootVisualElement.Q<Label>(this._timeLabelName);
        this._orderList = this._gameUI.rootVisualElement.Q<ScrollView>(this._orderListName);

		this._scoreManager.OnScoreChange += this.OnScoreChange;
        this._orderManager.OnNewOrder += this.OnNewOrder;

        this._timerText.text = ((int)(ServerGameTimer.CurrentGameTime / 60)).ToString() + ": " + ((int)(ServerGameTimer.CurrentGameTime % 60)).ToString();
    }

    void FixedUpdate(){
        this._timerText.text = ((int)(ServerGameTimer.CurrentGameTime / 60)).ToString() + ": " + ((int)(ServerGameTimer.CurrentGameTime % 60)).ToString();
    }

    void OnScoreChange(object sender, ScoreChangeEventArgs args) {
        this._scoreText.text = "Score: " + args.CurrentScore.ToString();
    }

    void OnNewOrder(object sender, OrderListChangeEventArgs args){
        print("OnNewOrder: ClientGameGUI");

        Sprite dishImage = args.RequestedDish.DisplaySprite;

        TemplateContainer newOrderItemVisual = this._orderListItemTemplate.CloneTree();
        VisualElement dishImageHolder = newOrderItemVisual.Q<VisualElement>(this._dishImageName);
        VisualElement recipeImageList = newOrderItemVisual.Q<ScrollView>(this._recipeImageListName);

		dishImageHolder.style.backgroundImage = new StyleBackground(dishImage);

        foreach (ICombinableSO requiredCombinable in args.RequestedDish.RequiredCombinables){
            TemplateContainer newRecipeImageItemVisual = this._recipeImageListItemTemplate.CloneTree();

            newRecipeImageItemVisual.style.backgroundImage = new StyleBackground(requiredCombinable.DisplaySprite);
            recipeImageList.Add(newRecipeImageItemVisual);
        }

		this._orderList.Add(newOrderItemVisual);
    }

}
