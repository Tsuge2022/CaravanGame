// VillageManager.cs
using UnityEngine;
using VillageRaisingJourney.World; // Tileを参照するため
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

        // 初期住民のランダムな名前の候補
        private List<string> firstNames = new List<string> { "Alden", "Brynn", "Cedric", "Daria", "Errol", "Fiona", "Garrett", "Helena" };
        private List<string> lastNames = new List<string> { "Stone", "River", "Moon", "Iron", "Swift", "Bright", "Shadow", "Flame" };


        void InitializeVillage()
        {
            currentStats = new VillageStats(0, initialFood, initialWood, initialGold); // 人口は住民リストから設定
            GenerateInitialResidents();
            currentStats.Population = currentStats.Residents.Count; // 住民の数に基づいて人口を更新
            Debug.Log($"Village initialized. Population: {currentStats.Population}, Food: {currentStats.Food}, Wood: {currentStats.Wood}, Gold: {currentStats.Gold}");
            foreach(var resident in currentStats.Residents)
            {
                Debug.Log($"Resident: {resident.Name}, Job: {resident.Job}");
            }
        }

        void GenerateInitialResidents()
        {
            if (currentStats == null) currentStats = new VillageStats(0,0,0,0); // 安全策

            // 各職業1名ずつ生成 (例)
            AddResidentWithRandomName(Occupation.Farmer);
            AddResidentWithRandomName(Occupation.Craftsman);
            AddResidentWithRandomName(Occupation.Mercenary);
            AddResidentWithRandomName(Occupation.Merchant);

            // 追加で何名かランダムな職業で生成 (例: さらに2名)
            for(int i=0; i<2; i++)
            {
                Occupation randomJob = (Occupation)Random.Range(0, System.Enum.GetValues(typeof(Occupation)).Length);
                AddResidentWithRandomName(randomJob);
            }
        }

        void AddResidentWithRandomName(Occupation job)
        {
            string name = firstNames[Random.Range(0, firstNames.Count)] + " " + lastNames[Random.Range(0, lastNames.Count)];
            // 名前が重複しないようにする簡単なチェック (より堅牢なシステムではID管理など)
            while(currentStats.Residents.Exists(r => r.Name == name))
            {
                name = firstNames[Random.Range(0, firstNames.Count)] + " " + lastNames[Random.Range(0, lastNames.Count)] + " II"; // 仮対応
            }
            currentStats.Residents.Add(new Resident(name, job));
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
            if (currentStats == null || currentStats.Residents == null) return;

            // 人口変動ロジック
            bool populationIncreased = false;
            bool populationDecreased = false;

            // 人口増加条件: 食料が現在の人口より多い
            if (currentStats.Food > currentStats.Residents.Count && currentStats.Residents.Count > 0)
            {
                Occupation randomNewJob = (Occupation)Random.Range(0, System.Enum.GetValues(typeof(Occupation)).Length);
                AddResidentWithRandomName(randomNewJob);
                populationIncreased = true;
                Debug.Log($"A new resident ({randomNewJob}) has joined the caravan!");
            }
            // 人口減少条件: 食料が人口の半分未満 かつ 人口が一定数以上 (例: 1人より多い)
            // かつ食料が0 (餓死の表現)
            else if (currentStats.Food == 0 && currentStats.Residents.Count > 1 )
            {
                if (currentStats.Residents.Count > 0)
                {
                    int residentToRemoveIndex = Random.Range(0, currentStats.Residents.Count);
                    Resident removedResident = currentStats.Residents[residentToRemoveIndex];
                    currentStats.Residents.RemoveAt(residentToRemoveIndex);
                    populationDecreased = true;
                    Debug.LogWarning($"A resident ({removedResident.Name}, {removedResident.Job}) has left or perished due to lack of food!");
                }
            }

            // 人口ステータスを住民リストの数に合わせる
            currentStats.Population = currentStats.Residents.Count;

            // 食料消費 (変動後の人口に基づいて消費)
            currentStats.Food -= currentStats.Population;
            currentStats.Food = Mathf.Max(0, currentStats.Food);

            string popChangeMsg = populationIncreased ? "Population increased." : (populationDecreased ? "Population decreased." : "Population stable.");
            Debug.Log($"Village stats updated. {popChangeMsg} Pop:{currentStats.Population}, Food:{currentStats.Food}, Wood:{currentStats.Wood}, Gold:{currentStats.Gold}");
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

        public string GetResidentCountsByOccupationString()
        {
            if (currentStats == null || currentStats.Residents == null) return "住民情報なし";

            var occupationCounts = new Dictionary<Occupation, int>();
            foreach (Occupation job in System.Enum.GetValues(typeof(Occupation)))
            {
                occupationCounts[job] = 0;
            }

            foreach (var resident in currentStats.Residents)
            {
                occupationCounts[resident.Job]++;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("住民構成: ");
            foreach (var pair in occupationCounts)
            {
                // 職業名を短縮して表示する例（F:農 C:職 M:傭 T:商）
                string jobShortName = "";
                switch(pair.Key) {
                    case Occupation.Farmer: jobShortName = "農"; break;
                    case Occupation.Craftsman: jobShortName = "職"; break;
                    case Occupation.Mercenary: jobShortName = "傭"; break;
                    case Occupation.Merchant: jobShortName = "商"; break;
                    default: jobShortName = pair.Key.ToString().Substring(0,1); break;
                }
                sb.Append($"{jobShortName}:{pair.Value} ");
            }
            return sb.ToString().Trim();
        }
    }
}
