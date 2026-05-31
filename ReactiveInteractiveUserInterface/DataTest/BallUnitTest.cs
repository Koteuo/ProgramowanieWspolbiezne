//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        internal class FakeDiagnosticLogger : IDiagnosticLogger
        {
            public int LogCount { get; private set; } = 0;

            public void LogBallState(IBall ball)
            {
                LogCount++;
            }

            public void Dispose() { }
        }

        [TestMethod]
        public async Task Ball_ShouldMoveAndLog_WhenTimePasses()
        {
            Vector startPos = new Vector(0.0, 0.0);
            Vector velocity = new Vector(100.0, 50.0);
            FakeDiagnosticLogger fakeLogger = new FakeDiagnosticLogger();

        
            Ball newInstance = new Ball(startPos, velocity, 5.0, 10.0, fakeLogger);


            await Task.Delay(150);

            Assert.IsTrue(newInstance.Position.x > 0.0, "Pozycja X powinna wzrosnąć.");
            Assert.IsTrue(newInstance.Position.y > 0.0, "Pozycja Y powinna wzrosnąć.");

            Assert.IsTrue(fakeLogger.LogCount > 0, "Kula powinna wysłać dane do loggera.");

            newInstance.Dispose();
        }
    }
}