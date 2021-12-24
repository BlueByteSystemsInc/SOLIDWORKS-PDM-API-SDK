using System.Windows;
using System.Windows.Input;

namespace ProjectTemplateWizard
{
    /// <summary>
    /// Interaction logic for WizardWindow.xaml
    /// </summary>
    public partial class WizardWindow : Window
    {
        public AddInInfo AddInInfo { get; set; } = new AddInInfo();

        public WizardWindow()
        {
            InitializeComponent();
            DataContext = AddInInfo;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
