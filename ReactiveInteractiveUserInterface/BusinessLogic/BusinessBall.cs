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

namespace TP.ConcurrentProgramming.BusinessLogic
{
	internal class Ball : IBall
	{
		private readonly Data.IBall _dataBall;

		public Ball(Data.IBall ball)
		{
			_dataBall = ball;

			_dataBall.NewPositionNotification += RaisePositionChangeEvent;
			_dataBall.ColorChangedNotification += RaiseColorChangeEvent;
		}


		public event EventHandler<IPosition>? NewPositionNotification;
		public event EventHandler<string>? ColorChangedNotification;


		public string Color => _dataBall.Color;

		private void RaisePositionChangeEvent(object? sender, Data.IVector e)
		{
			NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
		}

		private void RaiseColorChangeEvent(object? sender, string newColor)
		{
			ColorChangedNotification?.Invoke(this, newColor);
		}

		internal void Move()
		{
			var dims = BusinessLogicAbstractAPI.GetDimensions;

			double nextX = _dataBall.Position.x + _dataBall.Velocity.x;
			double nextY = _dataBall.Position.y + _dataBall.Velocity.y;

			double velX = _dataBall.Velocity.x;
			double velY = _dataBall.Velocity.y;

			if (nextX <= 0)
			{
				nextX = 0;
				velX = -velX;
			}
			else if (nextX >= (dims.TableWidth - dims.BallDimension))
			{
				nextX = dims.TableWidth - dims.BallDimension;
				velX = -velX;
			}

			if (nextY <= 0)
			{
				nextY = 0;
				velY = -velY;
			}
			else if (nextY >= (dims.TableHeight - dims.BallDimension))
			{
				nextY = dims.TableHeight - dims.BallDimension;
				velY = -velY;
			}

			_dataBall.Velocity = new LogicVector(velX, velY);
			_dataBall.Position = new LogicVector(nextX, nextY);
		}
	}
}