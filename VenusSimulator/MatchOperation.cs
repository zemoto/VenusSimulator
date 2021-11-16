using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using ZemotoUI;

namespace VenusSimulator
{
   public enum MatchAction
   {
      [Description( "Click" )]
      Click = 0,
      [Description( "Pause clicking if matched" )]
      PauseClicking = 1,
      [Description( "Stop clicking if matched" )]
      StopClicking = 2
   }

   [Serializable]
   public sealed class MatchOperation : ViewModelBase
   {
      public string Name { get; set; }
      public string TemplateFilePath { get; set; }
      public int SkipCountAfterMatch { get; set; }
      public MatchAction Action { get; set; } = MatchAction.Click;

      private bool _isEnabled = true;
      public bool IsEnabled
      {
         get => _isEnabled;
         set => SetProperty( ref _isEnabled, value );
      }

      [XmlIgnore]
      public int TemplateId { get; set; }

      [XmlIgnore]
      public string TemplateFileName => Path.GetFileName( TemplateFilePath );
   }
}
