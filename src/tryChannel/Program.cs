using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace tryChannel
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            #region producer

            //var channel = new MyChannel<int>();
            var channel = Channel.CreateUnbounded<int>();

            _ = Task.Run(async delegate
            {
                for (int i = 0; ; i++)
                {
                    await Task.Delay(1000);
                    channel.Writer.TryWrite(i);
                }
            });
            #endregion

            #region consumer

            while (true)
            {
                Console.WriteLine(await channel.Reader.ReadAsync());
            }

            #endregion

        }

        public class MyChannel<T>
        {
            public ConcurrentQueue<T> queue { get; set; } = new ConcurrentQueue<T>();
            public SemaphoreSlim semaphore { get; set; } = new SemaphoreSlim(0);

            public void Write(T item)
            {
                queue.Enqueue(item);
                semaphore.Release();
            }

            public async ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)

            {
                await semaphore.WaitAsync();
                bool getOne = queue.TryDequeue(out T item);

                return item;
            }
        }


    }

}
