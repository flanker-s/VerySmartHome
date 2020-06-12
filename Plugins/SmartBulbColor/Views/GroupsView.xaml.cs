﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartBulbColor.Views
{
    /// <summary>
    /// Interaction logic for GroupsView.xaml
    /// </summary>
    public partial class GroupsView : UserControl
    {
        public GroupsView()
        {
            InitializeComponent();
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            AddGoupPopup.IsOpen = true;
        }

        private void RenameGroupButton_Click(object sender, RoutedEventArgs e)
        {
            RenameGoupPopup.IsOpen = true;
        }
    }
}
