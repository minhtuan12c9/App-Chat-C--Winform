﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppChat.Controls
{
    public partial class GetMess : UserControl
    {
        public GetMess(String s, String t)
        {
            InitializeComponent();
            mess.Text = s;
            timeMess.Text = t;
        }
    }
}