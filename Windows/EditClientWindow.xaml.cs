using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using WpfAppSalon.Entities.DataBase;

namespace WpfAppSalon.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditClientWindow.xaml
    /// </summary>
    public partial class EditClientWindow : Window
    {
        Clients clientGlob;
        string photo;
        public EditClientWindow()
        {
            InitializeComponent();
            tbid.Visibility = Visibility.Collapsed;
            tid.Visibility = Visibility.Collapsed;

        }

        public EditClientWindow(Clients client)
        {
            clientGlob = client;
            InitializeComponent();
            tbid.Text = client.IDClient.ToString();
            tblname.Text = client.LastName.ToString();
            tbfname.Text = client.FirstName.ToString();
            tbpa.Text = client.Patronymic.ToString();
            tbem.Text = client.Email.ToString();
            tbnum.Text = client.NumberPhone.ToString();
            tbbd.Text = client.BD.ToString();
            if (client.Gender == "м")
            {
                tbgen.SelectedIndex = 0;
            }
            else
            {
                tbgen.SelectedIndex = 1;
            }
            string newPath = $"{Environment.CurrentDirectory}/{client.Photo}";
            photo = client.Photo;
            LoadImage(newPath);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //загрузка картинки
        public void LoadImage(string path)
        {
            imbo.Source = new BitmapImage(new Uri(path));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if ((Proverka(tblname.Text, tbfname.Text, tbpa.Text, tbem.Text, tbnum.Text, (tbgen.SelectedItem), tbbd.Text)) == true)
            {
                using (var db = new SalonFaceEntities())
                {
                    if (clientGlob == null)
                    {
                        Clients clEdit = new Clients();
                        clEdit.LastName = tblname.Text;
                        clEdit.FirstName = tbfname.Text;
                        clEdit.Patronymic = tbpa.Text;
                        clEdit.NumberPhone = tbnum.Text;
                        clEdit.Email = tbem.Text;
                        clEdit.BD = DateTime.Parse(tbbd.Text);
                        string gend = (tbgen.SelectedItem as ComboBoxItem).Content.ToString();
                        if (gend == "мужской")
                        {
                            clEdit.Gender = "м";
                        }
                        else
                        {
                            clEdit.Gender = "ж";
                        }
                        clEdit.Photo = photo;
                        clEdit.DateRegistration = DateTime.Now;
                        db.Clients.Add(clEdit);
                        db.SaveChanges();
                        MessageBox.Show("Клиент добавлен");
                    }
                    else
                    {
                        Clients clEdit = db.Clients.Where(c => c.IDClient == clientGlob.IDClient).FirstOrDefault();
                        clEdit.LastName = tblname.Text;
                        clEdit.FirstName = tbfname.Text;
                        clEdit.Patronymic = tbpa.Text;
                        clEdit.NumberPhone = tbnum.Text;
                        clEdit.Email = tbem.Text;
                        clEdit.BD = DateTime.Parse(tbbd.Text);
                        if (tbgen.SelectedIndex == 0)
                        {
                            clEdit.Gender = "м";
                        }
                        else
                        {
                            clEdit.Gender = "ж";
                        }
                        clEdit.Photo = photo;
                        db.SaveChanges();
                        MessageBox.Show("Клиент отредактирован");
                    }
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Данные введены некорректно");
            }
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == true)
            {
                string FullPath = f.FileName;
                string file = f.SafeFileName;
                string newPath = $"{Environment.CurrentDirectory}/Клиенты/{file}";
                string savePath = $"Клиенты/{file}";
                if (!File.Exists(newPath))
                {
                    File.Copy(FullPath, newPath);
                }
                LoadImage(newPath);
                photo = savePath;
            }
        }

        private bool Proverka(string last, string first, string patron, string email, string number, object gen, string data)
        {
            bool result = true;
            if (string.IsNullOrEmpty(data))
            {
                result = false;
            }

            if (gen == null)
            {
                result = false;
            }

            if (string.IsNullOrEmpty(last) || string.IsNullOrWhiteSpace(last))
            {
                result = false;
            }

            if (string.IsNullOrEmpty(first) || string.IsNullOrWhiteSpace(first))
            {
                result = false;
            }

            if (string.IsNullOrEmpty(patron) || string.IsNullOrWhiteSpace(patron))
            {
                result = false;
            }

            if ((patron.Length >= 50) || (first.Length >= 50) || (last.Length >= 50))
            {
                result = false;
            }

            if (!email.Contains("@"))
            {
                return false;
            }

            for (int i = 0; i < number.Length; i++)
            {
                if (!char.IsDigit(number[i]))
                {
                    if (number[i] != '+')
                    {
                        if (number[i] != '-')
                        {
                            if (number[i] != '(')
                            {
                                if (number[i] != ')')
                                {
                                    if (!char.IsWhiteSpace(number[i]))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
