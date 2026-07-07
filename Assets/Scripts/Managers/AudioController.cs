using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    #region Properties
    [SerializeField] public bool PlayerWalking;
    #endregion

    #region Fields
    [Header("Music")]
    [SerializeField] private AudioSource _music;
    [Header("FX")]
    [SerializeField] private AudioSource _steps;
    [SerializeField] private AudioSource _rockDestroy;
    [SerializeField] private AudioSource _miningRock;
    [SerializeField] private AudioSource _treeDestroy;
    [SerializeField] private AudioSource _choppingTree;
    [SerializeField] private AudioSource _auch;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        PlayMusic(true);
        GameManager.Instance.OnGameOver += () => PlayMusic(false);
        StartCoroutine(WalkingSounds());
    }
    #endregion

    #region Public Methods
    public void PlayMusic(bool On)
    {
        if(On)
            _music.Play();
        else
            _music.Stop();
    }
    public void PlayStep()
    {
        _steps.Play();
    }
    public void PlayRockDestroy()
    {
        _rockDestroy.Play();
    }
    public void PlayTreeDestroy()
    {
        _treeDestroy.Play();
    }
    public void PlayAuch()
    {
        _auch.Play();
    }
    public void PlayMiningRock()
    {
        _miningRock.Play();
    }
    public void PlayChoppingTree()
    {
        _choppingTree.Play();
    }
    #endregion

    #region Private Methods
    private IEnumerator WalkingSounds()
    {
        while (true)
        {
            if (PlayerWalking)
            {
                PlayStep();
                yield return new WaitForSeconds(0.8f);
            }
            else
            {
                yield return null;
            } 
        }
    }
    #endregion
}
