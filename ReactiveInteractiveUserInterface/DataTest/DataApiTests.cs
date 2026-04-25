using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class DataApiTests
    {
        [TestMethod]
        public void Start_ShouldCreateSpecifiedNumberOfBalls()
        {
            DataAbstractAPI dataLayer = DataAbstractAPI.GetDataLayer();
            int expectedBallsCount = 5;
            int actualBallsCount = 0;

            dataLayer.Start(expectedBallsCount, (vector, ball) =>
            {
                actualBallsCount++;
                Assert.IsNotNull(vector);
                Assert.IsNotNull(ball);
            });

            Assert.AreEqual(expectedBallsCount, actualBallsCount, "Warstwa danych nie utworzyła oczekiwanej liczby kulek.");
        }

        [TestMethod]
        public void Stop_ShouldClearBallsList()
        {
            DataAbstractAPI dataLayer = DataAbstractAPI.GetDataLayer();
            dataLayer.Start(3, (v, b) => { });

            dataLayer.Stop();
        }
    }
}