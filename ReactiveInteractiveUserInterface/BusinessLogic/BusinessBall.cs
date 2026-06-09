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

			// Subskrypcja zdarzeń z warstwy Data
			_dataBall.NewPositionNotification += RaisePositionChangeEvent;
			_dataBall.ColorChangedNotification += RaiseColorChangeEvent; // NOWE: Nasłuchiwanie na zmianę koloru
		}

		// Zdarzenia udostępniane wyżej (do warstwy Presentation/ViewModel)
		public event EventHandler<IPosition>? NewPositionNotification;
		public event EventHandler<string>? ColorChangedNotification; // NOWE: Sygnał dla warstwy wyższej

		// Aktualny kolor pobierany z warstwy danych (przydatne do inicjalizacji)
		public string Color => _dataBall.Color;

		private void RaisePositionChangeEvent(object? sender, Data.IVector e)
		{
			NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
		}

		// NOWE: Metoda przekazująca sygnał o zmianie koloru wyżej
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

			// Odbicia w osi X
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

			// Odbicia w osi Y
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

			// Przypisanie nowych wartości
			_dataBall.Velocity = new LogicVector(velX, velY);
			_dataBall.Position = new LogicVector(nextX, nextY);
		}
	}
}