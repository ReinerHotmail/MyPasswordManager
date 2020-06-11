using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
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
            StackPanelTable.Visibility = Visibility.Collapsed;

            PmPath = GetSetting("PmPath");
            SelectPasswordFile();
        }

        private void SelectPasswordFile()
        {
          

            if (PmPath == "")
            {
                do
                {
                    PmPath = GetPath();
                } while (PmPath == "");

                SetSetting("PmPath", PmPath);
            }

            LabelSelectedPath.Content = PmPath + "\\" + "Mp.txt";

            if (!File.Exists(PmPath + "\\" + "Mp.txt"))
            {
                LabelSelectedPath.Background = System.Windows.Media.Brushes.LightPink;

                MessageBoxResult result =
                MessageBox.Show("Die PasswordDatei 'Mp.txt'\nexisiert noch nicht\nNeu erstellen mit OK\nOder nach CANCEL neuen Pfad anwählen", "Achtung", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    try
                    {
                        File.WriteAllText(PmPath + "\\" + "Mp.txt", "");
                        LabelSelectedPath.Background = System.Windows.Media.Brushes.LightGreen;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Fehler beim Schreiben der Datei");
                    }

                }

            }
            else
            {
                LabelSelectedPath.Background = System.Windows.Media.Brushes.LightGreen;

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


            if(DecryptFile())
            {

                StackPanelUser.Visibility = Visibility.Collapsed;
                StackPanelPath.Visibility = Visibility.Collapsed;
                StackPanelMainLeft.Visibility = Visibility.Visible;
                StackPanelTable.Visibility = Visibility.Visible;
            }

 

     


        }


        private void ButtonPath_Click(object sender, RoutedEventArgs e)
        {
            PmPath = "";

            SelectPasswordFile();


    

        }

        //private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        //{

        //}
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxUser.Text.Length < 8)
            {
                MessageBox.Show("Eingabe muss mindestens 8 Zeichen haben");
                return;
            }
            if (MyPasswordBox.Password.Length < 8)
            {
                MessageBox.Show("Eingabe muss mindestens 8 Zeichen haben");
                return;
            }


            PmPath = GetSetting("PmPath");
            if (PmPath =="" || !File.Exists(PmPath +"\\"+"Mp.txt"))
            {
                MessageBox.Show("PasswordFile '\n"+ PmPath + "\\" + "Mp.txt' "+"\nexistiert nicht");
                return;
            }


            ImageClock.Visibility = Visibility.Visible;
            TimerStart.Start();

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
