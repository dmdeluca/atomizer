using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Atomizer.Tests
{
    public class ThreadingOperationsTests
    {
        [Fact]
        public void Test1()
        {
            var atomic = Atomic.Struct(0);
            var threads = new Thread[10000];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var value = atomic.Get();
                    atomic.Set(x => x + 1);
                    System.Console.WriteLine($"Setting value to {value + 1}");
                });
                threads[i].Start();
            }
            foreach (var thread in threads)
                thread.Join();

            Assert.Equal(10000, atomic.Get());
        }

        [Fact]
        public void Test2()
        {
            var atomic = Atomic.Instance(new MyClass(0));
            var threads = new Thread[10000];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    atomic.Do(x => x.Data++);
                });
                threads[i].Start();
            }
            foreach (var thread in threads)
                thread.Join();

            Assert.Equal(10000, atomic.Get(x => x.Data));
        }

        [Fact]
        public void Test3()
        {
            // arrange
            var dictionary = Atomic.Instance(new Dictionary<int, int>());
            var threads = new Thread[200];
            const int increment = 5;
            const int expectedItemCount = 20;

            // act
            for (int i = 0; i < threads.Length; i++)
            {
                int copy = i;
                threads[i] = new Thread(() =>
                {
                    dictionary.Do(x =>
                    {
                        int key = copy / expectedItemCount;
                        if (!x.ContainsKey(key))
                        {
                            Thread.Sleep(50);
                            // This will throw if two threads get here.
                            x.Add(key, 0);
                        }
                        else
                        {
                            x[key] += increment;
                        }
                    });
                });
            }
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();

            // assert
            Assert.Equal(threads.Length / expectedItemCount, dictionary.Get(x => x.Count));
            Assert.All(dictionary.Get(x => x.AsEnumerable()), x =>
              {
                  Assert.Equal((expectedItemCount - 1) * increment, x.Value);
              });
        }

        [Fact]
        public void Test4()
        {
            // arrange
            var dictionary = new Dictionary<int, int>();
            var tasks = new Task[200];
            const int increment = 5;
            const int expectedGroupCount = 20;

            var agg = Assert.Throws<AggregateException>(() =>
            {
                // act
                for (int i = 0; i < tasks.Length; i++)
                {
                    int copy = i;
                    tasks[i] = new Task(() =>
                    {
                        int key = copy / expectedGroupCount;
                        if (!dictionary.ContainsKey(key))
                        {
                            Thread.Sleep(50);
                            // This will throw if two threads get here.
                            dictionary.Add(key, 0);
                        }
                        else
                        {
                            dictionary[key] += increment;
                        }
                    });
                }
                foreach (var task in tasks)
                    task.Start();
                foreach (var task in tasks)
                    task.Wait();
            });

            Assert.Contains("An item with the same key has already been added.", agg.InnerExceptions.First().Message);
        }
    }

    internal class MyClass
    {
        int _data;

        public MyClass(int data)
        {
            Data = data;
        }

        public int Data { get => _data; set => _data = value; }
    }
}

