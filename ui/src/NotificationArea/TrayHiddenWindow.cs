using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guardian.NotificationArea
{
    class TrayHiddenWindow : Form
    {
        public TrayHiddenWindow()
        {

        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
    }
}
