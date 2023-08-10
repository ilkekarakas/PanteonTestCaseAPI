using Newtonsoft.Json;
using PanteonTestCase.Enums;
using System;

namespace PanteonTestCase.Models
{
    [Serializable]
    public class ServiceResult
    {

        [JsonProperty(PropertyName = "resultType")]
        public ResultType ResultType { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        public ServiceResult(string message = "", ResultType state = ResultType.Success)
        {
            ResultType = state;
            Message = message;
        }
    }
    [Serializable]
    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }

        public ServiceResult(T result, string message = "", ResultType state = ResultType.Success)
            : base(message, state)
        {
            Data = result;
        }
    }
}
