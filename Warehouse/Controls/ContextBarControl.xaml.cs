using System;
using System.Windows;
using System.Windows.Controls;

namespace Warehouse.Controls
{
    public partial class ContextBarControl : UserControl
    {
        
        public event RoutedEventHandler SelectOrganizationRequested;

        public ContextBarControl()
        {
            InitializeComponent();
        }

        public void SetOrganization(string orgName)
        {
            txtOrg.Text = orgName ?? "Организация не выбрана";
        }

        public void SetWarehouse(string whName)
        {
            txtWh.Text = whName ?? "не выбран";
        }

        private void btnSelectOrg_Click(object sender, RoutedEventArgs e)
        {
            SelectOrganizationRequested?.Invoke(this, e);
        }
    }
}
