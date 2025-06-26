// Tile.cs
using UnityEngine;
using VillageRaisingJourney.Data;

namespace VillageRaisingJourney.World
{
    public class Tile : MonoBehaviour // MonoBehaviourを継承してタイルをGameObjectとしてシーンに配置できるようにする
    {
        public TileData tileData;
        public Vector2Int gridPosition;

        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        public void Initialize(TileData data, Vector2Int position)
        {
            tileData = data;
            gridPosition = position;
            gameObject.name = $"Tile_{position.x}_{position.y}_{data.type}";
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (tileData != null && tileData.sprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = tileData.sprite;
            }
            else if (spriteRenderer != null)
            {
                // デフォルトの見た目（例えば色分けなど）
                // spriteRenderer.color = GetColorForTileType(tileData.type);
            }
        }

        // TileDataにSpriteがない場合のフォールバックとして色を設定する例
        /*
        private Color GetColorForTileType(TileType type)
        {
            switch (type)
            {
                case TileType.Plains: return Color.green;
                case TileType.Forest: return new Color(0.0f, 0.5f, 0.0f); // Dark Green
                case TileType.Mountains: return Color.grey;
                default: return Color.white;
            }
        }
        */
    }
}
