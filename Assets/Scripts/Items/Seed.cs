using System.Collections;
using UnityEngine;

public class Seed : Item
{
    #region Properties
    public bool Active;
    #endregion

    #region Fields
    [SerializeField] private Resource[] _trees;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        Active = false;
        Type = ItemType.Seed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Active && collision.gameObject.TryGetComponent<Terrain>(out Terrain terrain))
        {
            PlantTree();  
        }
    }
    #endregion

    #region Public Methods
    
    #endregion

    #region Private Methods
    private void PlantTree()
    {
        Instantiate(_trees[Random.Range(0,_trees.Length)], transform.position-Vector3.up*transform.localScale.y, Quaternion.identity);
        Destroy(this.gameObject);
    }
    #endregion

}
