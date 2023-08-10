
using System;
using System.ComponentModel.DataAnnotations;

namespace PanteonTestCase.Enums
{
    [Serializable]
    public enum ResultType
    {
        [Display(Name = "Success")]
        Success = 1,

        [Display(Name = "Error")]
        Error = 2,

        [Display(Name = "Warning")]
        Warning = 3
    }
}
