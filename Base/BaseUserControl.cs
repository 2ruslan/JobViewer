using JobViewer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace JobViewer.Base
{
    public class BaseUserControl : UserControl
    {
        protected ApplicationContext ApplicationContext
        {
            get
            {
                if (App.Current != null && App.Current is App app)
                    return app.ApplicationContext;
                return null;
            }
        }
    }
}
