using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using ZemotoCommon.Utils;

namespace VenusSimulator
{
   internal static class OperationSerializer
   {
      private const string TemplatesDirectory = "Templates";
      private const string SavedOperationsFileName = "Conditions.xml";

      public static void SerializeOperations( List<MatchOperation> operations )
      {
         MoveTemplateFiles( operations );

         try
         {
            using ( var stream = File.Open( SavedOperationsFileName, FileMode.Create, FileAccess.Write ) )
            {
               var settings = new XmlWriterSettings
               {
                  Indent = true,
                  NewLineHandling = NewLineHandling.Entitize,
               };

               var serializer = new XmlSerializer( typeof( List<MatchOperation> ) );
               using ( var xmlWriter = XmlWriter.Create( stream, settings ) )
               {
                  serializer.Serialize( xmlWriter, operations );
                  MessageBox.Show( "Save Successful" );
               }
            }
         }
         catch
         {
            MessageBox.Show( "Could not save operations" );
         }
      }

      private static void MoveTemplateFiles( IEnumerable<MatchOperation> operations )
      {
         try
         {
            UtilityMethods.CreateDirectory( TemplatesDirectory );

            foreach ( var operation in operations )
            {
               var fileName = Path.GetFileName( operation.TemplateFilePath );
               var target = Path.Combine( TemplatesDirectory, fileName );

               if ( !string.IsNullOrEmpty( operation.TemplateFilePath ) )
               {
                  File.Copy( operation.TemplateFilePath, target );
                  operation.TemplateFilePath = target;

               }
            }
         }
         catch
         {
            // Ignore
         }
      }

      public static List<MatchOperation> DeserializeOperations()
      {
         if ( !File.Exists( SavedOperationsFileName ) )
         {
            return null;
         }

         try
         {
            var serializer = new XmlSerializer( typeof( List<MatchOperation> ) );
            using ( var stream = File.Open( SavedOperationsFileName, FileMode.Open, FileAccess.Read ) )
            {
               return serializer.Deserialize( stream ) as List<MatchOperation>;
            }
         }
         catch
         {
            MessageBox.Show( "Could not load operations" );
         }

         return null;
      }
   }
}