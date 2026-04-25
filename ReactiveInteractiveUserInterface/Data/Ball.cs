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
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly int _updateInterval = 15; // ms (ok. 60 FPS)

        public double Mass { get; }
        public double Radius { get; }

        public Ball(Vector initialPosition, Vector initialVelocity, double mass, double radius)
        {
            // PRZYPISANIE DO POLA: Zapobiega wywoływaniu eventu przed podpięciem subskrybentów
            _position = initialPosition;
            Velocity = initialVelocity;
            Mass = mass;
            Radius = radius;

            _cancelTokenSource = new CancellationTokenSource();
            Task.Run(MoveLoop);
        }

        private async Task MoveLoop()
        {
            try
            {
                while (!_cancelTokenSource.Token.IsCancellationRequested)
                {
                    // Przesunięcie kuli (Ruch kuli bazuje wyłącznie na jej aktualnej prędkości)
                    Position = new Vector(Position.x + Velocity.x, Position.y + Velocity.y);

                    // Czekamy określoną liczbę milisekund (asynchronicznie)
                    await Task.Delay(_updateInterval, _cancelTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // Zignoruj wyjątek. Jest to naturalne zachowanie przy zamykaniu programu (Stop).
            }
        }

        public void Dispose()
        {
            // Bezpieczne zatrzymanie wątku
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
                if (_position != value) // Drobna optymalizacja: wywołaj event tylko jeśli faktycznie coś się zmieniło
                {
                    _position = value;
                    NewPositionNotification?.Invoke(this, _position);
                }
            }
        }
    }
}