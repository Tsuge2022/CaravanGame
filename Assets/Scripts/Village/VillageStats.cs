// VillageStats.cs
namespace VillageRaisingJourney.Village
{
    [System.Serializable]
    public class VillageStats
    {
        public int Population;
        public int Food;
        public int Wood;
        public int Gold;
        public List<Resident> Residents; // 住民リスト

        public VillageStats(int population, int food, int wood, int gold)
        {
            Population = population; // 初期人口は住民リストの数で更新する方が良いかも
            Food = food;
            Wood = wood;
            Gold = gold;
            Residents = new List<Resident>();
        }
    }
}
