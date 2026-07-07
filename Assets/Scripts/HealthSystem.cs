using System;
using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour 
{
    #region Properties
    [SerializeField] public float MaxHealth;
    public float Health
    {
        get
        {
            return _health;
        } 
        set
        {
            if (value>MaxHealth)
            {
                _health = MaxHealth;
            }
            else if(value>0)
            {
                _health = value;
            }
            else
            {
                _health = 0;
                Die();
            }
        }
    }
    public event Action OnDeath;
    #endregion

    #region Fields
    [SerializeField] private float _health;
    private int _onFireCharges;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        Health = MaxHealth;
    }
    #endregion

    #region Public Methods
    public  virtual void TakeDamage(float dmg)
    {
        if (dmg > 0)
            Health -= dmg;
        else
            Debug.LogWarning("El valor del daño recibido es negativo");
        //Efectos / sonido
    }
    public virtual void Heal(float heal)
    {
        if(heal>0)
            Health += heal;
        else
            Debug.LogWarning("El valor de la curación recibida es negativo");
        //Efectos / sonido
    }
    public  virtual void Die()
    {
        OnDeath?.Invoke();
        _onFireCharges = 0;
        gameObject.SetActive(false);
    }
    public void Burn(int intensity)
    {
        _onFireCharges += intensity;
        StartCoroutine(OnFire());
    }
    #endregion

    #region Private Methods
    private IEnumerator OnFire()
    {
        while (_onFireCharges > 0 && Health>0)
        {
            TakeDamage(5*_onFireCharges);
            Debug.Log("Te quemas ("+_onFireCharges+")");
            //Efectos / Sonido
            _onFireCharges--;
            if (1 / _onFireCharges > 0.1)
                yield return new WaitForSeconds(1/_onFireCharges);
            else 
                yield return new WaitForSeconds(0.1f);
        }
        _onFireCharges = 0;
        yield return null;
    }
    #endregion

}
