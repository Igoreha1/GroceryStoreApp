using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStoreApp.AppData
{
    internal static class AppConnect
    {
        public static Users CurrentUser { get; set; }
        public static GroceryStoreDBEntities model01 = new GroceryStoreDBEntities();
    }
}