using StarterAssets;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Properties
    public event Action OnGameOver;
    public GodState ActualGodState;
    public Player Player1;
    public TimeManager GameTimeManager;
    public UIController GameUIController;
    public AudioController GameAudioController;
    public float ActualGodPoints
    {
        get
        {
            return (_actualGodPoints);
        }
        private set
        {
            if (value>0)
            {
                _actualGodPoints = value;
            }
            else
            {
                _actualGodPoints = 0;
                //No puede bajar de 0
            }
        }
    }
    public enum GodState
    {
        Delighted,
        Satisfied,
        Neutral,
        Unsatisfied,
        Furious
    }
    #endregion

    #region Fields
    [SerializeField] private TerrainManager _terrainManager;
    [SerializeField] private HolyLava _holyLava;
    [Header("CAMERAS")]
    [SerializeField] private Camera _timelapseCamera;
    [SerializeField] private Camera _playerCamera;
    [Header("GOD POINTS")]
    [SerializeField] private float _actualGodPoints;
    private float _defaultGodPointsGoal;
    private float _goalPercentage;
    [SerializeField] private float _actualGodPointsGoal;
    [SerializeField] private float[] _godPointsGoals;
    [Header("REWARDS")]
    [SerializeField] private GameObject[] _spawnerDayRewards;
    [SerializeField] private ItemSpawner _rewardSpawner;


    #endregion

    #region Unity Callbacks
    void Start()
    {
        _actualGodPointsGoal = _godPointsGoals[GameTimeManager.Day];
        ResetGodPoints();
        GameTimeManager.OnNewDay += NewDay;
        Player1.OnDeath += PlayerDied;
        ActualGodState = GodState.Neutral;
        _rewardSpawner.Spawn(_spawnerDayRewards[0]);
        GameUIController.ShowCrafterText("Bienvenido! Esta mesa sirve para crear mejores objetos, prueba a meter este palo y una roca");
    }
    void Update()
    {

    }
    #endregion

    #region Public Methods
    public void AddGodPoints(float points)
    {
        ActualGodPoints += points;
        UpdateGoalPercentage();
    }
    public void RemoveGodPoints(float points)
    {
        ActualGodPoints -= points;
        UpdateGoalPercentage();
    }
    public void StartTimelapse()
    {
        GameTimeManager.Timelapse = true;
        _playerCamera.gameObject.SetActive(false);
        _timelapseCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void StopTimelapse()
    {
        GameTimeManager.Timelapse = false;
        _timelapseCamera.gameObject.SetActive(false);   
        _playerCamera.gameObject.SetActive(true);
        Player1.gameObject.SetActive(true);
        Player1.ResetPlayer();
        GameUIController.HideInGameMenu();
        GameTimeManager.TimeOn = true;
    }
    public void UpdateGodState()
    {
        if (_goalPercentage >= 0.77)
        {
            Debug.Log("Los dioses están encantados!!");
            ActualGodState = GodState.Delighted;

        }
        else if (_goalPercentage >= 0.6f)
        {
            Debug.Log("Los dioses están contentos");
            ActualGodState = GodState.Satisfied;
        }
        else if (_goalPercentage >= 0.4f)
        {
            Debug.Log("Los dioses esperaban algo más...");
            ActualGodState = GodState.Neutral;

        }
        else if (_goalPercentage >= 0.23f)
        {
            Debug.Log("Los dioses están insadisfechos");
            ActualGodState = GodState.Unsatisfied;
        }
        else
        {
            Debug.Log("Los dioses están furiosos");
            ActualGodState = GodState.Furious;
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Private Methods
    private void ResetGodPoints()
    {
        ActualGodPoints = 0;
        UpdateGoalPercentage();
    }
    private void PlayerDied()
    {
        Debug.Log("El jugador ha muerto, se saltará hasta el próximo día");
        if (ActualGodState == GodState.Furious)
        {
            EndGame();
        }
        else
        {
            GameUIController.DeathText.SetActive(true);
            StartTimelapse();
        }
    } 
    private void UpdateGoalPercentage()
    {
        _goalPercentage = ActualGodPoints / _actualGodPointsGoal;
        GameUIController.UpdateGodPointsSlider(_goalPercentage);
    }
    private void EndGame()
    {
        OnGameOver?.Invoke();//TODO: Hacer que todos los sistemas se paren
    }
    private void NewDay(int day)
    {
        if (GameTimeManager.Day > _godPointsGoals.Length)
            _actualGodPointsGoal = _defaultGodPointsGoal;
        else
            _actualGodPointsGoal = _godPointsGoals[GameTimeManager.Day];

        if (GameTimeManager.Timelapse)
        {
            GameTimeManager.TimeOn = false;
            GameUIController.DeathText.SetActive(false);
            GameUIController.ShowInGameMenu();
        }
        if (_spawnerDayRewards[day] != null && ActualGodState >= GodState.Neutral)
        {
            _rewardSpawner.Spawn(_spawnerDayRewards[day]);
            GameUIController.ShowCrafterText("Recompensa! Has obtenido " + _spawnerDayRewards[day].name);
        }
        ResetGodPoints();
    }
    #endregion

}
