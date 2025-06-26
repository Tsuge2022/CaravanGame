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

        public VillageStats(int population, int food, int wood, int gold)
        {
            Population = population;
            Food = food;
            Wood = wood;
            Gold = gold;
        }
    }
}
