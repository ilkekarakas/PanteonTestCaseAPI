using MongoDB.Bson;

namespace PanteonTestCase.Dtos
{
    public class BuildingConfigurationListDto
    {
        public string Id { get; set; }
        public int BuildingType { get; set; }
        public int BuildingCost { get; set; }
        public int ConstructionTime { get; set; }
    }
}
