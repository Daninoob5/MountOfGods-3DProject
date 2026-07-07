using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class HolyLava : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    #endregion

    #region Unity Callbacks
    void Start()
    {

    }
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj.tag == "Item")
        {
            GameManager.Instance.AddGodPoints(obj.GetComponent<Item>().GodPoints);
            Destroy(obj, 1);
            obj.GetComponent<Collider>().enabled = false;
        }
        else if (obj.tag == "Player")
        {
            GameManager.Instance.AddGodPoints(1);
            obj.GetComponent<Player>().Burn(10);
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
