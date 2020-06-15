using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace MyPasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            ImageClock.Visibility = Visibility.Hidden;

            SetWindowLayout();

            TimerStart.Interval = TimeSpan.FromSeconds(1);
            TimerStart.Tick += TimerStart_Tick;
        }

     

        private void SetWindowLayout()
        {
            string w = GetSetting("Width");

            if (w != "")
                WindowMyPasswordManager.Width = Convert.ToDouble(w);
            else
                WindowMyPasswordManager.Width = System.Windows.SystemParameters.PrimaryScreenWidth;


            string h = GetSetting("Height");

            if (h != "")
                WindowMyPasswordManager.Height = Convert.ToDouble(h);
            else
                WindowMyPasswordManager.Height= System.Windows.SystemParameters.PrimaryScreenHeight/4.0;
          
            string t = GetSetting("Top");

            if (t != "")
                WindowMyPasswordManager.Top = Convert.ToDouble(t);

            string l = GetSetting("Left");

            if (l != "")
                WindowMyPasswordManager.Left = Convert.ToDouble(l);

        }





        private void WindowMyPasswordManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetSetting("Height", WindowMyPasswordManager.ActualHeight.ToString());
            SetSetting("Width", WindowMyPasswordManager.ActualWidth.ToString());
        }

        private void WindowMyPasswordManager_LocationChanged(object sender, EventArgs e)
        {
            SetSetting("Top", WindowMyPasswordManager.Top.ToString());
            SetSetting("Left", WindowMyPasswordManager.Left.ToString());
        }

        private void TextBoxFilter_KeyUp(object sender, KeyEventArgs e)
        {
            PwDatToListView(TextBoxFilter.Text);


        }

        private void ButtonFilterClear_Click(object sender, RoutedEventArgs e)
        {
            TextBoxFilter.Text = "";
            PwDatToListView("");
        }

        public static bool DoExport = false;
        public static bool DoImport = false;
        private void ButtonExImport_Click(object sender, RoutedEventArgs e)
        {
            WindowExImport winExIm = new WindowExImport(TextBoxUser.Text, MyPasswordBox.Password);
            bool? result = winExIm.ShowDialog();



            if (result != true)
                return;

            string temp = System.IO.Path.GetTempPath();

            if (DoExport)
            {
                MessageBox.Show("Die Daten werden in lesbarer Form (TXT-Datei) ausgegeben\n" +
                                "Bitte die Datei\n" +
                                "PASSWORT.TXT\n" +
                                "aus dem angezeigten Ordner nach Gebrauch\n" +
                                "sofort löschen !!! ");


                string tempPath = System.IO.Path.GetTempPath();


                try
                {
                    using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(tempPath, "PASSWORT.TXT")))
                    {
                        foreach (CPwDat item in ListPw)
                        {
                            if (item.Title.StartsWith("_") && item.Opt2.StartsWith("_"))
                                continue;
                            string line = item.Title + ";" + item.WebAdr + ";" + item.User + ";" + item.PW + ";" + item.Opt1 + ";" + item.Opt2;
                            outputFile.WriteLine(line);
                        }
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Fehler beim Schreiben von PASSWORT.TXT");
                }

                if (Directory.Exists(temp))
                    Process.Start("explorer.exe", tempPath);

            }



            if (DoImport)
            {

                MessageBoxResult doImport = 
                MessageBox.Show("Die importiere Datei überschreibt alle Daten\nWirklich importieren ?", "Achtung", MessageBoxButton.YesNo, MessageBoxImage.Warning);


                if (doImport != MessageBoxResult.Yes)
                    return;

                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                ofd.ShowDialog();

                if (ofd.FileName == "")
                    return;

                SimpleTxtToListPw(ofd.FileName);

                PwDatToListView("");

                EncryptFile();

            }


        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            Point position = Mouse.GetPosition(ImagePw);
            TextBoxFilter.Text = position.ToString();
        }

        private void CheckBoxOnTop_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxOnTop.IsChecked==true)
                WindowMyPasswordManager.Topmost = true;
            else
                WindowMyPasswordManager.Topmost = false;

        }

        private void ButtonTitleOut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, TextBoxTitelOut.Text);
        }

        private void ButtonWebAdrOut_Click(object sender, RoutedEventArgs e)
        {
            //Clipboard.SetData(DataFormats.Text, TextBoxWebAdrOut.Text);

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(TextBoxWebAdrOut.Text)
            {
                UseShellExecute = true
            };

            p.Start();
       
        }

        private void ButtonUserOut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, TextBoxUserOut.Text);
        }

        private void ButtonPwOut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, TextBoxPwOut.Text);
        }

        private void ButtonOpt1Out_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, TextBoxOpt1Out.Text);
        }

        private void ButtonOpt2Out_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, TextBoxOpt2Out.Text);
        }

        private void MyPasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonLogin_Click(null, null);
            }
        }

        private void ButtonDataCount_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", PmPath);
        }

        private void ImagePw_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(ImagePw);

            double xd = ImagePw.ActualWidth / 44;
            double yd = ImagePw.ActualHeight/ 12;

            int x = (int)(position.X / xd);
            int y = (int)(position.Y / yd);


            if (LoginImage.Length == 12)
                LoginImage = "";

            
            LoginImage += x.ToString("00") + y.ToString("00");

            MyPasswordBox.Password = LoginImage;

            //String loginImage = LoginImage + "                      ";
            //ButtonLogin.Content = loginImage[0..4] + " " + loginImage[4..8] + " " + loginImage[8..12] ;
        
        }

 

  

        private void ButtonChangeDirMain_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = 
            MessageBox.Show("Um einen neuen Ablageort einzurichten wird das Programm runtergefahren\n" +
                            "Es muss dann neu gestartet werden\n\nSoll dies ausgeführt werden ?", "Achtung", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            SetSetting("PmPath", "");
            Environment.Exit(0);
        }
    }
}
