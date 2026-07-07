using UnityEngine;

public class Resource : HealthSystem
{
    #region Properties
    public ResourceType Type;
    public enum ResourceType
    {
        Tree,
        Rock,
        Animal
    }
    #endregion

    #region Fields
    [SerializeField] private Item[] _spawns;
    #endregion

    #region Unity Callbacks
    public virtual void Start()
    {
        name = Type.ToString();
    }
    #endregion

    #region Public Methods
    public override void Die()
    {
        base.Die();
        int i = 4;
        foreach (Item item in _spawns)
        {
            if (Random.Range(0, i) < 4) //no siempre instanciará todo, pero al menos instanciará los 2 primeros items
            {
                Instantiate(item,transform.position+Vector3.up*2,Quaternion.identity);
                if (item.name.Contains("Log"))
                {
                    item.transform.localScale = Vector3.one*5;
                }
            }
            i++;
        }
        if (Type == ResourceType.Tree)
            GameManager.Instance.GameAudioController.PlayTreeDestroy();
        else if (Type == ResourceType.Rock)
            GameManager.Instance.GameAudioController.PlayRockDestroy();
        Destroy(this.gameObject);
    }
    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        if(Type == ResourceType.Tree)
            GameManager.Instance.GameAudioController.PlayChoppingTree();
        else if(Type == ResourceType.Rock)
            GameManager.Instance.GameAudioController.PlayMiningRock();
    }
    #endregion

    #region Private Methods
    #endregion

}
