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
            StackPanelFilter.Visibility = Visibility.Collapsed;
            StackPanelHelp.Visibility = Visibility.Collapsed;
            ListViewPwDat.Visibility = Visibility.Collapsed;

            PmPath = GetSetting("PmPath");
            SelectPasswordFile();
        }

        private void SelectPasswordFile()
        {
            if (PmPath == "")
            {
                PmPath = GetPath();
                if (PmPath == "")
                {
                    TextBoxPath.Background = System.Windows.Media.Brushes.LightPink;
                    MessageBox.Show("Bitte einen Ordner für die Daten auswählen");
                    return;
                }
                else
                {
                    SetSetting("PmPath", PmPath);
                }
            }
            TextBoxPath.Text = PmPath + "\\" + "Mp.txt";

            if (!File.Exists(PmPath + "\\" + "Mp.txt"))
            {
                TextBoxPath.Background = System.Windows.Media.Brushes.LightPink;

                MessageBoxResult result =
                MessageBox.Show("Die PasswordDatei 'Mp.txt'\nexisiert noch nicht\nNeu erstellen mit OK\nOder nach CANCEL neuen Pfad anwählen", "Achtung", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    try
                    {
                        ListPwAddRndData();

                        File.WriteAllText(PmPath + "\\" + "Mp.txt", "");

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

            }
        }

        private void ListPwAddRndData()
        {

            Random rnd = new Random();

            //ToDo File neu beschreibe
            for (int k = 0; k < 2; k++)
            {
                string[] term = new string[6];

                for (int i = 0; i < 6; i++)
                {

                    int len = rnd.Next(8, 32);
                    int start = rnd.Next(MyRandom.Chars.Length - len - 1);
                    term[i] = MyRandom.Chars.Substring(start, len);
                }

                ListPw.Add(new CPwDat { Title = "_" + term[0], WebAdr = term[1], User = term[2], PW = term[3], Opt1 = term[4], Opt2 = "_" + term[5] });
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

        private void Login()
        {
            ImageClock.Visibility = Visibility.Hidden;


            if (DecryptFile())
            {

                StackPanelUser.Visibility = Visibility.Collapsed;
                StackPanelPath.Visibility = Visibility.Collapsed;
                StackPanelMainLeft.Visibility = Visibility.Visible;
                StackPanelFilter.Visibility = Visibility.Visible;
                StackPanelHelp.Visibility = Visibility.Visible;
                ListViewPwDat.Visibility = Visibility.Visible;

                PwDatToListView("");

            }






        }

        private void PwDatToListView(string filter)
        {
            ListViewPwDat.Items.Clear();

            if (filter == "")
            {
                foreach (CPwDat item in ListPw)
                {
                    if (item.Title.StartsWith("_") && item.Opt2.StartsWith("_"))  //erster Random-Datensatz
                        continue;
                    ListViewPwDat.Items.Add(new CPwDat { Title = item.Title, WebAdr = item.WebAdr, User = item.User, PW = item.PW, Opt1 = item.Opt1, Opt2 = item.Opt2 });
                }
            }
            else
            {
                foreach (CPwDat item in ListPw)
                {
                    if (item.ToString().Contains(filter))
                    {
                        if (item.Title.StartsWith("_") && item.Opt2.StartsWith("_"))  //erster Random-Datensatz
                            continue;
                        ListViewPwDat.Items.Add(new CPwDat { Title = item.Title, WebAdr = item.WebAdr, User = item.User, PW = item.PW, Opt1 = item.Opt1, Opt2 = item.Opt2 });
                    }

                }
            }
        }

        private void ButtonPath_Click(object sender, RoutedEventArgs e)
        {

            if (!CheckUsePwLen())
                return;

            PmPath = "";

            SelectPasswordFile();




        }

        //private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        //{

        //}
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {

            if (!CheckUsePwLen())
                return;

            PmPath = GetSetting("PmPath");
            if (PmPath == "" || !File.Exists(PmPath + "\\" + "Mp.txt"))
            {
                MessageBox.Show("PasswordFile '\n" + PmPath + "\\" + "Mp.txt' " + "\nexistiert nicht");
                return;
            }


            ImageClock.Visibility = Visibility.Visible;
            TimerStart.Start();

        }

        private bool CheckUsePwLen()
        {
            if (TextBoxUser.Text.Length < 6 || TextBoxUser.Text.Length > 12)
            {
                MessageBox.Show("'User'  muss 6 bis 12 Zeichen haben");
                return false;
            }
            if (MyPasswordBox.Password.Length < 8 || MyPasswordBox.Password.Length > 16)
            {
                MessageBox.Show("'Password'  muss 8 bis 16 Zeichen haben");
                return false;
            }
            return true;
        }

        private void LabelUserName_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!TimerLoginShort.IsEnabled)
            {
                TimerLoginShort.Start();
                ShortLogin = 1;
            }
        }

        private void LabelPassword_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ShortLogin == 1)
            {
                ShortLogin = 2;
            }

        }

        private void ButtonLogin_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ShortLogin == 2 && TimerLoginShort.IsEnabled)
            {
                ShortLogin = 0;
                TimerLoginShort.Stop();
                TextBoxUser.Text = "reiner0533";
                MyPasswordBox.Password = "reiner0533";
            }
        }
    }
}
