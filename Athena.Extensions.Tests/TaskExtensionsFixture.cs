using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Athena.Extensions.Tests
{
    [TestFixture]
    public class TaskExtensionsFixture
    {
        [Test]
        [ExpectedException(typeof(AggregateException))]
        public void CancelableTaskTest()
        {
            int i = int.MinValue + 5;
            var task = Task.Factory.StartCancelable((ct) =>
            {
                while (i != int.MaxValue)
                {
                    ct.ThrowIfCancellationRequested();
                    i = i + 2;
                    i = i - 3;
                    ++i;
                    ++i;
                }
            });
            task.Cancel();
            try
            {
                task.Wait();
            }
            finally
            {
                Assert.That(i, Is.Not.EqualTo(int.MaxValue));
                Assert.That(task.Status, Is.EqualTo(TaskStatus.Canceled));
            }
        }
        [Test]
        [ExpectedException(typeof(AggregateException))]
        public void CancelableTaskWithResultTest()
        {
            int i = int.MinValue + 5;
            var task = Task.Factory.StartCancelable((ct) =>
            {
                while (i != int.MaxValue)
                {
                    ct.ThrowIfCancellationRequested();
                    i = i + 2;
                    i = i - 3;
                    ++i;
                    ++i;
                }
                return "Test";
            });
            task.Cancel();
            try
            {
                task.Wait();
            }
            finally
            {
                Assert.That(i, Is.Not.EqualTo(int.MaxValue));
                Assert.That(task.Status, Is.EqualTo(TaskStatus.Canceled));
            }
        }
        [Test]
        public void SimpleTimeoutCompletesTest()
        {
            var task = Task.Factory.StartCancelable((ct) => { for (int i = 0; i < 100000000; ++i); })
                .WithTimeout(TimeSpan.FromSeconds(30));
            task.Wait();

            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }
        [Test]
        public void SimpleTimeoutCompletesAndContinuesTest()
        {
            var success = false;
            var task = Task.Factory.StartCancelable((ct) => { for (int i = 0; i < 100000000; ++i); })
                .WithTimeout(TimeSpan.FromSeconds(30))
                .ContinueWith((t) => success = true, TaskContinuationOptions.OnlyOnRanToCompletion);
            task.Wait();

            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
            Assert.That(success, Is.True);
        }
        [Test]
        public void SimpleTimeoutTimesoutTest()
        {
            var task1 = Task.Factory.StartCancelable((ct) => { for (int i = 0; i < 100000000; ++i) { ct.ThrowIfCancellationRequested(); } });
            var task2 = task1.WithTimeout(TimeSpan.FromMilliseconds(30));
            try
            {
                task2.Wait();
            }
            catch (AggregateException ae)
            {
                var ex = ae.InnerExceptions.First() as TimeoutException;
                Assert.That(ex, Is.Not.Null);
            }

            Assert.That(task1.Status, Is.EqualTo(TaskStatus.Canceled));
            Assert.That(task2.Status, Is.EqualTo(TaskStatus.Faulted));
        }
        [Test]
        public void SimpleTimeoutWithResultTimesoutTest()
        {
            var task1 = Task.Factory.StartCancelable((ct) => { for (int i = 0; i < 100000000; ++i) { ct.ThrowIfCancellationRequested(); } return "success"; });
            var task2 = task1.WithTimeout(TimeSpan.FromMilliseconds(30));
            try
            {
                task2.Wait();
            }
            catch (AggregateException ae)
            {
                var ex = ae.InnerExceptions.First() as TimeoutException;
                Assert.That(ex, Is.Not.Null);
            }

            Assert.That(task1.Status, Is.EqualTo(TaskStatus.Canceled));
            Assert.That(task2.Status, Is.EqualTo(TaskStatus.Faulted));
        }
        [Test]
        public void SimpleTimeoutWithResultCompletesTest()
        {
            var task = Task.Factory.StartCancelable((ct) => { for (int i = 0; i < 100000000; ++i); return "success"; })
                .WithTimeout(TimeSpan.FromSeconds(30));
            task.Wait();

            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
            Assert.That(task.Result, Is.EqualTo("success"));
        }
    }
}
