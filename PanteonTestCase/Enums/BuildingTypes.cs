using System.ComponentModel.DataAnnotations;

namespace PanteonTestCase.Enums
{
    public enum BuildingTypes
    {
        [Display(Name = "Farm")]
        Farm = 1,

        [Display(Name = "Academy")]
        Academy = 2,

        [Display(Name = "Headquarters")]
        Headquarters = 3,

        [Display(Name = "LumberMill")]
        LumberMill = 4,

        [Display(Name = "Barracks")]
        Barracks = 5,
    }
}
