// UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using VillageRaisingJourney.Village; // VillageStatsを参照するため
using VillageRaisingJourney.GameManager; // GameManagerを参照するため

namespace VillageRaisingJourney.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Status Panel")]
        public Text populationText;
        public Text foodText;
        public Text woodText;
        public Text goldText;
        public Text turnText; // 現在のターン数を表示
        public Text villagePositionText; // 村の現在座標を表示 (デバッグ用)


        [Header("Control Panel")]
        public Button nextTurnButton;
        public Button moveUpButton;
        public Button moveDownButton;
        public Button moveLeftButton;
        public Button moveRightButton;

        void Start()
        {
            if (nextTurnButton != null) nextTurnButton.onClick.AddListener(OnNextTurnButtonClicked);
            else Debug.LogError("Next Turn Button is not assigned in UIManager.");

            if (moveUpButton != null) moveUpButton.onClick.AddListener(() => OnMoveButtonClicked(Vector2Int.up));
            else Debug.LogWarning("Move Up Button is not assigned in UIManager.");

            if (moveDownButton != null) moveDownButton.onClick.AddListener(() => OnMoveButtonClicked(Vector2Int.down));
            else Debug.LogWarning("Move Down Button is not assigned in UIManager.");

            if (moveLeftButton != null) moveLeftButton.onClick.AddListener(() => OnMoveButtonClicked(Vector2Int.left));
            else Debug.LogWarning("Move Left Button is not assigned in UIManager.");

            if (moveRightButton != null) moveRightButton.onClick.AddListener(() => OnMoveButtonClicked(Vector2Int.right));
            else Debug.LogWarning("Move Right Button is not assigned in UIManager.");
        }

        public void UpdateStatusUI(VillageStats stats, int currentTurn)
        {
            if (stats == null)
            {
                Debug.LogError("VillageStats is null. Cannot update UI.");
                return;
            }

            if (populationText != null) populationText.text = "Population: " + stats.Population;
            if (foodText != null) foodText.text = "Food: " + stats.Food;
            if (woodText != null) woodText.text = "Wood: " + stats.Wood;
            if (goldText != null) goldText.text = "Gold: " + stats.Gold;
            if (turnText != null) turnText.text = "Turn: " + currentTurn;

            // 村の座標を表示 (GameManagerからVillageManagerの座標を取得)
            if (villagePositionText != null && GameManager.GameManager.Instance != null && GameManager.GameManager.Instance.villageManager != null)
            {
                villagePositionText.text = "Position: " + GameManager.GameManager.Instance.villageManager.CurrentPosition.ToString();
            }
        }

        void OnNextTurnButtonClicked()
        {
            if (GameManager.GameManager.Instance != null)
            {
                GameManager.GameManager.Instance.NextTurn();
            }
            else
            {
                Debug.LogError("GameManager instance not found for Next Turn.");
            }
        }

        void OnMoveButtonClicked(Vector2Int direction)
        {
            if (GameManager.GameManager.Instance != null)
            {
                GameManager.GameManager.Instance.AttemptMoveVillage(direction);
            }
            else
            {
                Debug.LogError("GameManager instance not found for Move action.");
            }
        }
    }
}
