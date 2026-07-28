using Photon.Pun;
using StarterAssets;
using System.ComponentModel;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class Player : HealthSystem
{
    #region Properties
    [SerializeField] public Transform HoldPosition;
    [SerializeField] public bool Local;
    [SerializeField] public Camera PlayerCamera;
    #endregion

    #region Fields
    [SerializeField] private StarterAssetsInputs _playerInputs;
    [SerializeField] private float _rayDistance;
    [SerializeField] private Resource _resourceDetected;
    [SerializeField] private Item _itemDetected;
    private bool _smthDetected;
    [SerializeField] private Item _heldItem;
    [SerializeField] private Vector3 _spawnPosition;
    [SerializeField] private Animator _animator;

    #endregion

    #region Unity Callbacks
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerInputs = GetComponent<StarterAssetsInputs>();
        ResetPlayer();
    }
    private void Update()
    {
        if (Local)
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
            //Sonido y animación de pasos
            if (_playerInputs.move.x > 0.2f || _playerInputs.move.y > 0.2f || _playerInputs.move.x < -0.2f || _playerInputs.move.y < -0.2f)
            {
                GameManager.Instance.GameAudioController.PlayerWalking = true;
                _animator.SetBool("Run", true);
            }
            else
            {
                GameManager.Instance.GameAudioController.PlayerWalking = false;
                _animator.SetBool("Run", false);
            }
            if (_playerInputs.jump)
                _animator.SetBool("Jump", true);
            else
                _animator.SetBool("Jump", false);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (Input.GetMouseButtonDown(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    void FixedUpdate()
    {
        if (Local)
        {
            RaycastHit hit;
            Vector3 cameraPosition = PlayerCamera.transform.position;
            Debug.DrawRay(cameraPosition, PlayerCamera.transform.forward * _rayDistance, Color.red);
            if(Physics.Raycast(cameraPosition,PlayerCamera.transform.forward,out hit, _rayDistance))
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
    }
    #endregion

    #region Public Methods

    public void CollectNewItem(Item newItem)
    {
        if (_itemDetected.Owner == null)
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
        //TODO: Avisar al jugador que no puede recoger el item porque ya lo tiene otro jugador
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
        //TODO: Sonido de muerte
        ThrowItem();
        _animator.SetTrigger("Death");
        _playerInputs.cursorLocked = false;
        base.Die();
    }
    public void ResetPlayer()
    {
        Heal(MaxHealth);
        Alive=true;
        GameManager.Instance.NetworkController.IsAliveUpdate(true, GetComponent<PhotonView>().ViewID);
        transform.position = _spawnPosition;
        if (Local)
        {   
            CharacterController characterController = gameObject.GetComponent<CharacterController>();
            characterController.enabled = false;
            characterController.enabled = true;
            _playerInputs = GetComponent<StarterAssetsInputs>();
            transform.rotation = Quaternion.identity;
            _playerInputs.move.x = 0;
            _playerInputs.move.y = 0;
            _playerInputs.jump=false;
            _playerInputs.sprint=false;
        }
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
            }
            else if (_heldItem.Type == Item.ItemType.Pickaxe && _resourceDetected.Type == Resource.ResourceType.Rock)
            {
                _resourceDetected.TakeDamage(_heldItem.GetComponent<Tool>().Damage);
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
        }
    }
    #endregion
}
