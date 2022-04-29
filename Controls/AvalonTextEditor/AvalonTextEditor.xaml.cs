using JobViewer.Common;
using JobViewer.Model;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JobViewer.AvalonTextEditor
{
    public partial class AvalonTextEditor : UserControl
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

        public String TextSql
        {
            get { return (String)GetValue(TextSqlProperty); }
            set { SetValue(TextSqlProperty, value); }
        }
        public static readonly DependencyProperty TextSqlProperty =
            DependencyProperty.Register("TextSql", typeof(String), typeof(AvalonTextEditor), new PropertyMetadata(new PropertyChangedCallback(TextSqlPropertyChanged)));

        public String DefaultSchemaName
        {
            get { return (String)GetValue(DefaultSchemaNameProperty); }
            set { SetValue(DefaultSchemaNameProperty, value); }
        }
        public static readonly DependencyProperty DefaultSchemaNameProperty =
            DependencyProperty.Register("DefaultSchemaName", typeof(String), typeof(AvalonTextEditor));


        private static void TextSqlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (AvalonTextEditor)obj;
            ctrl.avalonEditCtrl.Text = ctrl.TextSql;
        }

        public AvalonTextEditor()
        {
            InitializeComponent();
        
            InitContextMenu();
        }

        private void InitContextMenu()
        {
            var menu = new ContextMenu();
            if (Properties.Settings.Default.UseOpenAsFile)
            {
                var menuItem = new MenuItem()
                {
                    Header = Properties.Settings.Default.MenuNameOpenAsFile,
                };
                menuItem.Click += OpenAsFile_Click;
                menu.Items.Add(menuItem);
            }
            if (Properties.Settings.Default.UseOpenAsParam)
            {
                var menuItem = new MenuItem()
                {
                    Header = Properties.Settings.Default.MenuNameOpenAsParam,
                };
                menuItem.Click += OpenAsParam_Click;
                menu.Items.Add(menuItem);
            }

            avalonEditCtrl.ContextMenu = menu;
        }

        private void EditorLoaded(object sender, RoutedEventArgs e)
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("JobViewer.Resourcees.sql.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    avalonEditCtrl.SyntaxHighlighting =
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                        ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }
        }

        private void avalonEditCtrl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var delimiters = new char[] {' ', '\r', '\n', '\t', '(', ')' };
            var clockPos = avalonEditCtrl.SelectionStart;
            var last = TextSql.IndexOfAny(delimiters, clockPos);
            if (last == -1)
                last = TextSql.Length - 1;

            var first = TextSql.LastIndexOfAny(delimiters, clockPos);
            first = first == -1 ? 0 : first + 1;

            var candidate = TextSql.Substring(first, last - first +1).Trim(delimiters);
            
            string sql = null;
            string objName = candidate;
            try
            {
                var parts = candidate.Split('.', 2);

                var schema = parts[0].Trim();
                objName = parts[1].Trim();

                if (!ApplicationContext.JobDbContext.IsDataBase(schema))
                {
                    schema = DefaultSchemaName;
                    objName = candidate;
                }

                sql = ApplicationContext.JobDbContext.SpHelpText(schema, objName);
                
                if (!string.IsNullOrEmpty(sql))
                    new DBObjectViewer(DefaultSchemaName, sql, objName).Show();
            }
            catch 
            {
            }
        }

        private void OpenAsParam_Click(object sender, RoutedEventArgs e)
        {
            var prg = Properties.Settings.Default.PathProgramOpenAsParam;
            var parm = avalonEditCtrl.SelectedText;
            System.Diagnostics.Process.Start(prg, parm);
        }

        private void OpenAsFile_Click(object sender, RoutedEventArgs e)
        {
            var file = System.IO.Path.Combine(Properties.Settings.Default.PathOpenAsFileSqlDir, $"{Guid.NewGuid()}.sql");
            File.WriteAllText(file, avalonEditCtrl.SelectedText);

            var prg = Properties.Settings.Default.PathProgramOpenAsFile;
            
            System.Diagnostics.Process.Start(prg, file);
        }
    }
}
