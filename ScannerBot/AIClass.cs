using System;
using Torque3D;
using Torque3D.Engine.Util.Enums;
using Torque3D.Util;

namespace GameAI
{
   public class AIClass
   {
        private static bool inSight;
        private static bool tookDamage;
        private static bool playerInRange;

        private static bool prepared = false;
        private static double maxDistance;
        private static int currentDirection;
        private static int maxDistanceDirection;
        private static bool scanStarted = false;
        private static bool scanCompleted = false;
        private static int moveCount = 0;


       [ConsoleFunction]
       public static PlayerAction ScannerBot(FeatureVector vector)
       {
           tookDamage = vector.TicksSinceDamage < 15;
           inSight = vector.DamageProb > 0.0;
           playerInRange = vector.DamageProb > 0.75;

           if (inSight)
               return Attack(vector);
           if (tookDamage)
               return DodgeOpponent(vector);
           return Scanning(vector);
       }

       private static PlayerAction Scanning(FeatureVector vector)
       {
            Console.WriteLine("No sighting - scanning");

            if (!prepared)
            {
                prepared = true;
                return PlayerAction.Prepare;
            }
            prepared = false;

            if (!scanStarted)
            {
                Console.WriteLine("Initialize scanning");
                maxDistance = 0;
                scanStarted = true;
                scanCompleted = false;
                currentDirection = 0;
            }
            if (!scanCompleted)
            {
                double dist = Math.Sqrt(Math.Pow(vector.DistanceToObstacleLeft, 2) + Math.Pow(vector.DistanceToObstacleRight, 2));
                if (dist > maxDistance)
                {
                    Console.WriteLine("New max found");
                    maxDistance = dist;
                    maxDistanceDirection = currentDirection;
                }
                currentDirection++;
                if (currentDirection == 180)
                {
                    scanCompleted = true;
                    moveCount = 0;
                    currentDirection = 0;
                    Console.WriteLine("Scan completed");
                }
                return PlayerAction.TurnRight;
            }
            else
            {
                if (currentDirection == maxDistanceDirection)
                {
                    Console.WriteLine("Locked in to direction - dash forward");
                    moveCount++;
                    if (moveCount>20) scanStarted = false;
                    return PlayerAction.MoveForward;
                }
                else
                {
                    currentDirection++;
                    return PlayerAction.TurnRight;
                }
            }

        }

        private static PlayerAction Attack(FeatureVector vector)
        {
            scanStarted = false;

            if (playerInRange && vector.ShootDelay == 0)
            {
                return PlayerAction.Shoot;
            }

            if (vector.DeltaDamageProb < 0.0)
            {
                if (vector.DeltaRot > 0.0)
                {
                    return PlayerAction.TurnLeft;
                }
                return PlayerAction.TurnRight;
            }
            if (vector.DeltaRot > 0.0)
            {
                return PlayerAction.TurnRight;
            }
            return PlayerAction.TurnLeft;
        }

        private static PlayerAction DodgeOpponent(FeatureVector vector)
        {
            scanStarted = false;

            if (vector.TickCount % 3 == 0)
                return PlayerAction.MoveLeft;
            return PlayerAction.TurnLeft;
        }
   }
}