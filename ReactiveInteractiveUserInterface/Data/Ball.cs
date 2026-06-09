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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
	internal class Ball : IBall
	{
		private readonly CancellationTokenSource _cancelTokenSource;
		private readonly IDiagnosticLogger _logger;
		private readonly int _updateInterval = 15;

		// Dodajemy Timer do sterowania kolorem
		private readonly Timer _colorChangeTimer;
		private static readonly Random _random = new Random();

		public double Mass { get; }
		public double Radius { get; }

		// Właściwość przechowująca obecny kolor
		public string Color { get; private set; }

		// Zdarzenie powiadamiające logikę o zmianie koloru (sygnał)
		public event EventHandler<string>? ColorChangedNotification;

		public Ball(Vector initialPosition, Vector initialVelocity, double mass, double radius, IDiagnosticLogger logger)
		{
			_position = initialPosition;
			Velocity = initialVelocity;
			Mass = mass;
			Radius = radius;
			_logger = logger;
			Color = "#FFFFFFFF"; // Domyślny kolor startowy (np. biały)

			// Konfiguracja timera: 
			// 1. ZmianaKoloru - metoda do wywołania (sygnał)
			// 2. null - dodatkowe dane (niepotrzebne)
			// 3. 0 - opóźnienie startu (od razu)
			// 4. 2000 - interwał w czasie rzeczywistym (np. co 2000 ms / 2 sekundy)
			_colorChangeTimer = new Timer(ChangeColorSignal, null, 0, 2000);

			_cancelTokenSource = new CancellationTokenSource();
			Task.Run(MoveLoop);
		}

		// Metoda wywoływana "sygnałowo" przez systemowy Timer
		private void ChangeColorSignal(object? state)
		{
			// Przykładowa paleta kolorów (kody HEX dla WPF)
			string[] colors = { "#FFFF0000", "#FF00FF00", "#FF0000FF", "#FFFFFF00", "#FFFF00FF" };

			string newColor = colors[_random.Next(colors.Length)];

			if (Color != newColor)
			{
				Color = newColor;
				// Wysłanie sygnału wyżej, że kolor się zmienił
				ColorChangedNotification?.Invoke(this, Color);
			}
		}

		private async Task MoveLoop()
		{
			try
			{
				while (!_cancelTokenSource.Token.IsCancellationRequested)
				{
					double nextX = Position.x + Velocity.x;
					double nextY = Position.y + Velocity.y;

					Position = new Vector(nextX, nextY);

					await Task.Delay(_updateInterval, _cancelTokenSource.Token);
				}
			}
			catch (TaskCanceledException)
			{

			}
		}

		public void Dispose()
		{
			if (!_cancelTokenSource.IsCancellationRequested)
			{
				_cancelTokenSource.Cancel();
			}

			_colorChangeTimer?.Dispose();
		}

		public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        private IVector _position;
        public IVector Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;

                    NewPositionNotification?.Invoke(this, _position);
                    _logger?.LogBallState(this);
                }
            }
        }
    }
}