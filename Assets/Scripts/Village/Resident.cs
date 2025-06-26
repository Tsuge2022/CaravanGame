// Resident.cs
namespace VillageRaisingJourney.Village
{
    [System.Serializable] // Inspectorで表示したり、シリアライズする場合に便利
    public class Resident
    {
        public string Name;
        public Occupation Job;

        public Resident(string name, Occupation job)
        {
            Name = name;
            Job = job;
        }
    }
}
