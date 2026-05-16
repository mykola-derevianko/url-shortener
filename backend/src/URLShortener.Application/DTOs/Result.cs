namespace URLShortener.Application.DTOs
{
    public class Result
    {
        public bool Succeeded { get; protected set; }
        public string? ErrorCode { get; protected set; }
        public string? ErrorMessage { get; protected set; }

        public static Result Success() => new Result { Succeeded = true };

        public static Result Failure(string errorCode, string errorMessage)
        {
            return new Result
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }

    public class Result<T> : Result
    {
        public T? Value { get; private set; }

        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                Succeeded = true,
                Value = value
            };
        }

        public static new Result<T> Failure(string errorCode, string errorMessage)
        {
            return new Result<T>
            {
                Succeeded = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
}
