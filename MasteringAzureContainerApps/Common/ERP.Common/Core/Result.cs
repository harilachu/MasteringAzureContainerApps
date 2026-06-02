using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Common.Core
{
    public record Error(string Status, string Message);

    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }

        private Result(bool isSuccess, Error? error)
        {
            this.IsSuccess = isSuccess;
            this.Error = error;
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(Error error) => new Result(false, error);
    }

    public class Result<TValue>
    {
        public bool IsSuccess { get; }
        public TValue Value { get; }
        public Error? Error { get; }

        private Result(bool isSuccess, TValue value, Error? error)
        {
            this.IsSuccess = isSuccess;
            this.Value = value;
            this.Error = error;
        }

        public static Result<TValue> Success(TValue value) => new Result<TValue>(true, value, null);
        public static Result<TValue> Failure(Error error) => new Result<TValue>(false, default, error);

    }

    public static class ResultExtensions
    {
        public static void Match(this Result result, Action onSuccess, Action<Error> onFailure)
        {
            if (result.IsSuccess)
            {
                onSuccess();
            }
            else if (result.Error != null)
            {
                onFailure(result.Error);
            }
        }

        public static TResult Match<TValue, TResult>(
            this Result<TValue> result,
            Func<TValue, TResult> onSuccess,
            Func<Error, TResult> onFailure)
        {
            if (result.IsSuccess)
            {
                return onSuccess(result.Value);
            }
            else if (result.Error != null)
            {
                return onFailure(result.Error);
            }
            else
            {
                throw new InvalidOperationException("Invalid Result state: Failure result must contain an error.");
            }
        }
    }
}
