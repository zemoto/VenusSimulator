namespace VenusSimulator
{
   public partial class MainWindow
   {
      public MainWindow()
      {
         DataContext = new MainViewModel( this );

         InitializeComponent();
      }
   }
}
