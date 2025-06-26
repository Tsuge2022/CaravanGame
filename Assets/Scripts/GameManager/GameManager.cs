// GameManager.cs
using UnityEngine;
using VillageRaisingJourney.World;
using VillageRaisingJourney.Village;
using VillageRaisingJourney.UI;

namespace VillageRaisingJourney.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Component References")]
        public WorldGenerator worldGenerator;
        public VillageManager villageManager;
        public UIManager uiManager;
        public VillageRaisingJourney.Events.EventManager eventManagerRef; // EventManagerへの参照

        private int currentTurn = 0;
        public int CurrentTurn => currentTurn;
        private bool villageHasMovedThisTurn = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // GameManagerはシーンをまたいで存在し続ける
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // 他の重要なコンポーネントが設定されているか確認
            if (worldGenerator == null) Debug.LogError("WorldGenerator is not assigned in GameManager.");
            if (villageManager == null) Debug.LogError("VillageManager is not assigned in GameManager.");
            if (uiManager == null) Debug.LogError("UIManager is not assigned in GameManager.");
            if (eventManagerRef == null) Debug.LogError("EventManager is not assigned in GameManager.");
        }

        void Start()
        {
            InitializeGame();
        }

        void InitializeGame()
        {
            if (worldGenerator != null)
            {
                worldGenerator.GenerateWorld();
            }
            else
            {
                Debug.LogError("WorldGenerator is null, cannot generate world.");
                return; // ワールド生成に失敗したら初期化を中断することも検討
            }

            if (villageManager != null && worldGenerator != null)
            {
                // VillageManagerのInitializeVillageはStartで呼ばれるのでここでは不要かも
                // villageManager.InitializeVillage();
                Vector2Int startPosition = worldGenerator.GetRandomGridPosition();
                Transform worldContainer = worldGenerator.GetWorldContainer();
                villageManager.PlaceVillage(startPosition, worldContainer);

                // 村が配置されたタイルから初期リソースを得る
                Tile initialTile = worldGenerator.GetTileAt(startPosition);
                if (initialTile != null)
                {
                    villageManager.CollectResourcesFromTile(initialTile);
                }
            }
            else
            {
                Debug.LogError("VillageManager or WorldGenerator is null, cannot place village.");
            }

            Debug.Log("Game Initialized. Current Turn: " + currentTurn);

            // UIの初期化と初回更新
            if (uiManager != null && villageManager != null && villageManager.currentStats != null)
            {
                uiManager.UpdateStatusUI(villageManager.currentStats, currentTurn);
            }
            else
            {
                Debug.LogError("UIManager or VillageManager.currentStats is null, cannot update UI on init.");
            }
        }

        public void NextTurn()
        {
            currentTurn++;
            villageHasMovedThisTurn = false; // ターン開始時に移動フラグをリセット
            Debug.Log($"--- Turn {currentTurn} Start (Move allowed) ---");

            if (villageManager != null)
            {
                villageManager.UpdateVillageStatsPerTurn(); // ターン毎のステータス変動

                // 現在のタイルからリソースを収集
                Tile currentTile = worldGenerator.GetTileAt(villageManager.CurrentPosition);
                if (currentTile != null)
                {
                    villageManager.CollectResourcesFromTile(currentTile);
                }
            }
            else
            {
                Debug.LogWarning("VillageManager is null, cannot process village turn.");
            }

            if (uiManager != null && villageManager != null && villageManager.currentStats != null)
            {
                uiManager.UpdateStatusUI(villageManager.currentStats, currentTurn);
            }
            else
            {
                Debug.LogWarning("UIManager or VillageManager.currentStats is null, cannot update UI on next turn.");
            }
            Debug.Log($"--- Turn {currentTurn} End ---");
        }

        public void AttemptMoveVillage(Vector2Int direction)
        {
            if (villageManager == null || worldGenerator == null)
            {
                Debug.LogError("VillageManager or WorldGenerator is not set in GameManager. Cannot attempt move.");
                return;
            }

            Vector2Int currentPos = villageManager.CurrentPosition;
            Vector2Int targetPos = currentPos + direction;

            if (villageHasMovedThisTurn)
            {
                Debug.LogWarning("Village has already moved this turn. Cannot move again until next turn.");
                // UIにフィードバックを出すことも検討 (例: UIManager経由でメッセージ表示)
                return;
            }

            if (worldGenerator.IsWithinBounds(targetPos))
            {
                Debug.Log($"Attempting to move village from {currentPos} to {targetPos}");
                villageManager.MoveVillage(targetPos); // これによりタイルからの資源収集とUI内の座標更新が行われる
                villageHasMovedThisTurn = true;
                Debug.Log("Village has moved this turn.");

                // 移動後にランダムイベントを発生させる
                if (eventManagerRef != null && villageManager != null && villageManager.currentStats != null)
                {
                    string eventMessage = eventManagerRef.TriggerRandomEvent(villageManager.currentStats);
                    if (uiManager != null)
                    {
                        uiManager.LogEventMessage(eventMessage);
                        // イベントによって資源が変動したので、ステータスUIも更新
                        uiManager.UpdateStatusUI(villageManager.currentStats, currentTurn);
                    }
                }
                else
                {
                    Debug.LogWarning("EventManager, VillageManager, or VillageStats is null. Cannot trigger event or log.");
                }
            }
            else
            {
                Debug.LogWarning($"Cannot move village to {targetPos}. Out of bounds.");
            }
        }
    }
}
