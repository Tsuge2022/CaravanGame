// GameEventData.cs
using UnityEngine;
using VillageRaisingJourney.Data; // ResourceTypeのため

namespace VillageRaisingJourney.Events
{
    [CreateAssetMenu(fileName = "NewGameEvent", menuName = "VillageRaisingJourney/Game Event")]
    public class GameEventData : ScriptableObject
    {
        [TextArea(3, 5)]
        public string eventMessageTemplate = "{resource}が{amount}変化しました。"; // 例: "食料が10増加しました。" / "金が5減少しました。"
        public ResourceType targetResource;
        public int minAmountChange; // マイナス値で減少、プラス値で増加
        public int maxAmountChange; // minAmountChangeより大きい値を想定

        public string GetFormattedMessage(string resourceName, int actualAmount)
        {
            string changeVerb = actualAmount >= 0 ? "増加しました" : "減少しました";
            // メッセージテンプレートが "{resource}が{amount}{verb}。" のような形式を期待する場合
            // return eventMessageTemplate.Replace("{resource}", resourceName).Replace("{amount}", Mathf.Abs(actualAmount).ToString()).Replace("{verb}", changeVerb);

            // シンプルなメッセージの場合
            return $"{resourceName}が{Mathf.Abs(actualAmount)} {changeVerb}";
        }
    }
}
