using UnityEngine;

public class ResourceSpawner : Spawner
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private Resource[] _resources;
    [SerializeField] private Vector2 _spawnArea;
    #endregion

    #region Unity Callbacks
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_spawnArea.x, 0, _spawnArea.y));
    }
    #endregion

    #region Public Methods
    public override void SpawnRandom()
    {
        Spawn(_resources[Random.Range(0, _resources.Length)].gameObject);
    }
    public override void Spawn(GameObject item)
    {
        if (item != null)
        {
            Vector3 randomPos = transform.position;
            randomPos.x += Random.Range(-_spawnArea.x / 2, _spawnArea.x / 2);
            randomPos.z += Random.Range(-_spawnArea.y / 2, _spawnArea.y / 2);
            if(Physics.Raycast(randomPos,-Vector3.up, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent<Terrain>(out Terrain terrain))
                {
                    randomPos.y = hit.point.y;  
                    Instantiate(item, randomPos, Quaternion.identity);
                }
            }
        }
    }
    #endregion

    #region Private Methods
    #endregion
}
