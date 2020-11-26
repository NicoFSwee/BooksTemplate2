using Books.Wpf.Common;
using Books.Wpf.ViewModels;
using System.Windows;

namespace Books.Wpf
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
      var controller = new WindowController();
      var viewModel = await MainWindowViewModel.Create(controller);
      controller.ShowWindow(viewModel);
    }
  }
}
