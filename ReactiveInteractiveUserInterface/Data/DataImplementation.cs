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
using System.Collections.Generic;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        public DataImplementation()
        {
        }

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                // Inicjalizacja stanu początkowego - tylko wygenerowanie danych
                Vector startingPosition = new Vector(random.Next(5, 95), random.Next(5, 95));
                double velocityX = (random.NextDouble() * 5) - 2.5;
                double velocityY = (random.NextDouble() * 5) - 2.5;
                Vector initialVelocity = new Vector(velocityX, velocityY);

                Ball newBall = new Ball(startingPosition, initialVelocity);
                upperLayerHandler(startingPosition, newBall);

                // ZABEZPIECZENIE DODAWANIA
                lock (_ballsLock)
                {
                    BallsList.Add(newBall);
                }
            }
        }

        public override void Stop()
        {
            // ZABEZPIECZENIE CZYSZCZENIA
            lock (_ballsLock)
            {
                BallsList.Clear();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // ZABEZPIECZENIE CZYSZCZENIA
                    lock (_ballsLock)
                    {
                        BallsList.Clear();
                    }
                }
                Disposed = true;
            }
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool Disposed = false;
        private List<Ball> BallsList = new List<Ball>();
        private readonly object _ballsLock = new object();

        // -------------------------------------------------------------
        // Metody testowe z atrybutem Conditional
        // -------------------------------------------------------------

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }
    }
}