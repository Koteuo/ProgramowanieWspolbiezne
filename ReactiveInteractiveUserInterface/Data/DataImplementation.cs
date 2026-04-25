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

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {
      MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
    }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition = new(random.Next(5, 95), random.Next(5, 95));
                double velocityX = (random.NextDouble() * 5) - 2.5;
                double velocityY = (random.NextDouble() * 5) - 2.5;
                Vector initialVelocity = new Vector(velocityX, velocityY);

                Ball newBall = new(startingPosition, initialVelocity);
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
                    MoveTimer.Dispose();
                    // ZABEZPIECZENIE CZYSZCZENIA
                    lock (_ballsLock)
                    {
                        BallsList.Clear();
                    }
                }
                Disposed = true;
            }
        }

        #endregion DataAbstractAPI

        #region IDisposable



		public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    //private bool disposedValue;
    private bool Disposed = false;

    private readonly Timer MoveTimer;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];

        private readonly object _ballsLock = new object();

        private void Move(object? x)
        {
            // ZABEZPIECZENIE ITERACJI
            lock (_ballsLock)
            {
                foreach (Ball item in BallsList)
                {
                    item.Move();
                }
            }
        }

        #endregion private

        #region TestingInfrastructure

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

    #endregion TestingInfrastructure
  }
}