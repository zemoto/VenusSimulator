using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace VenusSimulator
{
   internal static class NativeMethods
   {
      [StructLayout( LayoutKind.Sequential )]
      public struct POINT
      {
         public int x;
         public int y;
      }

      [Flags]
      internal enum MOUSEEVENTF : uint
      {
         LEFTDOWN = 0x0002,
         LEFTUP = 0x0004,
      }

      internal struct INPUT
      {
         public uint Type;
         public MOUSEKEYBDHARDWAREINPUT Data;
      }

      [StructLayout( LayoutKind.Explicit )]
      internal struct MOUSEKEYBDHARDWAREINPUT
      {
         [FieldOffset( 0 )]
         public MOUSEINPUT Mouse;
      }

#pragma warning disable CS0649  
      internal struct MOUSEINPUT
      {
         public int X;
         public int Y;
         public uint MouseData;
         public MOUSEEVENTF Flags;
         public uint Time;
         public IntPtr ExtraInfo;
      }
#pragma warning restore CS0649

      [DllImport( "user32.dll" )]
      public static extern uint SendInput( uint nInputs, [MarshalAs( UnmanagedType.LPArray ), In] INPUT[] pInputs, int cbSize );

      [DllImport( "user32.dll" )]
      [return: MarshalAs( UnmanagedType.Bool )]
      public static extern bool SetCursorPos( int x, int y );

      [DllImport( "user32.dll" )]
      [return: MarshalAs( UnmanagedType.Bool )]
      public static extern bool GetCursorPos( ref POINT pt );
   }
}
