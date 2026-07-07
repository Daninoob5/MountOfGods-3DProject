using System;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

public class TerrainManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private Terrain _mainTerrain;
    [SerializeField] private GameObject[] _terrains;
    private Dictionary<Vector2Int, Terrain> _terrainGrid = new();
    private List<Vector2Int> _availablePositions = new List<Vector2Int>();
    private Vector2Int[] _directions = {Vector2Int.left,Vector2Int.right,Vector2Int.up,Vector2Int.down};
    private enum TerrainSide
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [Header("TERRAIN GENERATOR")]
    [SerializeField] private bool _generateRandomTerrain;
    [SerializeField] private bool _generateTerrain;
    [SerializeField] private int _terrainIndex;
    [SerializeField] private Vector2Int _newTerrainPos;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        GameManager.Instance.GameTimeManager.OnNewDay += NewDayReward;
        _terrainGrid.Add(Vector2Int.zero, _mainTerrain);
    }
    void Update()
    {
        if (_generateTerrain)
        {
            GenerateNewPlot(_newTerrainPos, _terrainIndex);
            _generateTerrain = false;
        }
        if( _generateRandomTerrain)
        {
            GenerateRandomPlot();
            _generateRandomTerrain = false;
        }
    }
    #endregion

    #region Public Methods
    public void GenerateNewPlot(Vector2Int gridPosition,int prefabIndex)
    {
        //TODO: Poner un límite a la cantidad de casillas que puedes generar
        if (_terrainGrid.ContainsKey(gridPosition))
        {
            Debug.LogWarning("YA EXISTE UN TERRENO EN LA POSICIÓN: (" + gridPosition.x +", " + gridPosition.y + ")." );
            return;
        }
        Vector3 worldPosition = new Vector3(gridPosition.x*100,0,gridPosition.y*100);
        GameObject terrainGO = Instantiate(_terrains[prefabIndex], worldPosition, Quaternion.identity);
        Terrain newTerrain = terrainGO.GetComponent<Terrain>();
        newTerrain.terrainData = Instantiate(newTerrain.terrainData); //Instancia un terrainData separado para no modificar el original
        _terrainGrid.Add(gridPosition, newTerrain);
        UpdateNeighbors(gridPosition);
    }
    public void GenerateRandomPlot()
    {
        //TODO: Si el proceso lleva mucho tiempo, implementar pantalla de carga
        //Limpieza antes de comenzar el proceso
        _availablePositions.Clear();
        //Selección de casillas disponibles
        foreach (Vector2Int plot in _terrainGrid.Keys)
        {
            //Debug.Log("Comprobando casillas disponibles en: " + plot);
            foreach(Vector2Int direction in _directions)
            {
                Vector2Int candidate = plot + direction;
                if (!_terrainGrid.ContainsKey(candidate) && !_availablePositions.Contains(candidate))
                {
                    _availablePositions.Add(candidate);
                    //Debug.Log("Candidato añadido en: " + candidate);
                }
                //Debug.Log("Candidato descartado en: " + candidate);
            }
        }
        //Elección de una de las casillas al azar
        if (_availablePositions.Count == 0)
        {
           Debug.LogWarning("NO HAY CASILLAS DISPONIBLES");
        }
        else
        {
            Vector2Int randomPosition = _availablePositions[Random.Range(0, _availablePositions.Count)];
            int randomPrefabIndex = Random.Range(0, _terrains.Length);
            //Generación del terreno
            GenerateNewPlot(randomPosition, randomPrefabIndex);
        }
    }
    #endregion

    #region Private Methods
    private void NewDayReward(int day)
    {
        GodState actualGodState = GameManager.Instance.ActualGodState;
        switch (actualGodState)
        {
            case GodState.Delighted: Debug.Log("Spawneando terreno porque GodState = Delighted");
                    GenerateRandomPlot();
                    if (Random.Range(0, 3) == 2)
                        GenerateRandomPlot();
                break;
            case GodState.Satisfied: Debug.Log("Spawneando terreno porque GodState = Satisfied");
                    GenerateRandomPlot();
                break;
            case GodState.Neutral: 
                        if(Random.Range(0,3) == 2)
                        {
                            GenerateRandomPlot();
                            Debug.Log("Spawneando terreno porque GodState = Neutral");
                        }   
                break;
            case GodState.Unsatisfied:
                break;
            case GodState.Furious:
                break;
            default:
                    Debug.LogWarning("NO HAY UN ESTADO DE DIOSES ASIGNADO");
                break;

        }
    }
    private void UpdateNeighbors(Vector2Int gridPosition)
    {
        //Referencia de cada terreno adyacente
        Terrain terrain = _terrainGrid[gridPosition];
        Terrain left = GetTerrain(gridPosition + Vector2Int.left);
        Terrain right = GetTerrain(gridPosition + Vector2Int.right);
        Terrain top = GetTerrain(gridPosition + Vector2Int.up);
        Terrain bottom = GetTerrain(gridPosition + Vector2Int.down);
        //Asignación como vecinos
        terrain.SetNeighbors(left, top, right, bottom);
        //Asignación de vecinos de vecinos y unión de terrenos
        if (left != null)
        {
            left.SetNeighbors(GetTerrain(gridPosition + Vector2Int.left * 2),GetTerrain(gridPosition + Vector2Int.left + Vector2Int.up),terrain,GetTerrain(gridPosition + Vector2Int.left + Vector2Int.down));
            JoinTerrainBorders(terrain, left, TerrainSide.Left);
        }
        if (right != null)
        {
            right.SetNeighbors(terrain,GetTerrain(gridPosition + Vector2Int.right + Vector2Int.up),GetTerrain(gridPosition + Vector2Int.right * 2),GetTerrain(gridPosition + Vector2Int.right + Vector2Int.down));
            JoinTerrainBorders(terrain, right, TerrainSide.Right);
        }
        if(top != null)
        {
            top.SetNeighbors(GetTerrain(gridPosition + Vector2Int.up + Vector2Int.left),GetTerrain(gridPosition + Vector2Int.up * 2),GetTerrain(gridPosition + Vector2Int.up + Vector2Int.right),terrain);
            JoinTerrainBorders(terrain, top, TerrainSide.Top);
        }
        if (bottom != null)
        {
            bottom.SetNeighbors(GetTerrain(gridPosition + Vector2Int.down + Vector2Int.left),terrain,GetTerrain(gridPosition + Vector2Int.down + Vector2Int.right),GetTerrain(gridPosition + Vector2Int.down * 2));
            JoinTerrainBorders(terrain, bottom, TerrainSide.Bottom);
        }
        //Recalcular la conectividad de los terrenos
        Terrain.SetConnectivityDirty();
    }
    private Terrain GetTerrain(Vector2Int pos) //Devuelve un terreno en una posición si lo hay
    {
        if( _terrainGrid.TryGetValue(pos, out Terrain terrain))
            return terrain;
        else
            return null;
    }
    private void JoinTerrainBorders(Terrain terrain, Terrain neighbour, TerrainSide side)
    {
        TerrainData terrainData = terrain.terrainData;
        TerrainData neighbourData = neighbour.terrainData;
        int resolution = terrainData.heightmapResolution;
        float[,] terrainBorder;
        float[,] neighbourBorder;
        switch (side)
        {
            case TerrainSide.Left:
                        terrainBorder = terrainData.GetHeights(0, 0, 1, resolution);
                        neighbourBorder = neighbourData.GetHeights(resolution-1,0,1,resolution);
                        AverageBorders(terrainBorder, neighbourBorder);
                        terrainData.SetHeights(0, 0, terrainBorder);
                        neighbourData.SetHeights(resolution - 1, 0, neighbourBorder);
                break;
            case TerrainSide.Right:
                        terrainBorder = terrainData.GetHeights(resolution-1, 0, 1, resolution);
                        neighbourBorder = neighbourData.GetHeights(0, 0, 1, resolution);
                        AverageBorders(terrainBorder, neighbourBorder);
                        terrainData.SetHeights(resolution-1, 0, terrainBorder);
                        neighbourData.SetHeights(0, 0, neighbourBorder);
                break;
            case TerrainSide.Top:
                        terrainBorder = terrainData.GetHeights(0, resolution-1, resolution,1);
                        neighbourBorder = neighbourData.GetHeights(0, 0, resolution, 1);
                        AverageBorders(terrainBorder, neighbourBorder);
                        terrainData.SetHeights(0, resolution-1, terrainBorder);
                        neighbourData.SetHeights(0, 0, neighbourBorder);
                break;
            case TerrainSide.Bottom:
                        terrainBorder = terrainData.GetHeights(0, 0, resolution,1);
                        neighbourBorder = neighbourData.GetHeights(0, resolution-1,resolution,1);
                        AverageBorders(terrainBorder, neighbourBorder);
                        terrainData.SetHeights(0, 0, terrainBorder);
                        neighbourData.SetHeights(0, resolution-1, neighbourBorder);
                break;
        }
    }
    private void AverageBorders(float[,] terrainBorder, float[,] neighbourBorder)
    {
        int height = terrainBorder.GetLength(0);
        int width = terrainBorder.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for(int x =0; x < width; x++)
            {
                /*
                //TODO: Arreglar bordes de terrenos
                //float average = ((terrainBorder[y,x]+neighbourBorder[y,x])*0.5f);
                //terrainBorder[y,x] = average;
                //neighbourBorder[y,x] = average;
                */
                terrainBorder[y,x] = neighbourBorder[y,x];
            }
        }
    }


    #endregion

}
