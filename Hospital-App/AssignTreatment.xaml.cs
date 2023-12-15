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
using System.Windows.Shapes;

namespace Hospital_App
{
    /// <summary>
    /// Interaction logic for AssignTreatment.xaml
    /// </summary>
    public partial class AssignTreatment : Window
    {
        public AssignTreatment()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

            // close the window
            this.Close();

        }
    }
}
