using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyPasswordManager
{
    public partial class MainWindow
    {
        private void ButtonInputStore_Click(object sender, RoutedEventArgs e)
        {
            string titleUp = TextBoxTitelIn.Text.Trim().ToUpper();
          

            if (titleUp == "")
            {
                MessageBox.Show("Titel im Datensatz fehlt");
                return;
            }

            // Durchsuchen der Liste nach dem neuen Titel
            CPwDat dat = ListPw.Find(x => x.Title.ToUpper() == titleUp);

            if (dat !=null)
            {
                MessageBoxResult result =
                MessageBox.Show("Es gibt schon einen Datensatz mit dem Titel\n     "+dat.Title+
                                "\nÜberschreiben","Achtung",MessageBoxButton.YesNo,MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;

                ListPw.Remove(dat);
            }

            CPwDat newDat = new CPwDat { Title = TextBoxTitelIn.Text.Trim(), WebAdr = TextBoxWebAdrIn.Text.Trim(), User = TextBoxUserIn.Text.Trim(), PW = TextBoxPwIn.Text.Trim(), Opt1 = TextBoxOpt1In.Text.Trim(), Opt2 = TextBoxOpt2In.Text.Trim() };

            ListPw.Add(newDat);

            PwDatToListView(TextBoxFilter.Text);

            EncryptFile();

            WriteOutputFields(newDat);
        }



        private void ListViewPwDat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewPwDat.SelectedItem == null)
                return;

            CPwDat selItem = (CPwDat)ListViewPwDat.SelectedItem;
            WriteInputFields(selItem);
            WriteOutputFields(selItem);
        }
        private void WriteInputFields(CPwDat selItem)
        {
            TextBoxTitelIn.Text = selItem.Title;
            TextBoxWebAdrIn.Text = selItem.WebAdr;
            TextBoxUserIn.Text = selItem.User;
            TextBoxPwIn.Text = selItem.PW;
            TextBoxOpt1In.Text = selItem.Opt1;
            TextBoxOpt2In.Text = selItem.Opt2;
        }

        private void WriteOutputFields(CPwDat selItem)
        {
            TextBoxTitelOut.Text = selItem.Title;


            TextBoxWebAdrOut.Text = selItem.WebAdr;

            string s = selItem.WebAdr;
            if (s.Contains(":"))
            {
                string[] sSplit = s.Split(":");
                LabelWebAdrOut.Content = sSplit[0];
                TextBoxWebAdrOut.Text = s.Substring(sSplit[0].Length + 1).Trim();
            }
            else
            {
                LabelWebAdrOut.Content = "Web-Adr";
                TextBoxWebAdrOut.Text = s;
            }

            s = selItem.User;
            if (s.Contains(":"))
            {
                string[] sSplit = s.Split(":");
                LabelUserOut.Content = sSplit[0];
                TextBoxUserOut.Text = s.Substring(sSplit[0].Length + 1).Trim();
            }
            else
            {
                LabelUserOut.Content = "User";
                TextBoxUserOut.Text = s;
            }

            s = selItem.PW;
            if (s.Contains(":"))
            {
                string[] sSplit = s.Split(":");
                LabelPwOut.Content = sSplit[0];
                TextBoxPwOut.Text = s.Substring(sSplit[0].Length + 1).Trim();
            }
            else
            {
                LabelPwOut.Content = "Password";
                TextBoxPwOut.Text = s;
            }

            s = selItem.Opt1;
            if (s.Contains(":"))
            {
                string[] sSplit = s.Split(":");
                LabelOpt1Out.Content = sSplit[0];
                TextBoxOpt1Out.Text = s.Substring(sSplit[0].Length + 1).Trim();
            }
            else
            {
                LabelOpt1Out.Content = "Opt1";
                TextBoxOpt1Out.Text = s;
            }

            s = selItem.Opt2;
            if (s.Contains(":"))
            {
                string[] sSplit = s.Split(":");
                LabelOpt2Out.Content = sSplit[0];
                TextBoxOpt2Out.Text = s.Substring(sSplit[0].Length + 1).Trim();
            }
            else
            {
                LabelOpt2Out.Content = "Opt2";
                TextBoxOpt2Out.Text = s;
            }
        }

        private void ListViewPwDat_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ListViewPwDat.SelectedItem == null)
                return;


            CPwDat selItem = (CPwDat)ListViewPwDat.SelectedItem;

            MessageBoxResult result =
           MessageBox.Show($"Datensatz '{selItem.Title}' löschen ?", "Achtung", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;


           

            ListPw.Remove(selItem);

            PwDatToListView(TextBoxFilter.Text);

            EncryptFile();

            //Löschen der Ausgangsanzeige
            CPwDat dat = new CPwDat { Title = "", WebAdr = "", User = "", PW = "", Opt1 = "", Opt2 = "" };
            WriteOutputFields(dat);
        }

        private void ButtonTitleDelete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxTitelIn.Text = "";
        }

        private void ButtonWebAdrDelete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxWebAdrIn.Text = "";
        }

        private void ButtonUser1Delete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxUserIn.Text = "";
        }

        private void ButtonPwDelete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxPwIn.Text = "";
        }

        private void ButtonOpt1Delete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxOpt1In.Text = "";
        }

        private void ButtonOpt2Delete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxOpt2In.Text = "";
        }

        private void ButtonInputDelete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxTitelIn.Text = "";
            TextBoxWebAdrIn.Text = "";
            TextBoxUserIn.Text = "";
            TextBoxPwIn.Text = "";
            TextBoxOpt1In.Text = "";
            TextBoxOpt2In.Text = "";
        }
    }
}
