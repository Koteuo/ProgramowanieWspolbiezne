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
        Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
        Ball newBall = new(startingPosition, startingPosition);
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);

				//// TYMCZASOWY KOD TESTOWY (ignorujemy numberOfBalls i wstawiamy na sztywno):

				//// Kulka 1: Lewy górny róg stołu (z uwzględnieniem np. 10px promienia)
				//Vector pos1 = new Vector(0, 0);
				//Ball ball1 = new Ball(pos1, new Vector(0, 0)); // Prędkość ustawiamy na 0
				//upperLayerHandler(pos1, ball1);
				//BallsList.Add(ball1);

				//// Kulka 2: Środek stołu (zakładając stół 400x420)
				//Vector pos2 = new Vector(200, 210);
				//Ball ball2 = new Ball(pos2, new Vector(0, 0));
				//upperLayerHandler(pos2, ball2);
				//BallsList.Add(ball2);

				//// Kulka 3: Prawy dolny róg (blisko krawędzi)
				//Vector pos3 = new Vector(392, 372);
				//Ball ball3 = new Ball(pos3, new Vector(0, 0));
				//upperLayerHandler(pos3, ball3);
				//BallsList.Add(ball3);
			}
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          MoveTimer.Dispose();
          BallsList.Clear();
        }
        Disposed = true;
      }
    }

		public override void Stop()
		{
			BallsList.Clear();
		}

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

    private void Move(object? x)
    {
            foreach (Ball item in BallsList)
            {
                double deltaX = (RandomGenerator.NextDouble() - 0.5) * 10;
                double deltaY = (RandomGenerator.NextDouble() - 0.5) * 10;



                item.Move(new Vector(deltaX, deltaY));
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