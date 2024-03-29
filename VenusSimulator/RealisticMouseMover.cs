﻿using System;
using System.Threading;

namespace VenusSimulator
{
   internal static class RealisticMouseMover
   {
      private const double MouseMovementWind = 1;
      private const int MouseSpeedConstant = 15;
      private const int MouseTargetDrift = 20;

      private static readonly Random Rand = new();

      public static void MoveMouse( double x, double y )
      {
         var current = new NativeMethods.POINT();
         _ = NativeMethods.GetCursorPos( ref current );

         x += Rand.Next( MouseTargetDrift * 2 ) - MouseTargetDrift;
         y += Rand.Next( MouseTargetDrift * 2 ) - MouseTargetDrift;

         AnimateMouseMove( current.x, current.y, x, y );
      }

      private static void AnimateMouseMove( double startX, double startY, double endX, double endY )
      {
         double randomSpeed = ( ( Rand.Next( MouseSpeedConstant ) / 2.0 ) + MouseSpeedConstant ) / 10.0;
         double minWait = 10.0 / randomSpeed;
         double maxWait = 15.0 / randomSpeed;
         double maxStep = 10.0 * randomSpeed;
         double targetArea = 10.0 * randomSpeed;
         double wind = MouseMovementWind;

         double windX = 0, windY = 0;
         double velocityX = 0, velocityY = 0;
         int newX = (int)Math.Round( startX );
         int newY = (int)Math.Round( startY );

         double waitDiff = maxWait - minWait;
         const double sqrt2 = 1.41421356237;
         const double sqrt3 = 1.73205080757;
         const double sqrt5 = 2.2360679775;
         const double gravity = 9.0;

         var dist = Hypotenuse( endX - startX, endY - startY );

         while ( dist > 1.0 )
         {
            wind = Math.Min( wind, dist );

            if ( dist >= targetArea )
            {
               int w = Rand.Next( ( (int)Math.Round( wind ) * 2 ) + 1 );
               windX = ( windX / sqrt3 ) + ( ( w - wind ) / sqrt5 );
               windY = ( windY / sqrt3 ) + ( ( w - wind ) / sqrt5 );
            }
            else
            {
               windX /= sqrt2;
               windY /= sqrt2;
               if ( maxStep < 3 )
               {
                  maxStep = Rand.Next( 3 ) + 3.0;
               }
               else
               {
                  maxStep /= sqrt5;
               }
            }

            velocityX += windX;
            velocityY += windY;
            velocityX += gravity * ( endX - startX ) / dist;
            velocityY += gravity * ( endY - startY ) / dist;

            if ( Hypotenuse( velocityX, velocityY ) > maxStep )
            {
               var randomDist = ( maxStep / 2.0 ) + Rand.Next( (int)Math.Round( maxStep ) / 2 );
               var veloMag = Hypotenuse( velocityX, velocityY );
               velocityX = velocityX / veloMag * randomDist;
               velocityY = velocityY / veloMag * randomDist;
            }

            var oldX = (int)Math.Round( startX );
            var oldY = (int)Math.Round( startY );
            startX += velocityX;
            startY += velocityY;
            dist = Hypotenuse( endX - startX, endY - startY );
            newX = (int)Math.Round( startX );
            newY = (int)Math.Round( startY );

            if ( oldX != newX || oldY != newY )
            {
               _ = NativeMethods.SetCursorPos( newX, newY );
            }

            var step = Hypotenuse( startX - oldX, startY - oldY );
            int wait = (int)Math.Round( ( waitDiff * ( step / maxStep ) ) + minWait );
            Thread.Sleep( wait );
         }

         int finalX = (int)Math.Round( endX );
         int finalY = (int)Math.Round( endY );
         if ( finalX != newX || finalY != newY )
         {
            _ = NativeMethods.SetCursorPos( finalX, finalY );
         }
      }

      private static double Hypotenuse( double dx, double dy ) => Math.Sqrt( ( dx * dx ) + ( dy * dy ) );
   }
}
