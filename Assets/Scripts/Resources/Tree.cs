using UnityEngine;

public class Tree : Resource
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private Collider _collider;
    [SerializeField] private float _growingTime;
    [SerializeField] private float _growingPhase;
    [SerializeField] private float _timer;
    [SerializeField] private bool _growing;
    #endregion

    #region Unity Callbacks
    public override void Start()
    {
        _growing = true;
        transform.localScale = Vector3.one * 0.1f;
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _timer = 1;
        _growingPhase = 1;
        base.Start();
    }
    void Update()
    {
        if (_growing)
        {
            Grow();
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void Grow() //TODO: Sincronizar con GameTime
    {
        _timer += Time.deltaTime;
        if (_timer >= _growingTime / 10 * _growingPhase)
        {
            transform.localScale = Vector3.one * _growingPhase / 10;
            _growingPhase++;
            if (_growingPhase >= 10)
            {
                transform.localScale = Vector3.one;
                _collider.enabled = true;
                _growing = false;
            }
        }
    }
    #endregion

}
