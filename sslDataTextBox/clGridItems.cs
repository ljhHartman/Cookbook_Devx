﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sslDataTextBox
{
    class clGridItems
    {
        public static string strReturnValue { get; set; }

        public static BindingList<SelectItems> blItems { get; set; }
    }

    public class SelectItems
    {
        public SelectItems() { }
        public string ItemString { get; set; }
    }

    class DataItemInformation
    {
        public static string ItemHeader { get; set; }
        public static string ItemContext { get; set; }
    }
}
