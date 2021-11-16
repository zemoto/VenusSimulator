using Microsoft.Win32;
using System.Windows;
using ZemotoUI;

namespace VenusSimulator
{
   internal partial class CreateOperationWindow
   {
      private readonly ImageDetector _detector;

      public MatchOperation CreatedOperation { get; private set; }

      public CreateOperationWindow( ImageDetector detector )
      {
         _detector = detector;

         InitializeComponent();
      }

      private void OnTemplateFilePathButtonClick( object sender, RoutedEventArgs e )
      {
         var dlg = new OpenFileDialog
         {
            Filter = "BMP Template Files (*.bmp)|*.bmp",
            Multiselect = false
         };

         if ( dlg.ShowDialog( Application.Current.MainWindow ) == true )
         {
            TemplateFilePathTextBox.Text = dlg.FileName;
            CreateButton.IsEnabled = !string.IsNullOrEmpty( TemplateFilePathTextBox.Text );
         }
      }

      private void OnCreateClick( object sender, RoutedEventArgs e )
      {
         var operation = new MatchOperation();

         var id = _detector.LoadTemplate( TemplateFilePathTextBox.Text );
         if ( id != -1 )
         {
            operation.Name = NameTextBox.Text;
            operation.TemplateFilePath = TemplateFilePathTextBox.Text;
            operation.TemplateId = id;
            operation.Action = (MatchAction)( (BoundEnumMember)ActionComboBox.SelectedItem ).Value;

            if ( !string.IsNullOrEmpty( SkipCountTextBox.Text ) )
            {
               operation.SkipCountAfterMatch = int.Parse( SkipCountTextBox.Text );
            }

            CreatedOperation = operation;

            DialogResult = true;
         }
         else
         {
            MessageBox.Show( "Could not load selected template file" );
         }
      }

      private void OnCancelClick( object sender, RoutedEventArgs e )
      {
         DialogResult = false;
      }
   }
}
