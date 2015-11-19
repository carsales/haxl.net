using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Haxl.Net.Errors;
using NUnit.Framework;

namespace Haxl.Net.Tests
{
    public class AsyncExtensionTests
    {
        [Test]
        public async void SelectMany_Happy()
        {
            var expected = 20;

            var actual = await (
                from x in Task.FromResult(10)
                from y in Task.FromResult(10)
                select x + y
                );

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async void Where_Happy()
        {
            var expected = 20;

            var actual = await (
                from x in Task.FromResult(10)
                where x > 0
                from y in Task.FromResult(10)
                select x + y
                );

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async void Where_FailingPredicate()
        {
            var expected = 20;

            var task =
                from x in Task.FromResult(10)
                where x < 0
                from y in Task.FromResult(10)
                select x + y;

            try
            {
                await task;
                Assert.Fail("Expected exception");
            }
            catch (HaxlPredicateFailedException e)
            {
                Assert.That(e.Message.Contains("x < 0"));
            }
            catch (Exception e)
            {
                Assert.Fail($"Unexpected exception: {e.Message}");
            }
        }

        [Test]
        public async void SelectMany_NotStarted()
        {
            var cancellation = new CancellationTokenSource();
            var failureWindow = 100;

            cancellation.CancelAfter(failureWindow);

            var sw = Stopwatch.StartNew();

            var expected = 20;

            var actual = await (
                from x in new Task<int>(() => 10, cancellation.Token)
                from y in new Task<int>(() => 10, cancellation.Token)
                select x + y
                );

            sw.Stop();

            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(failureWindow));
        }


        [Test]
        public async void SelectMany_Exceptions()
        {
            var runner =
                from x in Task.Factory.StartNew<int>(() =>
                {
                    Thread.Sleep(100);
                    throw new Exception("X");
                })
                from y in Task.Factory.StartNew<int>(() => { throw new Exception("Y"); })
                select x + y;

            try
            {
                await runner;
                Assert.Fail("Expecting exception");
            }
            catch (Exception e)
            {
                Assert.That(e.Message, Is.EqualTo("X"));
            }
        }

        [Test]
        public async void Select_Happy()
        {
            var expected = 20;

            var actual = await Task.FromResult(10).Select(x => x*2);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Select_Unhappy()
        {
            var expected = 20;

            var actual = Task.Run(() => {throw new Exception("Boo"); return 10;}).Select(x => x * 2);

            Assert.Throws<AggregateException>(() => actual.Wait(100));
        }
    }
}