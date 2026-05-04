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
using System.Collections.Generic;
using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private readonly UnderneathLayerAPI layerBellow;

        private readonly List<Data.IBall> _allDataBalls = new List<Data.IBall>();

        private readonly object _collisionLock = new object();

		private bool Disposed = false;

		public BusinessLogicImplementation() : this(null) { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer ?? UnderneathLayerAPI.GetDataLayer();
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            lock (_collisionLock)
            {
                _allDataBalls.Clear();

                layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
                {
                    _allDataBalls.Add(databall);

                    Ball logicBall = new Ball(databall);

                    databall.NewPositionNotification += (sender, args) => CheckCollisions(databall);

                    upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
                });
            }
        }

        private void CheckCollisions(Data.IBall movingBall)
        {
            lock (_collisionLock)
            {
                CheckWallCollisions(movingBall);

                foreach (var otherBall in _allDataBalls)
                {
                    if (movingBall == otherBall) continue;

                    if (AreBallsColliding(movingBall, otherBall))
                    {
                        ResolveCollision(movingBall, otherBall);
                    }
                }
            }
        }

        private void CheckWallCollisions(Data.IBall ball)
        {
            var dims = GetDimensions;
            double nextX = ball.Position.x;
            double nextY = ball.Position.y;
            double velX = ball.Velocity.x;
            double velY = ball.Velocity.y;

            bool bounced = false;

            if (nextX <= 0 && velX < 0)
            {
                nextX = 0;
                velX = -velX;
                bounced = true;
            }
            else if (nextX >= (dims.TableWidth - dims.BallDimension) && velX > 0)
            {
                nextX = dims.TableWidth - dims.BallDimension;
                velX = -velX;
                bounced = true;
            }

            if (nextY <= 0 && velY < 0)
            {
                nextY = 0;
                velY = -velY;
                bounced = true;
            }
            else if (nextY >= (dims.TableHeight - dims.BallDimension) && velY > 0)
            {
                nextY = dims.TableHeight - dims.BallDimension;
                velY = -velY;
                bounced = true;
            }

            if (bounced)
            {
                ball.Velocity = new LogicVector(velX, velY);
                ball.Position = new LogicVector(nextX, nextY);
            }
        }

        private bool AreBallsColliding(Data.IBall b1, Data.IBall b2)
        {
            double radius = GetDimensions.BallDimension / 2;

            double centerX1 = b1.Position.x + radius;
            double centerY1 = b1.Position.y + radius;
            double centerX2 = b2.Position.x + radius;
            double centerY2 = b2.Position.y + radius;

            double distance = Math.Sqrt(Math.Pow(centerX1 - centerX2, 2) + Math.Pow(centerY1 - centerY2, 2));

            if (distance <= (radius * 2))
            {
                double dotProduct = (centerX2 - centerX1) * (b2.Velocity.x - b1.Velocity.x) +
                                    (centerY2 - centerY1) * (b2.Velocity.y - b1.Velocity.y);
                return dotProduct < 0;
            }
            return false;
        }

        private void ResolveCollision(Data.IBall b1, Data.IBall b2)
        {
            double m1 = 1.0;
            double m2 = 1.0;

            double newVelX1 = (b1.Velocity.x * (m1 - m2) + (2 * m2 * b2.Velocity.x)) / (m1 + m2);
            double newVelY1 = (b1.Velocity.y * (m1 - m2) + (2 * m2 * b2.Velocity.y)) / (m1 + m2);

            double newVelX2 = (b2.Velocity.x * (m2 - m1) + (2 * m1 * b1.Velocity.x)) / (m1 + m2);
            double newVelY2 = (b2.Velocity.y * (m2 - m1) + (2 * m1 * b1.Velocity.y)) / (m1 + m2);

            b1.Velocity = new LogicVector(newVelX1, newVelY1);
            b2.Velocity = new LogicVector(newVelX2, newVelY2);
        }

        public override void Stop()
        {
            lock (_collisionLock)
            {
                _allDataBalls.Clear();
            }

            layerBellow.Stop();
        }

        public override void Dispose()
        {
            Stop();
            layerBellow.Dispose();
			Disposed = true;
		}
		[Conditional("DEBUG")]
		internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
		{
			returnInstanceDisposed(Disposed);
		}


	}

    internal record LogicVector(double x, double y) : Data.IVector;
}