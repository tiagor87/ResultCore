using System;
using System.Collections.Generic;
using System.Linq;

namespace ResultCore
{
    public sealed class Result
    {
        private readonly object _value; 
        
        private Result()
        {
            _value = null;
            Successful = true;
        }

        private Result(object value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            Successful = true;
        }

        private Result(string message)
        {
            Message = message;
        }
        
        private Result(IEnumerable<string> messages) : this (string.Join("\n", messages))
        {
        }

        public object Value =>
            Successful
                ? _value
                : throw new InvalidOperationException(
            $"The result has failed, it's not possible to get value from it.\nMessage: \"{Message}\".");
        public bool Successful { get; }
        public string Message { get; }
        public bool Failure => !Successful;

        public static Result Success() => new Result();
        public static Result Success(object value) => new Result(value);
        public static Result Fail(string message) => new Result(message);
        public static Result Fail(IEnumerable<string> messages) => new Result(messages);

        public T As<T>() => (T) Value;

        public static Result Combine(params Result[] results)
        {
            if (results.Any(x => x.Successful == false))
            {
                return Fail(results.Where(x => !x.Successful).Select(x => x.Message));
            }

            var genericType = Type.GetType($"System.ValueTuple`{results.Length}");
            var typeArgs = results.Select(x => x.Value.GetType()).ToArray();
            var specificType = genericType.MakeGenericType(typeArgs);
            var constructorArguments = results.Select(x => x.As<object>()).ToArray();
            return Success(Activator.CreateInstance(specificType, constructorArguments));
        }
    }

    public sealed class Result<TValue> 
    {
        private readonly TValue _value;
        
        private Result(TValue value)
        {
            Successful = true;
            _value = value;
        }
        
        private Result(string message)
        {
            Message = message;
        }
        
        private Result(IEnumerable<string> messages) : this (string.Join("\n", messages))
        {
        }
        
        public bool Successful { get; }
        public bool Failure => !Successful;
        public string Message { get; }
        
        public TValue Value =>
            Successful
                ? _value
                : throw new InvalidOperationException(
                    $"The result has failed, it's not possible to get value from it.\nMessage: \"{Message}\".");
        
        public static Result<TValue> Success(TValue value) => new Result<TValue>(value);
        public static Result<TValue> Fail(string message) => new Result<TValue>(message);
        public static Result<TValue> Fail(IEnumerable<string> messages) => new Result<TValue>(messages);

        public static implicit operator Result(Result<TValue> result)
        {
            return result.Successful
                ? Result.Success(result.Value)
                : Result.Fail(result.Message);
        }
    }
}