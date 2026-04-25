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
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        #endregion IBall

        #region private

        private Vector Position;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move()
        {
            // 1. Wyliczenie nowej pozycji poprzez dodanie wektora prędkości
            // Zakładamy, że Velocity.x to "siła i zwrot" w osi X, a Velocity.y w osi Y
            double nextX = Position.x + Velocity.x;
            double nextY = Position.y + Velocity.y;

            double logicalBoardWidth = 100;
            double logicalBoardHeight = 100;
            double logicalBallSize = 2;

            // 2. Obsługa odbicia od lewej lub prawej ściany (oś X)
            if (nextX <= 0)
            {
                nextX = 0; // Wyrównanie do krawędzi
                Velocity = new Vector(-Velocity.x, Velocity.y); // Odwrócenie zwrotu X
            }
            else if (nextX >= (logicalBoardWidth - logicalBallSize))
            {
                nextX = logicalBoardWidth - logicalBallSize;
                Velocity = new Vector(-Velocity.x, Velocity.y);
            }

            // 3. Obsługa odbicia od górnej lub dolnej ściany (oś Y)
            if (nextY <= 0)
            {
                nextY = 0;
                Velocity = new Vector(Velocity.x, -Velocity.y); // Odwrócenie zwrotu Y
            }
            else if (nextY >= (logicalBoardHeight - logicalBallSize))
            {
                nextY = logicalBoardHeight - logicalBallSize;
                Velocity = new Vector(Velocity.x, -Velocity.y);
            }

            // 4. Aktualizacja pozycji i powiadomienie subskrybentów
            Position = new Vector(nextX, nextY);
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}