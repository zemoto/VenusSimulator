using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using ZemotoUtils;

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
         UtilityMethods.CreateDirectory( TemplatesDirectory );

         foreach ( var operation in operations )
         {
            if ( FileUtils.CopyFileToFolder( operation.TemplateFilePath, TemplatesDirectory, out var newPath ) )
            {
               operation.TemplateFilePath = newPath;
            }
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