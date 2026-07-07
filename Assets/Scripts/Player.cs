using StarterAssets;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Player : HealthSystem
{
    #region Properties
    [SerializeField] public Transform HoldPosition;
    #endregion

    #region Fields
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private StarterAssetsInputs _playerInputs;
    [SerializeField] private float _rayDistance;
    [SerializeField] private Resource _resourceDetected;
    [SerializeField] private Item _itemDetected;
    private bool _smthDetected;
    [SerializeField] private Item _heldItem;
    [SerializeField] private Vector3 _spawnPosition;

    #endregion

    #region Unity Callbacks
    private void Start()
    {
        _playerInputs = GetComponent<StarterAssetsInputs>();
        _heldItem = null;
        Heal(MaxHealth);
    }
    private void Update()
    {
        _smthDetected = false;
        if (_heldItem != null)
        {
            GameManager.Instance.GameUIController.ShowInteractionText("Pulsa Q para soltar " + _heldItem.name);
            _smthDetected = true;
            if (Input.GetKeyUp(KeyCode.Q))
            {
                ThrowItem();
            }
        }
        else if (_itemDetected != null)
        {
            GameManager.Instance.GameUIController.ShowInteractionText("Pulsa E para recoger " + _itemDetected.name);
            _smthDetected = true;
            if (Input.GetKeyUp(KeyCode.E))
            {
                CollectNewItem(_itemDetected);
            }
        }
        if (_resourceDetected != null)
        {
            GameManager.Instance.GameUIController.ShowInteractionText("Pulsa E para romper " + _resourceDetected.name);
            _smthDetected = true;
            if (Input.GetKeyUp(KeyCode.E))
            {
                DamageResource();
            }
        }
        if(!_smthDetected)
            GameManager.Instance.GameUIController.ShowInteractionText("");
        //Sonido de pasos
        if (_playerInputs.move.x>0.2f||_playerInputs.move.y>0.2f)
            GameManager.Instance.GameAudioController.PlayerWalking = true;
        else
            GameManager.Instance.GameAudioController.PlayerWalking = false;
    }
    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 cameraPosition = _playerCamera.transform.position;
        Debug.DrawRay(cameraPosition, _playerCamera.transform.forward * _rayDistance, Color.red);
        if(Physics.Raycast(cameraPosition,_playerCamera.transform.forward,out hit, _rayDistance))
        {
            Item item = hit.collider.GetComponent<Item>();
            if(item != null)
            {
                _itemDetected = item;
                _resourceDetected = null;
            }
            else
            {
                _itemDetected = null;
                if (hit.collider.gameObject.TryGetComponent<Resource>(out Resource resource))
                {
                    _resourceDetected = resource;
                }
                else
                {
                    _resourceDetected = null;
                }
            }

        }
        else
        {
            _resourceDetected = null;
            _itemDetected = null;
        }
    }
    #endregion

    #region Public Methods
    public void CollectNewItem(Item newItem)
    {
        ThrowItem();
        newItem.Owner = this.gameObject;
        _heldItem=newItem;
        _heldItem.GetComponent<Rigidbody>().isKinematic = true;
        foreach (Collider collider in _heldItem.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }
    public void ThrowItem()
    {
        if (_heldItem != null)
        {
            if (_heldItem.gameObject.TryGetComponent<Seed>(out Seed seed))
            {
                seed.Active = true;
            }
            _heldItem.GetComponent<Rigidbody>().isKinematic = false;
            foreach (Collider collider in _heldItem.GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
            _heldItem.Owner = null;
            _heldItem = null;
        }
    }
    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        GameManager.Instance.GameUIController.UpdateHealthSlider(Health,MaxHealth);
        GameManager.Instance.GameAudioController.PlayAuch();
    }
    public override void Heal(float heal)
    {
        base.Heal(heal);
        GameManager.Instance.GameUIController.UpdateHealthSlider(Health, MaxHealth);
    }
    public override void Die()
    {
        //Efectos/Sonido de muerte
        ThrowItem();
        base.Die();
    }
    public void ResetPlayer()
    {
        Heal(MaxHealth);
        CharacterController characterController = gameObject.GetComponent<CharacterController>();
        characterController.enabled = false;
        transform.position = _spawnPosition;
        characterController.enabled = true;

        transform.rotation = Quaternion.identity;
        _playerInputs.move.x = 0;
        _playerInputs.move.y = 0;
        _playerInputs.jump=false;
        _playerInputs.sprint=false;
    }
    #endregion

    #region Private Methods
    private void DamageResource()
    {
        if (_heldItem != null)
        {
            if (_heldItem.Type == Item.ItemType.Axe && _resourceDetected.Type == Resource.ResourceType.Tree)
            {
                _resourceDetected.TakeDamage(_heldItem.GetComponent<Tool>().Damage);
                //TODO: Efecto de sonido de daño al recurso
            }
            else if (_heldItem.Type == Item.ItemType.Pickaxe && _resourceDetected.Type == Resource.ResourceType.Rock)
            {
                _resourceDetected.TakeDamage(_heldItem.GetComponent<Tool>().Damage);
                //TODO: Efecto de sonido de daño al recurso
            }
            else
            {
                _resourceDetected.TakeDamage(1);
                TakeDamage(1);
            }
        }
        else
        {
            _resourceDetected.TakeDamage(1);
            TakeDamage(1);
            //TODO: Efecto de sonido de daño al jugador y al recurso
        }
    }
    #endregion
}
