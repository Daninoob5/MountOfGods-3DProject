using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    #region Properties
    public float GodPoints
    {
        get
        {
            return _godPoints;
        }
        private set
        {
            _godPoints = value;
        }
    }
    public ItemType Type; 
    public enum ItemType
    {
        Stick,
        Stone,
        Log,
        Seed,
        Axe,
        Pickaxe,
        Bronze,
        Silver,
        Gold,
        Platinum,
    }
    public GameObject Owner;
    #endregion

    #region Fields
    [SerializeField] private float _godPoints;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Vector3 _heldRotation;
    [SerializeField] private bool _verticalOffSet;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        name = Type.ToString();
        Owner = null;
        if (tag == "Untagged")
            tag = "Item";
    }
    void Update()
    {
        if (Owner != null)
            FollowOwner();
        //else
            //_rb.useGravity=true;
    }
    #endregion

    #region Public Methods
    public virtual void FollowOwner()
    {
        //_rb.useGravity = false; Versión sin Kinematic
        //_rb.linearVelocity = Vector3.zero;
        if (_verticalOffSet)
            transform.position = Vector3.Lerp(transform.position, Owner.GetComponent<Player>().HoldPosition.position - Vector3.up*0.8f, 20 * Time.deltaTime);
        else
            transform.position = Vector3.Lerp(transform.position, Owner.GetComponent<Player>().HoldPosition.position, 20 * Time.deltaTime);

        if (_heldRotation==Vector3.zero)
            transform.rotation = Camera.main.transform.rotation;
        else
                transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(_heldRotation);
    }
    #endregion

    #region Private Methods
    #endregion
}

