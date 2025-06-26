// TileData.cs
using UnityEngine;

namespace VillageRaisingJourney
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
        public ResourceType resourceType;
        public int amount;
    }
}
