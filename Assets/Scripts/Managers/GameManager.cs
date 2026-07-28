using Photon.Pun;
using StarterAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class GameManager : Singleton<GameManager>
{
    #region Properties
    public event Action OnGameOver;
    public GodState ActualGodState;
    public Player LocalPlayer;
    public List<Player> RemotePlayers = new();
    public TimeManager GameTimeManager;
    public TerrainManager TerrainManager;
    public NetworkController NetworkController;
    public UIController GameUIController;
    public AudioController GameAudioController;
    public Camera LocalPlayerCamera;
    public float ActualGodPoints
    {
        get
        {
            return (_actualGodPoints);
        }
        set
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
        Furious,
        Unsatisfied,
        Neutral,
        Satisfied,
        Delighted
    }
    #endregion

    #region Fields
    [SerializeField] private HolyLava _holyLava;
    [SerializeField] private int _localPlayerViewID;
    [Header("CAMERAS")]
    [SerializeField] private Camera _timelapseCamera;
    [Header("GOD POINTS")]
    [SerializeField] private float _actualGodPoints;
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
        LocalPlayerCamera = Camera.main;
        _actualGodPointsGoal = _godPointsGoals[GameTimeManager.Day];
        ResetGodPoints();
        GameTimeManager.OnNewDay += NewDay;
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
        NetworkController.UpdatePoints(ActualGodPoints);
    }
    public void RemoveGodPoints(float points)
    {
        ActualGodPoints -= points;
        NetworkController.UpdatePoints(ActualGodPoints);
    }
    public void UpdateGoalPercentage()
    {
        _goalPercentage = ActualGodPoints / _actualGodPointsGoal;
        GameUIController.UpdateGodPointsSlider(_goalPercentage);
    }
    public void StartTimelapse()
    {
        if(PhotonNetwork.IsMasterClient)
            GameTimeManager.Timelapse = true;
        //TODO: UI informativa
    }
    public void StopTimelapseAndContinue()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameTimeManager.Timelapse = false;
            GameTimeManager.TimeOn = true;
            NetworkController.RemoteContinuePlaying();
        }
        _timelapseCamera.gameObject.SetActive(false);   
        LocalPlayerCamera.gameObject.SetActive(true);
        LocalPlayer.gameObject.SetActive(true);
        LocalPlayer.ResetPlayer();
        GameUIController.HideInGameMenu();
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
    public void SetLocalPlayer()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.Local = true;
            _localPlayerViewID = LocalPlayer.GetComponent<PhotonView>().ViewID;
            LocalPlayer.PlayerCamera = GameManager.Instance.LocalPlayerCamera;
            LocalPlayer.OnDeath += LocalPlayerDied;
            NetworkController.AddRemotePlayer(_localPlayerViewID);
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
    private void LocalPlayerDied()
    {
        NetworkController.IsAliveUpdate(false, _localPlayerViewID);

        LocalPlayerCamera.gameObject.SetActive(false);
        _timelapseCamera.gameObject.SetActive(true);

        if(ActualGodState != GodState.Furious)
            GameUIController.DeathText.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkController.CheckPlayersAlive();
    } 
    public void CheckPlayersAlive()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int alive = 0;
            foreach (Player player in RemotePlayers)
            {
                if (player != null && player.Alive)
                {
                    alive++;
                }
            }
            if (alive <= 0 && !LocalPlayer.Alive)
            {
                if (ActualGodState == GodState.Furious)
                {
                    EndGame();
                }
                else
                {
                    StartTimelapse();
                }
            }
        }
    }
    private void EndGame()
    {
        OnGameOver?.Invoke();//TODO: Hacer que todos los sistemas se paren
    }
    private void NewDay(int day)
    {
        if (day >= _godPointsGoals.Length)
        {
            _actualGodPointsGoal = _godPointsGoals[_godPointsGoals.Length-1];
        }
        else
        {
            _actualGodPointsGoal = _godPointsGoals[GameTimeManager.Day];
        }

        if (GameTimeManager.Timelapse && PhotonNetwork.IsMasterClient)
        {
            GameTimeManager.TimeOn = false;
            GameUIController.DeathText.SetActive(false);
            GameUIController.ShowInGameMenu();
        }

        if (ActualGodState >= GodState.Neutral && day < _spawnerDayRewards.Length - 1)
        {
            if (_spawnerDayRewards[day]!=null)
            {
                _rewardSpawner.Spawn(_spawnerDayRewards[day]);
                GameUIController.ShowCrafterText("Recompensa! Has obtenido " + _spawnerDayRewards[day].name);
            }
        }
        ResetGodPoints();
    }
    #endregion

}
