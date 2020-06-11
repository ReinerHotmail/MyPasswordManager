using Simplify.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyPasswordManager
{
    public partial class MainWindow
    {

        private string GetSetting(string key)
        {

            string[] s = File.ReadAllLines("Resources\\SettingDat.txt");

            try
            {
                foreach (string item in s)
                {
                    if (item.StartsWith(key))
                    {
                        return item.Split(";")[1];
                    }
                }
                return "";
            }
            catch (Exception)
            {

                return "";
            }
      
        }

        private void SetSetting(string key,string  val)
        {
            string[] s = File.ReadAllLines("Resources\\SettingDat.txt");
            bool found = false;

            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].StartsWith(key))
                    {
                        string[] keyVal = s[i].Split(";");

                        keyVal[1] = val;
                        s[i] = keyVal[0] + ";" + keyVal[1];
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Array.Resize(ref s, s.Length + 1);

                    s[s.Length - 1] = key + ";" + val;
                }

                File.WriteAllLines("Resources\\SettingDat.txt", s);


          
            }
            catch (Exception)
            {

                System.Windows.MessageBox.Show("Fehler beim schreiben des SettingFiles");
            }

        }
    }
}