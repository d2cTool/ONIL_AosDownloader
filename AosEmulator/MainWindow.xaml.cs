using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace AosEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ConnectionModel ConnectionModel = new ConnectionModel();

        public MainWindow()
        {
            DataContext = ConnectionModel;
            InitializeComponent();
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConnectionModel.Dispose();
        }
    }
}
