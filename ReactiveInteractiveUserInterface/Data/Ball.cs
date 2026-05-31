//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly IDiagnosticLogger _logger;
        private readonly int _updateInterval = 15;

        public double Mass { get; }
        public double Radius { get; }

        public Ball(Vector initialPosition, Vector initialVelocity, double mass, double radius, IDiagnosticLogger logger)
        {
            _position = initialPosition;
            Velocity = initialVelocity;
            Mass = mass;
            Radius = radius;
            _logger = logger;

            _cancelTokenSource = new CancellationTokenSource();
            Task.Run(MoveLoop);
        }

        private async Task MoveLoop()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                while (!_cancelTokenSource.Token.IsCancellationRequested)
                {
                    double timeElapsed = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();

                    double nextX = Position.x + (Velocity.x * timeElapsed);
                    double nextY = Position.y + (Velocity.y * timeElapsed);

                    Position = new Vector(nextX, nextY);

                    await Task.Delay(_updateInterval, _cancelTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        public void Dispose()
        {
            if (!_cancelTokenSource.IsCancellationRequested)
            {
                _cancelTokenSource.Cancel();
            }
        }

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        private IVector _position;
        public IVector Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;

                    NewPositionNotification?.Invoke(this, _position);
                    _logger?.LogBallState(this);
                }
            }
        }
    }
}