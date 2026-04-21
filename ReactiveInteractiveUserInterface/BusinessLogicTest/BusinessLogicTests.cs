using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogicTest
{
	internal class FakeDataLayer : DataAbstractAPI
	{
		public int StartCalledCount = 0;
		public int RequestedBalls = 0;

		public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
		{
			StartCalledCount++;
			RequestedBalls = numberOfBalls;

			for (int i = 0; i < numberOfBalls; i++)
			{
				upperLayerHandler(new FakeVector(), new FakeBall());
			}
		}

		public override void Stop() { }
		public override void Dispose() { }
	}
	internal class FakeVector : IVector
	{
		public double x { get; init; } = 50;
		public double y { get; init; } = 50;
	}

	internal class FakeBall : Data.IBall
	{
		public double Radius => 10;
		public IVector Velocity { get; set; }
		public event EventHandler<IVector> NewPositionNotification;
	}

	[TestClass]
	public class BusinessLogicTests
	{
		[TestMethod]
		public void Start_ShouldPassExecutionToDataLayerAndMapResults()
		{
			FakeDataLayer fakeDataLayer = new FakeDataLayer();

			BusinessLogicImplementation logicLayer = new BusinessLogicImplementation(fakeDataLayer);

			int mappedBallsCount = 0;

			logicLayer.Start(5, (position, logicBall) =>
			{
				mappedBallsCount++;

				Assert.IsNotNull(position);
				Assert.IsNotNull(logicBall);
				Assert.AreEqual(50, position.x);
			});
			Assert.AreEqual(1, fakeDataLayer.StartCalledCount, "Metoda Start z warstwy Data nie została wywołana.");
			Assert.AreEqual(5, fakeDataLayer.RequestedBalls, "Logika przekazała złą liczbę kulek do warstwy Data.");

			Assert.AreEqual(5, mappedBallsCount, "Logika nie zwróciła w górę utworzonych kulek.");
		}
	}
}