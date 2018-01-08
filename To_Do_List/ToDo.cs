using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace To_Do_List
{
    public class ToDo
    {
        public int id { get; set; }
        public string heading { get; set; }
        public string description { get; set; }

        public Boolean is_complete { get; set; }
    }
}