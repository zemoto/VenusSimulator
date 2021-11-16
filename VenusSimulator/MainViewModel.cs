using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ZemotoUI;

namespace VenusSimulator
{
   internal sealed class MainViewModel : ViewModelBase
   {
      private readonly MainWindow _window;
      private readonly ImageDetector _detector = new ImageDetector();
      private readonly AutoClicker _clicker;

      public MainViewModel( MainWindow window )
      {
         _window = window;
         _window.Activated += OnWindowActivated;

         _clicker = new AutoClicker( _detector );
         _clicker.StopSignalDetected += OnStopSignalDetected;
      }

      public ObservableCollection<MatchOperation> Operations { get; } = new ObservableCollection<MatchOperation>();

      private bool _running;
      public bool Running
      {
         get => _running;
         set => SetProperty( ref _running, value );
      }

      private RelayCommand _loadOperationsCommand;
      public RelayCommand LoadOperationsCommand => _loadOperationsCommand ?? ( _loadOperationsCommand = new RelayCommand( () =>
      {
         var operations = OperationSerializer.DeserializeOperations();
         if ( operations == null )
         {
            return;
         }

         Operations.Clear();
         foreach ( var operation in operations )
         {
            var id = _detector.LoadTemplate( operation.TemplateFilePath );
            if ( id != -1 )
            {
               operation.TemplateId = id;
               Operations.Add( operation );
            }
         }
      } ) );

      private RelayCommand _saveOperationsCommand;
      public RelayCommand SaveOperationsCommand => _saveOperationsCommand ?? ( _saveOperationsCommand = new RelayCommand( () => OperationSerializer.SerializeOperations( Operations.ToList() ) ) );

      private RelayCommand _createOperationCommand;
      public RelayCommand CreateOperationCommand => _createOperationCommand ?? ( _createOperationCommand = new RelayCommand( () =>
      {
         var dlg = new CreateOperationWindow( _detector ) { Owner = _window };
         if ( dlg.ShowDialog() == true )
         {
            Operations.Add( dlg.CreatedOperation );
         }
      } ) );

      private RelayCommand<MatchOperation> _deleteOperationCommand;
      public RelayCommand<MatchOperation> DeleteOperationCommand => _deleteOperationCommand ?? ( _deleteOperationCommand = new RelayCommand<MatchOperation>( x =>
      {
         Operations.Remove( x );
         _detector.RemoveTemplate( x.TemplateId );
      } ) );

      private RelayCommand<MatchOperation> _increasePriorityCommand;
      public RelayCommand<MatchOperation> IncreasePriorityCommand => _increasePriorityCommand ?? ( _increasePriorityCommand = new RelayCommand<MatchOperation>( x =>
      {
         var currentIndex = Operations.IndexOf( x );
         if ( currentIndex != 0 )
         {
            Operations.Move( currentIndex, currentIndex - 1 );
         }
      } ) );

      private RelayCommand _startCommand;
      public RelayCommand StartCommand => _startCommand ?? ( _startCommand = new RelayCommand( () =>
      {
         _window.WindowState = WindowState.Minimized;

         _clicker.Start( Operations.Where( x => x.IsEnabled ) );
         Running = true;

      }, () => Operations.Any() ) );

      private void OnStopSignalDetected( object sender, EventArgs e )
      {
         Application.Current.Dispatcher.Invoke( () => _window.WindowState = WindowState.Normal );
      }

      private void OnWindowActivated( object sender, EventArgs e )
      {
         _clicker.Stop();
         Running = false;
      }
   }
}
