// WorldGenerator.cs
using UnityEngine;
using System.Collections.Generic;
using VillageRaisingJourney.Data; // TileDataを使用するため

namespace VillageRaisingJourney.World
{
    public class WorldGenerator : MonoBehaviour
    {
        [Header("World Settings")]
        public int worldWidth = 3;
        public int worldHeight = 3;
        public float tileSize = 1.0f; // タイルの物理的なサイズ（Unity単位）

        [Header("Tile Data")]
        public List<TileData> availableTileTypes; // Inspectorから設定するタイルデータのリスト
        public GameObject tilePrefab; // タイル表示用のプレハブ（SpriteRendererを持つ空のGameObjectなど）

        private Tile[,] worldGrid;
        private Transform worldContainer; // 生成されたタイルをまとめる親オブジェクト

        void Awake()
        {
            if (availableTileTypes == null || availableTileTypes.Count == 0)
            {
                Debug.LogError("AvailableTileTypes is not set or empty in WorldGenerator.");
                return;
            }
            if (tilePrefab == null)
            {
                Debug.LogError("Tile Prefab is not assigned in WorldGenerator.");
                // return; // プレハブがなくても処理を続け、Tileスクリプト側でSpriteRendererをAddComponentするように変更
            }
            CreateWorldContainer();
        }

        void CreateWorldContainer()
        {
            GameObject containerGO = new GameObject("WorldGridContainer");
            worldContainer = containerGO.transform;
            worldContainer.SetParent(this.transform); // WorldGeneratorの子にする
        }

        public void GenerateWorld()
        {
            if (availableTileTypes == null || availableTileTypes.Count == 0)
            {
                Debug.LogError("Cannot generate world: AvailableTileTypes is not set or empty.");
                return;
            }

            worldGrid = new Tile[worldWidth, worldHeight];

            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    TileData selectedTileData = GetRandomTileData();

                    GameObject tileGO;
                    if (tilePrefab != null)
                    {
                        tileGO = Instantiate(tilePrefab, GetWorldPosition(gridPosition), Quaternion.identity, worldContainer);
                    }
                    else
                    {
                        // プレハブがない場合は空のGameObjectを作成し、TileスクリプトがSpriteRendererを追加する
                        tileGO = new GameObject();
                        tileGO.transform.position = GetWorldPosition(gridPosition);
                        tileGO.transform.SetParent(worldContainer);
                    }

                    Tile tileComponent = tileGO.GetComponent<Tile>();
                    if (tileComponent == null)
                    {
                        tileComponent = tileGO.AddComponent<Tile>();
                    }

                    tileComponent.Initialize(selectedTileData, gridPosition);
                    worldGrid[x, y] = tileComponent;
                }
            }
            Debug.Log("World generated.");
        }

        TileData GetRandomTileData()
        {
            if (availableTileTypes == null || availableTileTypes.Count == 0) return null;
            return availableTileTypes[Random.Range(0, availableTileTypes.Count)];
        }

        public Vector3 GetWorldPosition(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x * tileSize, gridPosition.y * tileSize, 0);
        }

        public Tile GetTileAt(Vector2Int gridPosition)
        {
            if (IsWithinBounds(gridPosition))
            {
                return worldGrid[gridPosition.x, gridPosition.y];
            }
            return null;
        }

        public bool IsWithinBounds(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < worldWidth &&
                   gridPosition.y >= 0 && gridPosition.y < worldHeight;
        }

        public Vector2Int GetRandomGridPosition()
        {
            return new Vector2Int(Random.Range(0, worldWidth), Random.Range(0, worldHeight));
        }

        public Transform GetWorldContainer()
        {
            return worldContainer;
        }
    }
}
