using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEdge.Monitor.Common
{
    public static class Fallible
    {
        public static Fallible<T> Success<T>(T value)
        {
            return new Fallible<T>(value);
        }

        public static Fallible<T> Fail<T>(Exception exception)
        {
            return new Fallible<T>(exception);
        }

        public static Fallible<T> Operation<T>(Func<T> operation)
        {
            try
            {
                T value = operation();

                return Success(value);
            }
            catch (Exception e)
            {
                return Fail<T>(e);
            }
        }

        public static Fallible<T> Operation<T>(T value, Action<T> operation)
        {
            try
            {
                operation(value);

                return Success(value);
            }
            catch (Exception e)
            {
                return Fail<T>(e);
            }
        }

        public static async Task<Fallible<T>> AsyncOperation<T>(Func<Task<T>> operation)
        {
            try
            {
                T value = await operation();

                return Success(value);
            }
            catch (Exception e)
            {
                return Fail<T>(e);
            }
        }

        public static void ToTaskCompletionSource<T>(this Fallible<T> source, TaskCompletionSource<T> tcs)
        {
            if (source.IsSuccessful)
            {
                tcs.SetResult(source.Value);
            }
            else
            {
                tcs.SetException(source.Exception);
            }
        }

        public static TSource Coalesce<TSource>(this Fallible<TSource> source, Func<Exception, TSource> projection)
        {
            return source.IsSuccessful ? source.Value : projection(source.Exception);
        }

        public static Fallible<TResult> Select<TSource, TResult>(this Fallible<TSource> source, Func<TSource, TResult> success)
        {
            return source.IsSuccessful ? Fallible.Success(success(source.Value)) : Fallible.Fail<TResult>(source.Exception);
        }

        public static TResult Select<TSource, TResult>(this Fallible<TSource> source, Func<TSource, TResult> success, Func<Exception, TResult> fail)
        {
            return source.IsSuccessful ? success(source.Value) : fail(source.Exception);
        }

        public static async Task<TResult> Select<TSource, TResult>(this Task<Fallible<TSource>> source, Func<TSource, TResult> success, Func<Exception, TResult> fail)
        {
            var result = await source;

            return result.Select(success, fail);
        }

        public static T ValueOrThrow<T>(this Fallible<T> source, Func<Exception, Exception> exceptionFactory)
        {
            if (source.IsSuccessful)
            {
                return source.Value;
            }
            else
            {
                throw exceptionFactory(source.Exception);
            }
        }

        public static T ValueOrThrow<T>(this Fallible<T> source)
        {
            return ValueOrThrow(source, ex => ex);
        }

        public static IEnumerable<T> Collect<T>(this IEnumerable<Fallible<T>> source)
        {
            return source
                .Where(fallible => fallible.IsSuccessful)
                .Select(fallible => fallible.Value);
        }

        public static Fallible<T> Convert<T>(object value)
        {
            if (value is T dest)
            {
                return Success(dest);
            }
            else
            {
                return Operation(() => (T)System.Convert.ChangeType(value, typeof(T)));
            }
        }
    }

    public interface IFallible
    {
        bool IsSuccessful { get; }
        bool IsError { get; }
        Exception Exception { get; }
    }

    public struct Fallible<T> : IFallible
    {
        public Fallible(T value)
        {
            IsSuccessful = true;
            Value = value;
            Exception = null;
        }

        public Fallible(Exception exception)
        {
            IsSuccessful = false;
            Value = default(T);
            Exception = exception;
        }

        public bool IsSuccessful { get; private set; }
        public bool IsError => !IsSuccessful;
        public T Value { get; private set; }
        public Exception Exception { get; private set; }
    }
}
