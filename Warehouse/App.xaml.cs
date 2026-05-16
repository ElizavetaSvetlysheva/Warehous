using System.Windows;
using Warehouse.Views;
using WarehouseData.Context;

namespace Warehouse
{
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ApplicationContext.Initialize();

            var orgWindow = new OrganizationWindow();
            orgWindow.ShowDialog();

            if (orgWindow.SelectedOrganization == null)
            {
                Shutdown();
                return;
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var mainWindow = new MainWindow();
            Current.MainWindow = mainWindow;
            mainWindow.Show();
            mainWindow.SelectOrganization(orgWindow.SelectedOrganization);
        }
    }
}
