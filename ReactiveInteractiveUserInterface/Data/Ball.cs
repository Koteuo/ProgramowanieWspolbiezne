//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
        }

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        private Vector Position;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            double nextX = Position.x + delta.x;
            double nextY = Position.y + delta.y;

            double logicalBoardWidth = 100;
            double logicalBoardHeight = 100;
            double logicalBallSize = 2;

            if (nextX <= 0 || nextX >= (logicalBoardWidth - logicalBallSize))
            {
                nextX = Position.x;
            }

            if (nextY <= 0 || nextY >= (logicalBoardHeight - logicalBallSize))
            {
                nextY = Position.y;
            }

            Position = new Vector(nextX, nextY);
            RaiseNewPositionChangeNotification();
        }
    }
}