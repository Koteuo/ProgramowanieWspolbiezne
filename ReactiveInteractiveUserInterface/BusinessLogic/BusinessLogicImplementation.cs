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

        // Lista wszystkich kul potrzebna do sprawdzania zderzeń między nimi
        private readonly List<Data.IBall> _allDataBalls = new List<Data.IBall>();

        // Obiekt synchronizacji - sekcja krytyczna (zapobiega wyścigom)
        private readonly object _collisionLock = new object();

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
                    // Główny wątek dodaje kule...
                    _allDataBalls.Add(databall);

                    Ball logicBall = new Ball(databall);

                    databall.NewPositionNotification += (sender, args) => CheckCollisions(databall);

                    upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
                });
            }
        }

        // --- OBSŁUGA ZDERZEŃ ---

        private void CheckCollisions(Data.IBall movingBall)
        {
            // Zabezpieczenie przed modyfikacją tej samej kuli przez dwa różne wątki naraz
            lock (_collisionLock)
            {
                // 1. Najpierw sprawdzamy odbicia od ścian
                CheckWallCollisions(movingBall);

                // 2. Następnie zderzenia z innymi kulami
                foreach (var otherBall in _allDataBalls)
                {
                    if (movingBall == otherBall) continue; // Pomijamy sprawdzanie kuli samej ze sobą

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

            // Ściany X - ZABEZPIECZENIE: Odbijamy tylko, jeśli kulka leci w stronę ściany (velX < 0 lub velX > 0)
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

            // Ściany Y - ZABEZPIECZENIE: analogicznie dla osi Y
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

            // Środki kul
            double centerX1 = b1.Position.x + radius;
            double centerY1 = b1.Position.y + radius;
            double centerX2 = b2.Position.x + radius;
            double centerY2 = b2.Position.y + radius;

            // Odległość euklidesowa między środkami
            double distance = Math.Sqrt(Math.Pow(centerX1 - centerX2, 2) + Math.Pow(centerY1 - centerY2, 2));

            // Jeśli odległość jest mniejsza lub równa sumie promieni, kule się stykają
            if (distance <= (radius * 2))
            {
                // Zabezpieczenie przed sklejaniem: upewniamy się, że kule lecą ku sobie (iloczyn skalarny < 0)
                double dotProduct = (centerX2 - centerX1) * (b2.Velocity.x - b1.Velocity.x) +
                                    (centerY2 - centerY1) * (b2.Velocity.y - b1.Velocity.y);
                return dotProduct < 0;
            }
            return false;
        }

        private void ResolveCollision(Data.IBall b1, Data.IBall b2)
        {
            // Przyjmujemy równe masy dla uproszczenia pierwszego etapu symulacji zderzeń
            double m1 = 1.0;
            double m2 = 1.0;

            // Wymiana pędu (zderzenie idealnie sprężyste 2D dla równych mas sprowadza się do uśrednienia i zamiany)
            double newVelX1 = (b1.Velocity.x * (m1 - m2) + (2 * m2 * b2.Velocity.x)) / (m1 + m2);
            double newVelY1 = (b1.Velocity.y * (m1 - m2) + (2 * m2 * b2.Velocity.y)) / (m1 + m2);

            double newVelX2 = (b2.Velocity.x * (m2 - m1) + (2 * m1 * b1.Velocity.x)) / (m1 + m2);
            double newVelY2 = (b2.Velocity.y * (m2 - m1) + (2 * m1 * b1.Velocity.y)) / (m1 + m2);

            // Aplikacja nowych wektorów prędkości
            b1.Velocity = new LogicVector(newVelX1, newVelY1);
            b2.Velocity = new LogicVector(newVelX2, newVelY2);
        }

        public override void Stop()
        {
            // Najpierw bezpiecznie czyścimy listę referencji w logice
            lock (_collisionLock)
            {
                _allDataBalls.Clear();
            }

            // Potem przekazujemy komendę do warstwy niżej, by ubiła Taski
            layerBellow.Stop();
        }

        public override void Dispose()
        {
            Stop();
            layerBellow.Dispose();
        }
    }

    // Pomocniczy rekord do tworzenia wektorów w warstwie logiki
    internal record LogicVector(double x, double y) : Data.IVector;
}