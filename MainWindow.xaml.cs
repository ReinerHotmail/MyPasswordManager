﻿using Microsoft.Win32;
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

            TimerLoginShort.Interval = TimeSpan.FromSeconds(2);
            TimerLoginShort.Tick += TimerLoginShort_Tick;


            TimerStart.Interval = TimeSpan.FromSeconds(1);
            TimerStart.Tick += TimerStart_Tick;


        }

        private void TimerLoginShort_Tick(object sender, EventArgs e)
        {
            TimerLoginShort.Stop();
        }

        private void SetWindowLayout()
        {
            string w = GetSetting("Width");

            if (w != "")
            {
                WindowMyPasswordManager.Width = Convert.ToDouble(w);
            }
            else
            {
                WindowMyPasswordManager.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            }


            string h = GetSetting("Height");

            if (h != "")
            {
                WindowMyPasswordManager.Height = Convert.ToDouble(h);
            }
            string t = GetSetting("Top");

            if (t != "")
            {
                WindowMyPasswordManager.Top = Convert.ToDouble(t);
            }
            string l = GetSetting("Left");

            if (l != "")
            {
                WindowMyPasswordManager.Left = Convert.ToDouble(l);
            }
        }

        private void TimerStart_Tick(object sender, EventArgs e)
        {
            TimerStart.Stop();
            Login();
        }




        /// <summary>
        /// Erweitert zu kurze Kennwörter mit Nullen
        /// </summary>
        /// <param name="key">Kennwort</param>
        /// <param name="newKeyLength">Neue Kennwortlänge</param>
        /// <returns></returns>
        private byte[] DoExtendKey(string key, int newKeyLength)
        {
            byte[] bKey = new byte[newKeyLength];
            byte[] tmpKey = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < key.Length; i++)
            {
                bKey[i] = tmpKey[i];
            }

            return bKey;
        }

        /// <summary>
        /// Erweitert zu kurze Initialisierungsvektoren mit Nullen.
        /// </summary>
        /// <param name="newBlockSize">Neue Blockgröße</param>
        /// <returns></returns>
        private byte[] DoCreateBlocksize(string iv, int newBlockSize)
        {

            byte[] bIv = new byte[newBlockSize];
            byte[] tmpIv = Encoding.UTF8.GetBytes(iv);

            for (int i = 0; i < iv.Length; i++)
            {
                bIv[i] = tmpIv[i];
            }

            return bIv;


        }






        /// <summary>
        /// Verschlüsselt eine Datei.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdDateiVerschlüsseln_Click(object sender, RoutedEventArgs e)
        {
            EncryptFile();
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

                //ToDo PASSWORTE lesen




            }
















        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonLoginNew_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
