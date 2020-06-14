using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace VenusSimulator
{
   internal sealed class AutoClicker
   {
      private readonly ImageDetector _detector;
      private readonly Timer _timer = new Timer();
      private readonly object _timerLock = new object();

      private IEnumerable<MatchOperation> _operations;
      private Dictionary<MatchOperation, int> _skipOperations;
      private bool _stopped;

      public EventHandler StopSignalDetected;

      public AutoClicker( ImageDetector detector )
      {
         _detector = detector;

         _timer.Elapsed += OnTimerEllapsed;
         _timer.AutoReset = false;
      }

      public void Start( IEnumerable<MatchOperation> operations )
      {
         _operations = operations;
         _skipOperations = new Dictionary<MatchOperation, int>();

         lock ( _timerLock )
         {
            _stopped = false;
            StartTimer();
         }
      }

      public void Stop()
      {
         lock ( _timerLock )
         {
            _stopped = true;
            _timer.Stop();
         }
      }

      private void StartTimer()
      {
         _timer.Interval = 1000 + new Random( 324325 ).Next( 2000 );
         _timer.Start();
      }

      private async void OnTimerEllapsed( object sender, ElapsedEventArgs e )
      {
         bool shouldClick = true;
         var clickLocation = new Point( SystemParameters.VirtualScreenWidth / 2, SystemParameters.VirtualScreenHeight / 2 );

         var skippedOperations = _skipOperations.Keys.ToList();
         foreach ( var operation in skippedOperations )
         {
            if ( _skipOperations[operation] <= 0 )
            {
               _skipOperations.Remove( operation );
            }
            else
            {
               _skipOperations[operation] -= 1;
            }
         }

         var templateIds = _operations.Where( x => !_skipOperations.ContainsKey( x ) ).Select( x => x.TemplateId ).ToArray();

         if ( templateIds.Any() )
         {
            var ( foundId, location ) = await _detector.DetectImagesAsync( templateIds );
            if ( foundId != -1 )
            {
               var operation = _operations.First( x => x.TemplateId == foundId );
               if ( operation.Action == MatchAction.StopClicking )
               {
                  StopSignalDetected?.Invoke( this, EventArgs.Empty );
                  return;
               }

               if ( operation.SkipCountAfterMatch > 0 )
               {
                  _skipOperations[operation] = operation.SkipCountAfterMatch;
               }

               if ( operation.Action == MatchAction.Click )
               {
                  clickLocation = new Point( location.X, location.Y );
               }
               else
               {
                  shouldClick = false;
               }
            }
         }

         if ( shouldClick )
         {
            RealisticMouseMover.MoveMouse( clickLocation.X, clickLocation.Y );
            await SendMouseClickAsync();
         }

         lock ( _timerLock )
         {
            if ( !_stopped && !_timer.Enabled )
            {
               StartTimer();
            }
         }
      }

      private static async Task SendMouseClickAsync()
      {
         var inputMouseDown = new NativeMethods.INPUT { Type = 0 };
         inputMouseDown.Data.Mouse.Flags = NativeMethods.MOUSEEVENTF.LEFTDOWN;

         var inputs = new[] { inputMouseDown };
         NativeMethods.SendInput( (uint)inputs.Length, inputs, Marshal.SizeOf( typeof( NativeMethods.INPUT ) ) );

         await Task.Delay( new Random().Next( 100 ) );

         var inputMouseUp = new NativeMethods.INPUT { Type = 0 };
         inputMouseUp.Data.Mouse.Flags = NativeMethods.MOUSEEVENTF.LEFTUP;

         inputs = new[] { inputMouseUp };
         NativeMethods.SendInput( (uint)inputs.Length, inputs, Marshal.SizeOf( typeof( NativeMethods.INPUT ) ) );
      }
   }
}