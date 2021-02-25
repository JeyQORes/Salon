using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для ClientListWindow.xaml
    /// </summary>
    public partial class ClientListWindow : Window
    {
        public ClientListWindow()
        {
            InitializeComponent();
            DataLoadOne();
            DataLoad(temp);
            cmbSortir.SelectedIndex = 0;
            cbGender.SelectedIndex = 0;
            cbCountRows.SelectedIndex = 0;
        }
        int page = 1;
        int rows = 20;
        int allRows = 0;
        int rowsForm = 0;
        List<Clients> temp;

        public void DataLoadOne()
        {
            using (var db = new SalonFaceEntities())
            {
                temp = db.Clients.ToList();
                allRows = temp.Count();
                rowsForm = allRows;
                numpage.Text = page.ToString();
            }
        }

        public void DataLoad(List<Clients> ClientsFilter)
        {
            Data.ItemsSource = ClientsFilter.Skip(page * rows - rows).Take(rows);
            int RowsFilter = ClientsFilter.Count();
            tbCount.Text = $"{RowsFilter}записей из{rowsForm}";
        }

        string gender = "все";
        string search = "";
        string sortir = "";
        bool db = false;

        private void cbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            gender = (cbGender.SelectedItem as ComboBoxItem).Content.ToString();
            DataLoad(NewListClients(gender, search, sortir, db));
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            search = tbSearch.Text;
            DataLoad(NewListClients(gender, search, sortir, db));
        }

        private void cmbSortir_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sortir = (cmbSortir.SelectedItem as ComboBoxItem).Content.ToString();
            DataLoad(NewListClients(gender, search, sortir, db));
        }

        private void chbDB_Click(object sender, RoutedEventArgs e)
        {
            db = chbDB.IsChecked.Value;
            DataLoad(NewListClients(gender, search, sortir, db));
        }

        public List<Clients> NewListClients(string gender, string search, string sortir, bool db)
        {
            var result = temp;
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                result = result.Where(c => (c.LastName.ToLower().Contains(search.ToLower())) || (c.FirstName.ToLower().Contains(search.ToLower()))
                || (c.Patronymic.ToLower().Contains(search.ToLower())) || (c.Email.ToLower().Contains(search.ToLower())) || (c.NumberPhone.ToLower().Contains(search.ToLower()))).ToList();

            }
            if (gender.ToLower() != "все")
            {
                if (gender.ToLower() == "мужской")
                {
                    result = result.Where(c => c.Gender == "м").ToList();
                }
                else
                {

                    result = result.Where(c => c.Gender == "ж").ToList();
                }
            }
            if (sortir != "")
            {
                switch (sortir)
                {
                    case "По фамилии":
                        result = result.OrderBy(c => c.LastName).ToList();
                        break;
                    case "По дате последнего посещения":
                        result = result.OrderByDescending(c => c.DateLastVisit).ToList();
                        break;
                    case "По количеству посещений":
                        result = result.OrderByDescending(c => c.VisitCount).ToList();
                        break;
                }
            }
            if (db == true)
            {
                result = result.Where(c => c.BD.Value.Month == DateTime.Now.Month).ToList();
            }
            return result;
        }

        private void btRem_Click(object sender, RoutedEventArgs e)
        {
            Clients client = Data.SelectedItem as Clients;
            if (client != null)
            {
                EditClientWindow win = new EditClientWindow(client);
                this.Hide();
                win.ShowDialog();
                this.Show();
                DataLoadOne();
                DataLoad(NewListClients(gender, search, sortir, db));
            }
            else
            {
                MessageBox.Show("Клиент не выбран");
            }
        }

        private void btDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clients cl = Data.SelectedItem as Clients;
                if (cl != null)
                {
                    using (var bd = new SalonFaceEntities())
                    {
                        Clients clDel = bd.Clients.Where(c => c.IDClient == cl.IDClient).FirstOrDefault();
                        var rec = bd.ServiceClients.Where(c => c.ClientID == clDel.IDClient).FirstOrDefault();
                        if (rec != null)
                        {
                            MessageBox.Show("Удаление клиента невозможно. Имеется запись.");
                        }
                        else
                        {
                            var tags = bd.TagsClient.Where(c => c.ClientID == clDel.IDClient).ToList();
                            if (tags != null)
                            {
                                bd.TagsClient.RemoveRange(tags);
                            }
                            bd.Clients.Remove(clDel);
                            bd.SaveChanges();
                            MessageBox.Show("Клиент удален");
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Клиент не выбран");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла непредвиденная ошибка.Повторите попытку");
            }
        }

        private void left_Click(object sender, RoutedEventArgs e)
        {
            if (page != 1)
            {
                page--;
                DataLoad(NewListClients(gender, search, sortir, db));
                numpage.Text = page.ToString();
            }
        }

        private void right_Click(object sender, RoutedEventArgs e)
        {
            page++;
            DataLoad(NewListClients(gender, search, sortir, db));
            numpage.Text = page.ToString();
        }

        private void cbCountRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string chet = (cbCountRows.SelectedItem as ComboBoxItem).Content.ToString();
            if (chet != "все")
            {
                rows = Convert.ToInt32(chet);
                DataLoad(NewListClients(gender, search, sortir, db));
            }
            else
            {
                rows = rowsForm;
                DataLoad(NewListClients(gender, search, sortir, db));
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            EditClientWindow win = new EditClientWindow();
            this.Hide();
            win.ShowDialog();
            this.Show();
            DataLoadOne();
            DataLoad(NewListClients(gender, search, sortir, db));
        }

        private void btSbros_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            cbGender.SelectedIndex = 0;
            cmbSortir.SelectedIndex = 0;
            chbDB.IsChecked = false;
            gender = "все";
            search = "";
            sortir = "";
            db = false;
            DataLoadOne();
            DataLoad(NewListClients(gender, search, sortir, db));
        }

        private void btRec_Click(object sender, RoutedEventArgs e)
        {
            var client = Data.SelectedItem as Clients;
            if(client == null)
            {
                MessageBox.Show("Выберите клиента");
            }
            else
            {
                ClientServiceListWindow form = new ClientServiceListWindow(client);
                this.Hide();
                form.ShowDialog();
                this.Show();
            }
        }
    }
}
