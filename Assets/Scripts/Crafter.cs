using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static Item;

public class Crafter : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private Item _item1;
    [SerializeField] private Item _item2;
    [SerializeField] private ItemType[] _ingredients1;
    [SerializeField] private ItemType[] _ingredients2;
    [SerializeField] private GameObject[] _results;
    private Dictionary<(ItemType, ItemType), GameObject> _recipes = new();
    [SerializeField] private Vector3 _spawnPosition;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _item1 = null;
        _item2 = null;
        for (int i=0; i<_results.Length; i++)
        {
            _recipes.Add((_ingredients1[i], _ingredients2[i]), _results[i]);
        }
    }
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Item>(out Item newItem))
        {
            if (_item1 == null)
            {
                _item1 = newItem;
            }
            else if(_item2==null)
            {
                _item2 = newItem;
                CheckRecipes();
            }
            else
            {
                Debug.LogWarning("El crafter ha detectado más de dos objetos en su caja");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Item>(out Item item))
        {
            if (_item1 == item)
                _item1 = null;
            else if (_item2 == item)
                _item2 = null;
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void CheckRecipes()
    {
        bool recipeFound = false;
        foreach (KeyValuePair<(ItemType,ItemType),GameObject> recipe in _recipes) 
        {
            if (!recipeFound)
            {
                ItemType ingredient1 = recipe.Key.Item1;
                ItemType ingredient2 = recipe.Key.Item2;
                if ((ingredient1==_item1.Type && ingredient2==_item2.Type)||(ingredient1==_item2.Type && ingredient2==_item1.Type))//Comprueba que están los objetos
                {
                    Destroy(_item1.gameObject);
                    Destroy(_item2.gameObject);
                    _item1 = null;
                    _item2 = null;
                    Instantiate(recipe.Value,_spawnPosition,Quaternion.identity);
                    GameManager.Instance.GameUIController.ShowCrafterText("Se ha creado un " + recipe.Value.name);
                    recipeFound = true;
                }
            }
        }
        if (!recipeFound)
        {
            GameManager.Instance.GameUIController.ShowCrafterText("No existe una receta con " + _item1.name + " y "+ _item2.name + ". Prueba con otros objetos!");
            _item1.transform.Translate(Vector3.up*3);
            _item2.transform.Translate(Vector3.up*3);
            _item1 = null;
            _item2 = null;
        }
    }
    #endregion

}
