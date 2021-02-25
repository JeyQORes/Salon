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
using WpfAppSalon.Entities;
using WpfAppSalon.Entities.DataBase;

namespace WpfAppSalon.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClientServiceListWindow.xaml
    /// </summary>
    public partial class ClientServiceListWindow : Window
    {
        public ClientServiceListWindow(Clients client)
        {
            InitializeComponent();
            tbFullName.Text = $"{client.LastName} {client.FirstName} {client.Patronymic}";
            using (var db = new SalonFaceEntities())
            {
                List.ItemsSource = db.ServiceClients.Where(c => c.ClientID == client.IDClient).Select(c => new Services
                {
                    ServiceClient = c
                }).ToList();
            }
        }
    }
}
