﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace MyPasswordManager
{
    public partial class MainWindow
    {

        /// <summary>
        /// Entschlüsselt eine Datei.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdDateiEntschlüsseln_Click(object sender, RoutedEventArgs e)
        {
            DecryptFile();
        }

        private bool DecryptFile()
        {
            bool ok = true;

            // ---------------     Entschlüsseln    -------------------------------
            // Erstelle eine Instanz von AES und weise ihr einen Schlüssel und Initialisierungsvektor zu
            // Wichtig ist hier das Padding, welches wir unbedingt definieren müssen.
            Aes AESCrypto = Aes.Create();
            AESCrypto.Padding = PaddingMode.Zeros;
            AESCrypto.Key = DoExtendKey((MyPasswordBox.Password + TextBoxUser.Text+ "12345678901234567890abcd").Substring(0, 32), 32);
            AESCrypto.IV = DoCreateBlocksize((TextBoxUser.Text + "123456abcde").Substring(0, 16), 16);


            try
            {
                // Die neue Datei hat den gleichen Namen, allerdings die Dateiendung .txt
                // Achtung: Die Originaldatei wird überschrieben, wenn diese sich im gleichen Pfad befindet!
                //     string neueDatei = System.IO.Path.ChangeExtension(openFileDialog1.FileName, ".txt");
                // Erstelle einen Inputstream, Outputstream und Cryptostream

                FileStream inputStream = new FileStream(PmPath + "\\" + "Mp.txt", FileMode.Open);

          

                //FileStream outputStream = new FileStream(PmPath + "\\" + "Mp2.txt", FileMode.Create);
                CryptoStream cStream = new CryptoStream(inputStream, AESCrypto.CreateDecryptor(), CryptoStreamMode.Read);

                // Entschlüssele nun jedes Byte bis zum Dateiende
                int data;
                //while ((data = cStream.ReadByte()) != -1)
                //    outputStream.WriteByte((byte)data);

                StringBuilder sb = new StringBuilder();

                while ((data = cStream.ReadByte()) != -1)
                {
                    sb.Append(((char)data).ToString());
                }

                string[] s = sb.ToString().Split("\n");

                ListPw.Clear();

                foreach (string item in s)
                {
                    string[] b = item.Split(";");
                    if (b.Length==6)
                    {
                        CPwDat pw = new CPwDat();
                        pw.Title = b[0].Trim();
                        pw.WebAdr = b[1].Trim();
                        pw.User = b[2].Trim();
                        pw.PW = b[3].Trim();
                        pw.Opt1 = b[4].Trim();
                        pw.Opt2 = b[5].Trim();
                        ListPw.Add(pw);
                    }
                    else
                    {
                        if (ListPw.Count==0)
                        {
                            MessageBox.Show("Passwort passt nicht\n\nPasswort oder Directory ändern");
                            ok = false;
                            break;
                        }
                        //else
                        //{
                        //    MessageBox.Show("Fehler bei Datensatz " + ListPw.Count, "", MessageBoxButton.OK, MessageBoxImage.Error);
                        //}
                
                    }

                }


                inputStream.Close();
                cStream.Close();

             
            }
            catch
            {
                MessageBox.Show("Ein Fehler ist aufgetreten!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ok;
        }

        private void EncryptFile()
        {
            // ---------------     Verschlüsseln    -------------------------------
            // Erstelle eine Instanz von AES und weise ihr einen Schlüssel und Initialisierungsvektor zu
            Aes AESCrypto = Aes.Create();
            AESCrypto.Key = DoExtendKey((MyPasswordBox.Password + TextBoxUser.Text + "12345678901234567890abcd").Substring(0, 32), 32);
            AESCrypto.IV = DoCreateBlocksize((TextBoxUser.Text + "1234abcd").Substring(0, 16), 16);


            try
            {
                

  



                // Die neue Datei hat den gleichen Namen, allerdings die Dateiendung .crypt
                //  string neueDatei = System.IO.Path.ChangeExtension(openFileDialog1.FileName, ".crypt");
                // Erstelle einen Inputstream, Outputstream und Cryptostream
                //FileStream inputStream = new FileStream(PmPath + "\\" + "MpCopy.txt", FileMode.Open);
                FileStream outputStream = new FileStream(PmPath + "\\" + "Mp.txt", FileMode.Create);
                CryptoStream cStream = new CryptoStream(outputStream, AESCrypto.CreateEncryptor(), CryptoStreamMode.Write);

                // Verschlüssele nun jedes Byte bis zum Dateiende
                //int data;
                //while ((data = inputStream.ReadByte()) != -1)
                //    cStream.WriteByte((byte)data);

                foreach (CPwDat item in ListPw)
                {
                    string strPw = item.ToString()+"\n";

                    foreach (byte b in strPw)
                    {
                        cStream.WriteByte(b);
                    }
                }
         

                //inputStream.Close();
                cStream.Close();
                outputStream.Close();
                MessageBox.Show("Verschlüsselung abgeschlossen!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Ein Fehler ist aufgetreten!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Berechnet den Hashwert einer Datei.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdHashBilden_Click(object sender, RoutedEventArgs e)
        {
            SimpleTxtToListPw(PmPath + "\\" + "Passworte.txt");

            return;



            // Erstelle eine Instanz von MD5
            MD5 MD5Hash = MD5.Create();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                try
                {
                    // Erstelle einen Inputstream
                    FileStream inputStream = new FileStream(openFileDialog1.FileName, FileMode.Open);

                    // Berechne den Hashwert und schreibe ihn in ein Byte-Array
                    byte[] hash = MD5Hash.ComputeHash(inputStream);
                    // Wandel ihn Byte für Byte in eine Zeichenkette um (Dez -> Hex)
                    txtHash.Text = BitConverter.ToString(hash).Replace("-", "").ToLower(); ;

                    inputStream.Close();
                }
                catch
                {
                    MessageBox.Show("Ein Fehler ist aufgetreten!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SimpleTxtToListPw(string pathAndFile)
        {
            ListPw.Clear();

            ListPwAddRndData();

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
