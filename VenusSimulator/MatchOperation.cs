using System;
using System.ComponentModel;
using System.Xml.Serialization;
using ZemotoCommon.UI;

namespace VenusSimulator
{
   public enum MatchAction
   {
      [Description("Click")]
      Click,
      [Description("Don't click if matched")]
      DontClick,
      [Description("Stop clicking if matched")]
      StopClicking
   }

   [Serializable]
   public sealed class MatchOperation : ViewModelBase
   {
      public string Name { get; set; }
      public string TemplateFilePath { get; set; }
      public int SkipCountAfterMatch { get; set; } = 0;
      public MatchAction Action { get; set; } = MatchAction.Click;

      [XmlIgnore]
      public int TemplateId { get; set; }
   }
}
