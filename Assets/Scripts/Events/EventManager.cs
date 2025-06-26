// EventManager.cs
using UnityEngine;
using System.Collections.Generic;
using VillageRaisingJourney.Village; // VillageStatsのため
using VillageRaisingJourney.Data;   // ResourceTypeのため

namespace VillageRaisingJourney.Events
{
    public class EventManager : MonoBehaviour
    {
        public List<GameEventData> availableEvents;

        public string TriggerRandomEvent(VillageStats villageStats)
        {
            if (availableEvents == null || availableEvents.Count == 0)
            {
                return "特に何も起こらなかった...";
            }

            GameEventData selectedEvent = availableEvents[Random.Range(0, availableEvents.Count)];

            int amountChange = Random.Range(selectedEvent.minAmountChange, selectedEvent.maxAmountChange + 1);

            string resourceName = "";

            switch (selectedEvent.targetResource)
            {
                case ResourceType.Food:
                    villageStats.Food = Mathf.Max(0, villageStats.Food + amountChange);
                    resourceName = "食料";
                    break;
                case ResourceType.Wood:
                    villageStats.Wood = Mathf.Max(0, villageStats.Wood + amountChange);
                    resourceName = "木材";
                    break;
                case ResourceType.Gold:
                    villageStats.Gold += amountChange; // 金はマイナス許容
                    resourceName = "金";
                    break;
                // 他のリソースタイプがあればここに追加
            }

            string message = selectedEvent.GetFormattedMessage(resourceName, amountChange);
            Debug.Log($"Event Triggered: {message}. Village Stats Updated: Food:{villageStats.Food}, Wood:{villageStats.Wood}, Gold:{villageStats.Gold}");
            return message;
        }
    }
}
