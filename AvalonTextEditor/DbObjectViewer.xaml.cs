using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JobViewer
{
    /// <summary>
    /// Логика взаимодействия для ObjectViewer.xaml
    /// </summary>
    public partial class DBObjectViewer : Window
    {
        public String WindowTitle
        {
            get { return (String)GetValue(WindowTitleProperty); }
            set { SetValue(WindowTitleProperty, value); }
        }
        public static readonly DependencyProperty WindowTitleProperty =
            DependencyProperty.Register("WindowTitle", typeof(String), typeof(DBObjectViewer));

        public String TextSql
        {
            get { return (String)GetValue(TextSqlProperty); }
            set { SetValue(TextSqlProperty, value); }
        }
        public static readonly DependencyProperty TextSqlProperty =
            DependencyProperty.Register("TextSql", typeof(String), typeof(DBObjectViewer));

        public String DefaultSchemaName
        {
            get { return (String)GetValue(DefaultSchemaNameProperty); }
            set { SetValue(DefaultSchemaNameProperty, value); }
        }
        public static readonly DependencyProperty DefaultSchemaNameProperty =
            DependencyProperty.Register("DefaultSchemaName", typeof(String), typeof(DBObjectViewer));


        public DBObjectViewer( string defaultSchemaName, string sql, string objName )
        {
            InitializeComponent();
            this.DataContext = this;
            TextSql = sql;
            DefaultSchemaName = defaultSchemaName;
            WindowTitle = objName;
        }
    }
}
