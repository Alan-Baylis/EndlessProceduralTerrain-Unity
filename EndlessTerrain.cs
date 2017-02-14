using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{
	//Drag & Drop Components

	//Components & Prefabs
    public Transform viewer;
    public Material mapMaterial;
    private static MapGenerator mapGenerator;

	//Variables
    int chunkSize;
    int chunksVisibleInViewDst;
    public const float maxViewDst = 450;
    public static Vector2 viewerPosition;
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	private void Awake()
	{

	}

	private void Start()
	{
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
	}

	private void OnDestroy()
	{

	}
	
	void Update()
	{
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
	}

    private void UpdateVisibleChunks()
    {
        for(int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for(int yOffSet = -chunksVisibleInViewDst; yOffSet <= chunksVisibleInViewDst; yOffSet++)
        {
            for(int xOffSet = -chunksVisibleInViewDst; xOffSet <= chunksVisibleInViewDst; xOffSet++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffSet, currentChunkCoordY + yOffSet);

                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        //Components
        private GameObject meshObject;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        //Variables
        private Vector2 position;
        private Bounds bounds;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            //Initialize grid position, bounding box, and world position
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0.0f, position.y);

            //Initialize GameObject and Components
            meshObject = new GameObject("TerrainChunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            
            //Set world position and parents in Hierarchy
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            
            //Default to inactive state
            SetVisible(false);

            //Start generating map data on separate thread
            mapGenerator.RequestMapData(OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData)
        {
            mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            //Determine distance to viewer
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            //Check if distance is within view distance
            bool isVisible = viewerDistanceFromNearestEdge <= maxViewDst;

            //Set GameObject active/inactive
            SetVisible(isVisible);
        }

        public void SetVisible(bool visible)
        {
            //Set GameObject active/inactive if visible == true/false
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            //Can be called when GameObject isn't active because class isn't Monobehavior
            return meshObject.activeSelf;
        }
    }
}
