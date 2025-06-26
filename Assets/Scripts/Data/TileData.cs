using UnityEngine;

namespace VillageRaisingJourney.Data // 名前空間を .Data を追加
{
    public enum TileType
    {
        Plains,
        Forest,
        Mountains
    }

    [CreateAssetMenu(fileName = "TileData", menuName = "VillageRaisingJourney/TileData")]
    public class TileData : ScriptableObject
    {
        public TileType type;
        public Sprite sprite; // タイルの見た目（将来的に使用）
        public ResourceYield[] resourceYields; // このタイルで得られる資源
    }

    [System.Serializable]
    public class ResourceYield
    {
        public ResourceType resourceType; // これは VillageRaisingJourney.Data.ResourceType を参照するようになる
        public int amount;
    }
}