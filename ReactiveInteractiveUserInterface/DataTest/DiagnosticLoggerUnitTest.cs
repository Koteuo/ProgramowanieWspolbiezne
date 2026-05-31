using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DiagnosticLoggerUnitTest
    {

        private class MockBall : IBall
        {
            public double Mass { get; } = 1.0;
            public double Radius { get; } = 5.0;
            public IVector Position { get; set; } = new Vector(10.5, 20.5);
            public IVector Velocity { get; set; } = new Vector(1.0, -1.0);
            public event EventHandler<IVector>? NewPositionNotification;
        }

        [TestMethod]
        public void Logger_ShouldWriteDataToFile_WhenDisposed()
        {
            string tempFilePath = Path.GetTempFileName();
            DiagnosticLogger logger = new DiagnosticLogger(tempFilePath);
            MockBall testBall = new MockBall();

            logger.LogBallState(testBall);
            logger.LogBallState(testBall);

            logger.Dispose();

            string[] writtenLines = File.ReadAllLines(tempFilePath);

            Assert.AreEqual(2, writtenLines.Length, "Logger powinien zapisać dokładnie dwie linie tekstu.");

            Assert.IsTrue(writtenLines[0].Contains("PositionX"), "Zapisany log nie ma właściwego formatu JSON.");
            Assert.IsTrue(writtenLines[0].Contains("10.5"), "Log nie zawiera prawidłowej pozycji kuli.");

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }
}