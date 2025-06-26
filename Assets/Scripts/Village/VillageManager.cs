// VillageManager.cs
using UnityEngine;
using VillageRaisingJourney.World; // Tileを参照するため4
using VillageRaisingJourney.Data; // ResourceTypeやTileDataを参照するため

namespace VillageRaisingJourney.Village
{
    public class VillageManager : MonoBehaviour
    {
        public VillageStats currentStats;
        public GameObject villagePrefab; // 村の見た目となるプレハブ
        private GameObject villageInstance;
        private WorldGenerator worldGenerator; // WorldGeneratorへの参照

        // 村の初期ステータス
        [Header("Initial Village Stats")]
        public int initialPopulation = 10;
        public int initialFood = 50;
        public int initialWood = 20;
        public int initialGold = 10;

        public Vector2Int CurrentPosition { get; private set; }

        void Awake() // StartからAwakeに変更してGameManagerより先にWorldGeneratorを見つけられるようにする
        {
            worldGenerator = FindObjectOfType<WorldGenerator>();
            if (worldGenerator == null)
            {
                Debug.LogError("WorldGenerator not found in scene for VillageManager.");
            }
            InitializeVillage();
        }

        void InitializeVillage()
        {
            currentStats = new VillageStats(initialPopulation, initialFood, initialWood, initialGold);
            Debug.Log("Village initialized with stats.");
        }

        public void PlaceVillage(Vector2Int gridPosition, Transform parentTransform)
        {
            CurrentPosition = gridPosition;
            Vector3 worldPos;

            if (worldGenerator != null)
            {
                worldPos = worldGenerator.GetWorldPosition(gridPosition);
            }
            else
            {
                Debug.LogError("WorldGenerator reference is missing in VillageManager. Using gridPosition as worldPosition.");
                worldPos = new Vector3(gridPosition.x, gridPosition.y, 0); // フォールバック
            }
            
            if (villagePrefab != null)
            {
                villageInstance = Instantiate(villagePrefab, worldPos, Quaternion.identity, parentTransform);
                villageInstance.name = "Village";
            }
            else
            {
                Debug.LogWarning("Village Prefab is not assigned in VillageManager. Creating a default GameObject for the village.");
                villageInstance = new GameObject("Village");
                villageInstance.transform.position = worldPos;
                if(parentTransform != null) villageInstance.transform.SetParent(parentTransform);
            }
            Debug.Log($"Village placed at grid position: {gridPosition}, world position: {worldPos}");
        }

        public void MoveVillage(Vector2Int newGridPosition)
        {
            CurrentPosition = newGridPosition;
            if (villageInstance != null && worldGenerator != null)
            {
                Vector3 newWorldPos = worldGenerator.GetWorldPosition(newGridPosition);
                villageInstance.transform.position = newWorldPos;
                Debug.Log($"Village moved to grid: {newGridPosition}, world: {newWorldPos}");

                // 移動先のタイルからリソースを収集
                Tile newTile = worldGenerator.GetTileAt(newGridPosition);
                if (newTile != null)
                {
                    CollectResourcesFromTile(newTile);
                }
                 // UIも更新する (GameManager経由が望ましいが、直接呼ぶことも可能)
                if (GameManager.GameManager.Instance != null && GameManager.GameManager.Instance.uiManager != null)
                {
                    GameManager.GameManager.Instance.uiManager.UpdateStatusUI(currentStats, GameManager.GameManager.Instance.CurrentTurn);
                }
            }
            else
            {
                Debug.LogWarning("Cannot move village: villageInstance or worldGenerator is null.");
            }
        }

        public void UpdateVillageStatsPerTurn()
        {
            // 人口増加（仮のロジック）
            if (currentStats.Food > currentStats.Population)
            {
                currentStats.Population += 1;
            }
            else if (currentStats.Food < currentStats.Population / 2 && currentStats.Population > 5) // 食料が人口の半分未満で人口が5以上なら減少
            {
                currentStats.Population -=1;
            }


            // 食料消費
            currentStats.Food -= currentStats.Population;
            currentStats.Food = Mathf.Max(0, currentStats.Food); // 0未満にならないようにする

            // 木材や金の自然増加/減少は、タイルの影響やイベントで後ほど実装
            // 木材が消費されるロジックがある場合は currentStats.Wood = Mathf.Max(0, currentStats.Wood - consumedWood); のようにする
            Debug.Log($"Village stats updated. Pop:{currentStats.Population}, Food:{currentStats.Food}, Wood:{currentStats.Wood}, Gold:{currentStats.Gold}");
        }

        public void CollectResourcesFromTile(Tile tile)
        {
            if (tile == null || tile.tileData == null)
            {
                Debug.LogWarning("Current tile or tile data is null. Cannot collect resources.");
                return;
            }

            foreach (var yield in tile.tileData.resourceYields)
            {
                switch (yield.resourceType)
                {
                    case ResourceType.Food:
                        currentStats.Food = Mathf.Max(0, currentStats.Food + yield.amount);
                        break;
                    case ResourceType.Wood:
                        currentStats.Wood = Mathf.Max(0, currentStats.Wood + yield.amount);
                        break;
                    case ResourceType.Gold:
                        currentStats.Gold += yield.amount; // Goldはマイナスを許容
                        break;
                }
                Debug.Log($"Collected {yield.amount} of {yield.resourceType} from {tile.tileData.type} tile. New totals: F:{currentStats.Food}, W:{currentStats.Wood}, G:{currentStats.Gold}");
            }
        }
    }
}