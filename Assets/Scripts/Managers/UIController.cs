using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Properties
    public GameObject DeathText;
    #endregion

    #region Fields
    [SerializeField] private Slider _godPointsSlider;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _interactionText;
    [SerializeField] private TextMeshProUGUI _crafterText;
    [Header("Panels")]
    [SerializeField] private GameObject _interface;
    [SerializeField] private GameObject _inGameMenu;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _connectingPanel;

    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        ShowConnectingPanel();
    }
    void Start()
    {
        HideInGameMenu();
        GameManager.Instance.GameTimeManager.OnNewDay += UpdateDay;
        GameManager.Instance.OnGameOver += ShowGameOverPanel;
    }
    void Update()
    {

    }
    #endregion

    #region Public Methods
    public void UpdateGodPointsSlider(float percentage)
    {
        _godPointsSlider.value = percentage;
    }
    public void UpdateHealthSlider(float health, float maxHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = health;
    }
    public void UpdateDay(int day)
    {
        _dayText.text = day.ToString();
    }
    public void ShowInGameMenu()
    {
        _interface.SetActive(false);
        _inGameMenu.SetActive(true);
        GameManager.Instance.GameAudioController.PlayMusic(false);
    }
    public void HideInGameMenu()
    {
        _inGameMenu.SetActive(false);
        _interface.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.GameAudioController.PlayMusic(true);
    }
    public void ShowInteractionText(string text)
    {
        _interactionText.text = text;
    }
    public void ShowCrafterText(string text)
    {
        _crafterText.text = text;
    }
    public void ShowConnectingPanel()
    {
        _connectingPanel.SetActive(true);
        _interface.SetActive(false);
        _inGameMenu.SetActive(false);
        _gameOverPanel.SetActive(false);

    }   
    public void HideConnectingPanel()
    {
        _connectingPanel.SetActive(false);
        _interface.SetActive(true);
    }
    #endregion

    #region Private Methods
    private void ShowGameOverPanel()
    {
        _interface.SetActive(false);
        _inGameMenu.SetActive(false);
        DeathText.SetActive(false);
        _gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion
}
