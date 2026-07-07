using UnityEngine;
using static GameManager;
public class ItemSpawner : Spawner
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private Item[] _items;
    [SerializeField] private float _spawnRadius;
    #endregion

    #region Unity Callbacks
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,_spawnRadius);
    }
    #endregion

    #region Public Methods
    public override void SpawnRandom()
    {
        Spawn(_items[Random.Range(0, _items.Length)].gameObject);
    }
    public override void Spawn(GameObject item)
    {
        if (item!=null)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * _spawnRadius;
            Instantiate(item, randomPos, Quaternion.identity);
        }
    }
    #endregion

    #region Private Methods
    #endregion

}
