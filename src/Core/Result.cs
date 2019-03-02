using System;
using System.Linq;

namespace ResultCore
{
    public class Result
    {
        private Result()
        {
            Value = null;
            Successful = true;
        }

        private Result(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Successful = true;
        }

        private Result(params string[] messages)
        {
            Message = string.Join("\n", messages);
        }

        private object Value { get; }
        public bool Successful { get; }
        public string Message { get; }
        public bool Failure => Successful == false;

        public static Result Success() => new Result();
        public static Result Success(object value) => new Result(value);
        public static Result Fail(string message) => new Result(message);

        public T As<T>() => Successful
            ? (T) Value
            : throw new InvalidOperationException(
                $"The result has failed, it's not possible to get value from it.\nMessage: \"{Message}\".");

        public static Result Combine(params Result[] results)
        {
            if (results.Any(x => x.Successful == false))
            {
                return Fail(string.Join("\n", results.Where(x => x.Successful == false).Select(x => x.Message)));
            }

            var genericType = Type.GetType($"System.ValueTuple`{results.Length}");
            var typeArgs = results.Select(x => x.Value.GetType()).ToArray();
            var specificType = genericType.MakeGenericType(typeArgs);
            var constructorArguments = results.Select(x => x.As<object>()).ToArray();
            return Success(Activator.CreateInstance(specificType, constructorArguments));
        }
    }
}