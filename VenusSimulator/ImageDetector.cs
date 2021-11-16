using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;
using ZemotoUtils;
using Point = System.Drawing.Point;

namespace VenusSimulator
{
   internal sealed class ImageDetector
   {
      private readonly List<Image<Gray, byte>> _templates = new List<Image<Gray, byte>>();

      public int LoadTemplate( string filePath )
      {
         try
         {
            var image = new Image<Gray, byte>( filePath );
            _templates.Add( image );

            return _templates.Count - 1;
         }
         catch ( ArgumentException )
         {
            return -1;
         }
      }

      public void RemoveTemplate( int templateId )
      {
         var template = _templates[templateId];
         template.Dispose();

         _templates.RemoveAt( templateId );
      }

      private Point IsImageOnScreen( int templateId )
      {
         using ( var desktop = CaptureScreen() )
         {
            using ( var result = desktop.MatchTemplate( _templates[templateId], Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed ) )
            {
               result.MinMax( out _, out var maxValues, out _, out var maxLocations );
               return maxValues[0] >= Properties.Settings.Default.MatchTolerance ? maxLocations[0] : Point.Empty;
            }
         }
      }

      public async Task<(int, Point)> DetectImagesAsync( int[] templateIds )
      {
         var tasks = new List<Task<Point>>();

         templateIds.ForEach( id => tasks.Add( Task.Run( () => IsImageOnScreen( id ) ) ) );

         var results = await Task.WhenAll( tasks ).ConfigureAwait( false );

         for ( int i = 0; i < results.Length; i++ )
         {
            if ( results[i] == Point.Empty )
            {
               continue;
            }

            var foundImage = _templates[templateIds[i]];
            var location = new Point( results[i].X + foundImage.Width / 2, results[i].Y + foundImage.Height / 2 );
            return (templateIds[i], location);
         }

         return (-1, Point.Empty);
      }

      private Image<Gray, byte> CaptureScreen()
      {
         var desktopSize = new System.Drawing.Size( (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight );

         using ( var desktop = new Bitmap( desktopSize.Width, desktopSize.Height, PixelFormat.Format24bppRgb ) )
         {
            using ( var graphics = Graphics.FromImage( desktop ) )
            {
               graphics.CopyFromScreen( 0, 0, 0, 0, desktopSize, CopyPixelOperation.SourceCopy );
            }

            return desktop.ToImage<Gray, byte>();
         }
      }
   }
}