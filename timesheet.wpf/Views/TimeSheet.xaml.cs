using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using timesheet.wpf.ViewModel;

namespace timesheet.wpf.Views
{
    /// <summary>
    /// Interaction logic for TimeSheet.xaml
    /// </summary>
    public partial class TimeSheet : UserControl
    {
        EmployeeViewModel viewModel;
        public TimeSheet(EmployeeViewModel vm)
        {
            viewModel = vm;
            this.DataContext = viewModel;
            InitializeComponent();
        }
        private void GoBackToList_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new EmployeeList();
        }
        private void NavDaily_Click(object sender, RoutedEventArgs e)
        {
            Daily.Visibility = Visibility.Visible;
            Weekly.Visibility = Visibility.Collapsed;
            SheetTitle.Text = "Daily";
        }
        private void NavWeekly_Click(object sender, RoutedEventArgs e)
        {
            Daily.Visibility = Visibility.Collapsed;
            Weekly.Visibility = Visibility.Visible;
            SheetTitle.Text = "Weekly";
        }

    }
}
