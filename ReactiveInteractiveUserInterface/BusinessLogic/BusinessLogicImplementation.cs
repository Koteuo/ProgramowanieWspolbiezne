//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private CancellationTokenSource? _cancelTokenSource;
        private readonly UnderneathLayerAPI layerBellow;

        public BusinessLogicImplementation() : this(null) { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer ?? UnderneathLayerAPI.GetDataLayer();
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            _cancelTokenSource = new CancellationTokenSource();

            layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
            {
                Ball logicBall = new Ball(databall);
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);

                // Uruchamiamy niezależne zadanie (wątek) dla każdej kulki
                Task.Run(() => MoveLoop(logicBall, _cancelTokenSource.Token));
            });
        }

        private async Task MoveLoop(Ball ball, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ball.Move();
                // Częstotliwość odświeżania (ok. 50 FPS)
                await Task.Delay(20);
            }
        }

        public override void Stop()
        {
            _cancelTokenSource?.Cancel();
            layerBellow.Stop();
        }

        public override void Dispose()
        {
            Stop();
            layerBellow.Dispose();
        }
    }
}