using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class DiagnosticLogger : IDiagnosticLogger
    {
        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _loggingTask;
        private readonly string _filePath;

        public DiagnosticLogger(string filePath = "diagnostics.json")
        {
            _filePath = filePath;
            _loggingTask = Task.Run(ProcessQueue);
        }

        public void LogBallState(IBall ball)
        {
            var logData = new
            {
                Timestamp = DateTime.Now.ToString("O"),
                PositionX = ball.Position.x,
                PositionY = ball.Position.y,
                VelocityX = ball.Velocity.x,
                VelocityY = ball.Velocity.y
            };

            string jsonLog = JsonSerializer.Serialize(logData);
            _logQueue.Enqueue(jsonLog);
        }


        private async Task ProcessQueue()
        {
    
            using StreamWriter writer = new StreamWriter(_filePath, append: true, System.Text.Encoding.ASCII);
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                 
                    if (_logQueue.TryDequeue(out string logEntry))
                    {
                        await writer.WriteLineAsync(logEntry);
                    }
                    else
                    {
                      
                        await Task.Delay(20, _cts.Token);
                    }
                }
            }
            catch (TaskCanceledException) { }

            
            while (_logQueue.TryDequeue(out string logEntry))
            {
                await writer.WriteLineAsync(logEntry);
            }
        }

        public void Dispose()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _loggingTask.Wait();
                _cts.Dispose();
            }
        }
    }
}