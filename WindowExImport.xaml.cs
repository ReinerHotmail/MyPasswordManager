using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyPasswordManager
{
    /// <summary>
    /// Interaktionslogik für WindowExImport.xaml
    /// </summary>
    public partial class WindowExImport : Window
    {
        string user;
        string pw;

        public WindowExImport(string user,string pw)
        {
            this.user = user;
            this.pw = pw;
            MainWindow.DoExport = false;
            MainWindow.DoImport = false;

            InitializeComponent();
        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxUserHlp.Text!=user || MyPasswordBoxHlp.Password!=pw)
            {
                MessageBox.Show("User/Passwort ist falsch");
                return;
            }

            MainWindow.DoExport = true;
            DialogResult = true;



        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxUserHlp.Text != user || MyPasswordBoxHlp.Password != pw)
            {
                MessageBox.Show("User/Passwort ist falsch");
                return;
            }

            MainWindow.DoImport = true;
            DialogResult = true;
        }
    }
}
