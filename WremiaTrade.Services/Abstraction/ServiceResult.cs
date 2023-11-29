namespace Papara.Services.Abstraction
{
    using Newtonsoft.Json;

    public class ServiceResult<T> : ServiceResult
    {
        public ServiceResult() { }

        public T Data { get; set; }

        public ServiceResult(T data)
        {
            Data = data;
            Error = null;
        }

        public ServiceResult(ServiceError error) : base(error)
        {

        }

        public ServiceResult(ServiceSuccessResult result) : base(result)
        {

        }
    }

    public class ServiceResult
    {
        //To use private setter.
        [JsonProperty]
        public bool Succeeded
        {
            get => Error == null;
            private set
            {
                if (value)
                {
                    Error = null;
                }
                else
                {
                    if (Error == null)
                    {
                        Error = ServiceError.DefaultError;
                    }
                }
            }
        }

        [JsonIgnore]
        public string Feature { get; set; }

        public ServiceError Error { get; set; }

        public ServiceSuccessResult Result { get; set; }

        public ServiceResult(ServiceError error)
        {
            if (error == null)
            {
                error = ServiceError.DefaultError;
            }

            this.Error = error;
        }

        public ServiceResult(ServiceSuccessResult result)
        {
            if (result == null)
            {
                result = ServiceSuccessResult.DefaultServiceSuccessResult;
            }

            Result = result;
        }

        public ServiceResult()
        {
            Error = ServiceError.DefaultError;
        }

        #region Helper Methods

        public static ServiceResult Failed(ServiceError error)
        {
            return new ServiceResult(error);
        }

        public static ServiceResult<T> Failed<T>(ServiceError error)
        {
            return new ServiceResult<T>(error);
        }

        public static ServiceResult<T> Failed<T>(ServiceError error, T data)
        {
            return new ServiceResult<T>(error) { Data = data };
        }
        public static ServiceResult Failed(string feature, ServiceError error)
        {
            return new ServiceResult(error) { Feature = feature };
        }

        public static ServiceResult<T> Failed<T>(string feature, ServiceError error)
        {
            return new ServiceResult<T>(error) { Feature = feature };
        }

        public static ServiceResult<T> Failed<T>(string feature, ServiceError error, T data)
        {
            return new ServiceResult<T>(error) { Data = data, Feature = feature };
        }

        public static ServiceResult Success()
        {
            return new ServiceResult { Error = null };
        }

        public static ServiceResult Success(ServiceSuccessResult serviceSuccessResult)
        {
            return new ServiceResult(serviceSuccessResult);
        }

        public static ServiceResult<T> Success<T>(T data)
        {
            return new ServiceResult<T>(data);
        }

        public static ServiceResult<T> Success<T>(ServiceSuccessResult serviceSuccessResult, T data)
        {
            return new ServiceResult<T>(serviceSuccessResult) { Data = data };
        }

        #endregion
    }
}

