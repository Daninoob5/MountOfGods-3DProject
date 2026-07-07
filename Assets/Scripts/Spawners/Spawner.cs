using UnityEngine;
using static GameManager;

public class Spawner : MonoBehaviour
{
    #region Properties
    public float ActualSpawnsPerDay
    {
        get
        {
            return _actualSpawnsPerDay;
        }
        set
        {
            if (value >= _maxSpawnsPerDay)
            {
                _actualSpawnsPerDay = _maxSpawnsPerDay;
            }
            else if (value <= 0)
            {
                _actualSpawnsPerDay = 0;
            }
            else
            {
                _actualSpawnsPerDay = value;
            }
        }
    }
    #endregion

    #region Fields
    [SerializeField] private float _defaultSpawnsPerDay;
    [SerializeField] private float _maxSpawnsPerDay;
    private float _actualSpawnsPerDay;
    private float _gameTime;
    private float _spawnTime;
    private float _previousGameTime;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        GameManager.Instance.GameTimeManager.OnNewDay += SpawnTimeUpdate;
        ActualSpawnsPerDay = _defaultSpawnsPerDay;
    }
    void Update()
    {
        _gameTime = GameManager.Instance.GameTimeManager.Time;
        for (int i = 1; i <= ActualSpawnsPerDay; i++)
        {
            _spawnTime = (360 / ActualSpawnsPerDay) * i;
            if (_previousGameTime < _spawnTime && _gameTime >= _spawnTime) //Comprueba que se ha sobrepasado el número entero
            {
                SpawnRandom();
            }
        }
        _previousGameTime = _gameTime;
        if (_gameTime < _previousGameTime)
        {
            _previousGameTime = 0;
        }

    }
    #endregion

    #region Public Methods
    public virtual void SpawnRandom()
    {

    }
    public virtual void Spawn(GameObject obj)
    {

    }
    #endregion

    #region Private Methods
    private void SpawnTimeUpdate(int day)
    {
        GodState actualGodState = GameManager.Instance.ActualGodState;
        switch (actualGodState)
        {
            case GodState.Delighted:
                ActualSpawnsPerDay += 3;
                break;
            case GodState.Satisfied:
                ActualSpawnsPerDay++;
                break;
            case GodState.Neutral:
                break;
            case GodState.Unsatisfied:
                ActualSpawnsPerDay--;
                break;
            case GodState.Furious:
                ActualSpawnsPerDay -= 3;
                break;
            default:
                Debug.LogWarning("NO HAY UN ESTADO DE DIOSES ASIGNADO");
                break;
        }
    }
    #endregion

}
