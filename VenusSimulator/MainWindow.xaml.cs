namespace VenusSimulator
{
   internal partial class MainWindow
   {
      public MainWindow()
      {
         DataContext = new MainViewModel( this );

         InitializeComponent();
      }
   }
}
