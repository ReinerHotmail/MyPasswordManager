using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyPasswordManager
{
    public partial class MainWindow
    {
        private void WindowMyPasswordManager_Loaded(object sender, RoutedEventArgs e)
        {
            StackPanelUser.Visibility = Visibility.Visible;
            StackPanelPath.Visibility = Visibility.Visible;
            StackPanelMainLeft.Visibility = Visibility.Collapsed;
            StackPanelMainLeft2.Visibility = Visibility.Collapsed;
            StackPanelMainRight.Visibility = Visibility.Collapsed;
            ButtonDataCount.Visibility = Visibility.Collapsed;
            ButtonChangeDirMain.Visibility = Visibility.Collapsed;
            StackPanelFilter.Visibility = Visibility.Collapsed;
            StackPanelHelp.Visibility = Visibility.Collapsed;
            ListViewPwDat.Visibility = Visibility.Collapsed;
            ImagePw.Visibility= Visibility.Visible; 


            ButtonPath.Visibility = Visibility.Hidden;
            TextBoxPath.Visibility = Visibility.Hidden;

        }

        private void ImagePw_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point position = Mouse.GetPosition(ImagePw);

            double xd = ImagePw.ActualWidth / 44;
            double yd = ImagePw.ActualHeight / 12;

            int x = (int)(position.X / xd);
            int y = (int)(position.Y / yd);


            if (LoginImage.Length == 12)
                LoginImage = "";


            LoginImage += x.ToString("00") + y.ToString("00");

            MyPasswordBox.Password = LoginImage;

            //String loginImage = LoginImage + "                      ";
            //ButtonLogin.Content = loginImage[0..4] + " " + loginImage[4..8] + " " + loginImage[8..12] ;

        }
      
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {

            if (!CheckUsePwLen(true))
                return;

            ImagePw.Visibility = Visibility.Hidden;

            PmPath = GetSetting("PmPath");
            if (PmPath == "" || !File.Exists(PmPath + "\\" + "MyPW.txt"))
            {
                MessageBox.Show("PasswordFile '\n" + PmPath + "\\" + "  'MyPW.txt' " + "\nkann nicht gefunden werden\n\n" +
                                "- Mit 'Change Directory' einen Ordner wählen, in dem die 'MyPW.txt'-Datei des Users gespeichert ist\n" +
                                "oder\n" +
                                "- Mit 'Change Directory'  einen leeren Ordners anwählen, es wird eine neue 'MyPW.txt'-Datei erstellt");

                ButtonPath.Visibility = Visibility.Visible;
                TextBoxPath.Visibility = Visibility.Visible;
                return;
            }

            TextBoxPath.Text = PmPath + "\\" + "MyPW.txt";
            TextBoxPath.Visibility = Visibility.Visible;
            TextBoxPath.Background = System.Windows.Media.Brushes.LightGreen;
            ButtonDataCount.Visibility = Visibility.Visible;
            ButtonChangeDirMain.Visibility = Visibility.Visible;
            ImageClock.Visibility = Visibility.Visible;
            TimerLoginStart.Start();

        }


        private bool CheckUsePwLen(bool msgYes)
        {
            if (TextBoxUser.Text.Length < 2 || TextBoxUser.Text.Length > 12)
            {
                if (!msgYes)
                    return false;
                MessageBox.Show("Eingabe im LOGIN:\n'User'  muss 2 bis 12 Zeichen haben");
                return false;
            }
            if (MyPasswordBox.Password.Length < 8 || MyPasswordBox.Password.Length > 16)
            {
                if (!msgYes)
                    return false;
                MessageBox.Show("Eingabe im LOGIN:\n'Password'  muss 8 bis 16 Zeichen haben");
                return false;
            }
            return true;
        }

        private void Login()
        {
            ImageClock.Visibility = Visibility.Hidden;


            if (DecryptFile())
            {

                StackPanelUser.Visibility = Visibility.Collapsed;
                StackPanelPath.Visibility = Visibility.Collapsed;
                StackPanelMainLeft.Visibility = Visibility.Visible;
                StackPanelMainLeft2.Visibility = Visibility.Visible;
                StackPanelMainRight.Visibility = Visibility.Visible;
                ButtonDataCount.Visibility = Visibility.Visible;
                ButtonChangeDirMain.Visibility = Visibility.Visible;
                StackPanelFilter.Visibility = Visibility.Visible;
                StackPanelHelp.Visibility = Visibility.Visible;
                ListViewPwDat.Visibility = Visibility.Visible;
             

                PwDatToListView("");

            }

        }

        private void PwDatToListView(string filterIn)
        {
            string filter = filterIn.ToUpper();
            int underlineCount = 0;

            ListViewPwDat.Items.Clear();

            ListPw.Sort();

            if (filter == "")
            {
                foreach (CPwDat item in ListPw)
                {
                    if (item.Title.StartsWith("_") && item.Opt2.StartsWith("_"))  //erster Random-Datensatz
                    {
                        underlineCount++;
                        continue;
                    }

                    ListViewPwDat.Items.Add(new CPwDat { Title = item.Title, User = item.User, PW = item.PW, Opt1 = item.Opt1, Opt2 = item.Opt2, WebAdr = item.WebAdr });
                }
            }
            else
            {
                foreach (CPwDat item in ListPw)
                {
                    if (item.ToString().ToUpper().Contains(filter))
                    {
                        if (item.Title.StartsWith("_") && item.Opt2.StartsWith("_"))  //erster Random-Datensatz
                        {
                            underlineCount++;
                            continue;
                        }
                        ListViewPwDat.Items.Add(new CPwDat { Title = item.Title, User = item.User, PW = item.PW, Opt1 = item.Opt1, Opt2 = item.Opt2, WebAdr = item.WebAdr });
                    }

                }
            }

            ButtonDataCount.Content = "Records: "+ (ListPw.Count - underlineCount).ToString();

        }

        private void TimerLoginStart_Tick(object sender, EventArgs e)
        {
            TimerLoginStart.Stop();
            Login();
        }

        private void ButtonPath_Click(object sender, RoutedEventArgs e)
        {

            if (!CheckUsePwLen(true))
                return;

            PmPath = GetPath();


            if (PmPath=="")
            {
                MessageBox.Show("Kein  Ordner ausgewählt - Bitte wiederholen");
                return;
            }
            else
            {
                SetSetting("PmPath", PmPath);
            }
            SelectPasswordFile();

        }

        private void SelectPasswordFile()
        {

            TextBoxPath.Text = PmPath + "\\" + "MyPW.txt";

            if (!File.Exists(PmPath + "\\" + "MyPW.txt"))
            {
                TextBoxPath.Background = System.Windows.Media.Brushes.LightPink;

                MessageBoxResult result =
                MessageBox.Show("Die PasswordDatei 'MyPW.txt'\nexisiert noch nicht\nNeu erstellen mit OK\nOder nach CANCEL neuen Pfad anwählen", "Achtung", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    try
                    {
                        //AddRandomData();

                        File.WriteAllText(PmPath + "\\" + "MyPW.txt", "");

                        EncryptFile();

                        TextBoxPath.Background = System.Windows.Media.Brushes.LightGreen;
                        Login();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Fehler beim Schreiben der Datei");
                    }

                }

            }
            else
            {
                TextBoxPath.Background = System.Windows.Media.Brushes.LightGreen;
                Login();

            }
        }
        private string GetPath()
        {
            PmPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            MessageBox.Show("Wähle den Ordner für die Daten aus");

            //System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            //ofd.ShowDialog();

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();

            return fbd.SelectedPath;
        }
        private void AddRandomData()
        {

            Random rnd = new Random();

            //ToDo File neu beschreiben
            for (int k = 0; k < 2; k++)
            {
                string[] term = new string[6];

                for (int i = 0; i < 6; i++)
                {

                    int len = rnd.Next(8, 32);
                    int start = rnd.Next(CMyRandom.Chars.Length - len - 1);
                    term[i] = CMyRandom.Chars.Substring(start, len);
                }

                ListPw.Add(new CPwDat { Title = "_" + term[0], WebAdr = term[1], User = term[2], PW = term[3], Opt1 = term[4], Opt2 = "_" + term[5] });
            }
        }

        private void DeleteRandomData()
        {
            for (int i = ListPw.Count-1; i >=0; i--)
            {
                if (ListPw[i].Title.StartsWith("_") && ListPw[i].Opt2.StartsWith("_"))
                {
                    ListPw.RemoveAt(i);
                }
            }


        }


        private void SimpleTxtToListPw(string pathAndFile)
        {
            ListPw.Clear();

            //AddRandomData();

            string[] s = File.ReadAllLines(pathAndFile);

            //string[] b = s[0].Split(";");

            foreach (string item in s)
            {
                string[] b = item.Split(";");
                CPwDat pw = new CPwDat();
                pw.Title = b[0];
                pw.WebAdr = b[1];
                pw.User = b[2];
                pw.PW = b[3];
                pw.Opt1 = b[4];
                pw.Opt2 = b[5];
                ListPw.Add(pw);
            }
        }
    }
}
