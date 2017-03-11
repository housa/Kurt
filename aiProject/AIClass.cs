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

       [ConsoleFunction]
       public static PlayerAction Kurt(FeatureVector vector)
       {
           tookDamage = vector.TicksSinceDamage < 15;
           inPursuit = vector.DamageProb > 0.0;
           playerInRange = vector.DamageProb > 0.8;

           if (inPursuit)
               return PursueOpponent(vector);
           if (tookDamage)
               return DodgeOpponent(vector);
           return SearchForOpponent(vector);
       }

       private static PlayerAction SearchForOpponent(FeatureVector vector)
       {
           return PlayerAction.TurnLeft;
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
               return PlayerAction.TurnLeft;
           return PlayerAction.MoveLeft;
       }
   }
}