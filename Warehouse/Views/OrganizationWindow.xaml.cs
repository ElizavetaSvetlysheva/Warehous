using System.Windows;
using System.Windows.Input;
using Warehouse.ViewModels;
using WarehouseData.Models;

namespace Warehouse.Views
{
    public partial class OrganizationWindow : Window
    {
        private OrganizationViewModel _vm;

        public Organization SelectedOrganization { get; private set; }

        public OrganizationWindow()
        {
            InitializeComponent();
            _vm = new OrganizationViewModel();
            this.DataContext = _vm;

            listOrganizations.SelectionChanged += (s, e) =>
            {
                txtSelected.Text = _vm.SelectedOrganization?.OrgName ?? "Выберите организацию";
            };
        }

        private void listOrganizations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_vm.SelectedOrganization != null)
                SelectAndClose();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedOrganization != null)
                SelectAndClose();
            else
                MessageBox.Show("Выберите организацию из списка.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void SelectAndClose()
        {
            SelectedOrganization = _vm.SelectedOrganization;

            if (Owner is MainWindow main)
                main.SelectOrganization(SelectedOrganization);

            this.DialogResult = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            SelectedOrganization = null;
            this.DialogResult = false;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog("Введите наименование организации:")
            {
                RequireNonEmpty = true,
                Owner = this
            };
            if (dialog.ShowDialog() != true) return;
            try
            {
                _vm.AddOrganization(dialog.Result);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedOrganization == null)
            {
                MessageBox.Show("Выберите организацию для редактирования.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new InputDialog("Изменить наименование:", _vm.SelectedOrganization.OrgName)
            {
                RequireNonEmpty = true,
                Owner = this
            };
            if (dialog.ShowDialog() != true) return;
            try
            {
                _vm.EditOrganization(_vm.SelectedOrganization, dialog.Result);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedOrganization == null)
            {
                MessageBox.Show("Выберите организацию для удаления.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var org = _vm.SelectedOrganization;
            var confirmDialog = new InputDialog(
                $"Для подтверждения удаления введите название организации:\n\"{org.OrgName}\"")
            {
                Owner = this
            };

            if (confirmDialog.ShowDialog() != true) return;

            if (confirmDialog.Result != org.OrgName)
            {
                MessageBox.Show("Название введено неверно. Удаление отменено.",
                    "Ошибка подтверждения", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _vm.DeleteOrganization(org);
                txtSelected.Text = "Выберите организацию";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
