 using System;
using Torque3D;
using Torque3D.Engine.Util.Enums;
using Torque3D.Util;

namespace GameAI
{
   public class AIClass
   {
       private static bool inPursuit;
       private static bool tookDamage;
       private static bool playerInRange;

       private static bool aiTurningRight = true;
       private static Random rnd = new Random();


       [ConsoleFunction]
       public static PlayerAction Kurt(FeatureVector vector)
       {
           tookDamage = vector.TicksSinceDamage < 15;
           inPursuit = vector.DamageProb > 0.0;
           playerInRange = vector.DamageProb > 0.75;

           if (inPursuit)
               return PursueOpponent(vector);
           if (tookDamage)
               return DodgeOpponent(vector);
           return PlayerAction.TurnRight;
           return SearchForOpponent(vector);
       }

       private static PlayerAction SearchForOpponent(FeatureVector vector)
       {
            Console.WriteLine("No sighting");

            if (vector.DistanceToObstacleLeft < 4 || vector.DistanceToObstacleRight < 4)
            {
                if (vector.DeltaRot != 0)
                {
                    Console.WriteLine("Obstacle - moving forward");
                    return PlayerAction.MoveForward;
                }
                if (aiTurningRight)
                {
                    Console.WriteLine("Obstacle - turning right");
                    return PlayerAction.TurnRight;
                }
                Console.WriteLine("Obstacle - turning left");
                return PlayerAction.TurnLeft;
            }
            aiTurningRight = rnd.Next(0, 1) == 1;
            int r = rnd.Next(0, 9);
            switch (r)
            {
                case 0:
                    Console.WriteLine("Random left turn");
                    return PlayerAction.TurnLeft;
                case 1:
                    Console.WriteLine("Random right turn");
                    return PlayerAction.TurnRight;
                default:
                    Console.WriteLine("Moving forward");
                    return PlayerAction.MoveForward;
            }
        }

    private static PlayerAction PursueOpponent(FeatureVector vector)
       {
           if (playerInRange && vector.ShootDelay == 0)
               return PlayerAction.Shoot;

           if (tookDamage && vector.TicksSinceDamage > 12)
               return PlayerAction.MoveLeft;

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
           if (vector.TickCount % 3 == 0)
               return PlayerAction.MoveLeft;
           return PlayerAction.TurnLeft;
       }
   }
}