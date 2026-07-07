using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Properties
    public float Time {  get; private set; }
    public int Day;
    [SerializeField] public bool TimeOn;
    [SerializeField] public bool Timelapse;
    public event Action<int> OnNewDay;
    #endregion

    #region Field
    private float _dayLenght = 360;
    [SerializeField] private GameObject _sunLight;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        GameManager.Instance.OnGameOver += () => { TimeOn = false; };
        Day = 0;
        TimeOn = true;
        Timelapse = false;
    }
    void Update()
    {
        if (TimeOn)
        {
            if (Time >= _dayLenght)
            {
                Time = 0;
                Day++;
                GameManager.Instance.UpdateGodState();
                OnNewDay?.Invoke(Day);
            }
            if (Timelapse)
            {
                Time += UnityEngine.Time.deltaTime*50;
                //TODO: Bloquear o modificar el spawn de objetos cuando se hace timelapse
            }
            else
            {
                Time += UnityEngine.Time.deltaTime;
            }
            _sunLight.transform.rotation = Quaternion.Euler(Time,0,0);
        }

    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
