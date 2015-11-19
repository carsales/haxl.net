using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Haxl.Net.Errors;

namespace Haxl.Net
{
    public static class AsyncExtensions
    {
        public static async Task<C> SelectMany<A, B, C>(this Task<A> task, Func<A, Task<B>> bind, Func<A, B, C> project)
        {
            var a = await task.Run();
            var b = await bind(a).Run();

            return project(a, b);
        }

        public static Task<A> Where<A>(this Task<A> task, Expression<Func<A, bool>> predicate) =>
            task.Run().ContinueWith(t =>
            {
                if (predicate.Compile().Invoke(t.Result)) return t.Result;
                throw new HaxlPredicateFailedException($"Predicate failed: ({predicate.Body})");
            });

        public static Task<A> Run<A>(this Task<A> task)
        {
            if (task.Status == TaskStatus.Created) task.Start();
            return task;
        }
    }
}