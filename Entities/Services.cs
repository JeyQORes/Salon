using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppSalon.Entities.DataBase;

namespace WpfAppSalon.Entities
{
    public class Services
    {
        private static SalonFaceEntities db = new SalonFaceEntities();
        public ServiceClients ServiceClient { get; set; }
        public string Name
        {
            get
            {
                return $"Наименование услуги: {db.Service.Where(c => c.IDService == ServiceClient.ServiceID).FirstOrDefault().NameService}";
            }
        }
        public string Date
        {
            get
            {
                return ServiceClient.TimeRecord.Value.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }
}
