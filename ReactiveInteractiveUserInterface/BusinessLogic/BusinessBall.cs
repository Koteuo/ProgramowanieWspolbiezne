//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private readonly Data.IBall _dataBall;

        public Ball(Data.IBall ball)
        {
            _dataBall = ball;
            _dataBall.NewPositionNotification += RaisePositionChangeEvent;
        }

        public event EventHandler<IPosition>? NewPositionNotification;

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }

        internal void Move()
        {
            var dims = BusinessLogicAbstractAPI.GetDimensions;

            double nextX = _dataBall.Position.x + _dataBall.Velocity.x;
            double nextY = _dataBall.Position.y + _dataBall.Velocity.y;

            double velX = _dataBall.Velocity.x;
            double velY = _dataBall.Velocity.y;

            // Odbicia w osi X
            if (nextX <= 0)
            {
                nextX = 0; // Wyrównanie do lewej ściany
                velX = -velX; // Odbicie
            }
            else if (nextX >= (dims.TableWidth - dims.BallDimension))
            {
                nextX = dims.TableWidth - dims.BallDimension; // Wyrównanie do prawej ściany
                velX = -velX; // Odbicie
            }

            // Odbicia w osi Y
            if (nextY <= 0)
            {
                nextY = 0; // Wyrównanie do górnej ściany
                velY = -velY; // Odbicie
            }
            else if (nextY >= (dims.TableHeight - dims.BallDimension))
            {
                nextY = dims.TableHeight - dims.BallDimension; // Wyrównanie do dolnej ściany
                velY = -velY; // Odbicie
            }

            // Przypisanie nowych wartości
            _dataBall.Velocity = new LogicVector(velX, velY);
            _dataBall.Position = new LogicVector(nextX, nextY);
        }
    }
}