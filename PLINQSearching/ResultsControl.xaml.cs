//------------------------------------------------------------------------------
// <copyright file="ResultsControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace PLINQSearching
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ResultsControl.
    /// </summary>
    public partial class ResultsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsControl"/> class.
        /// </summary>
        public ResultsControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.DataContext = ResultsStorage.ResultsDataTable.DefaultView;
            dataGrid.UpdateLayout();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                DataRowView row = (DataRowView) dataGrid.SelectedItems[0];
                LineDetails line = new LineDetails(new FileInfo(row[0].ToString()), (int)row[1], (string)row[2]);


                var dte = FileSearch.GetCurrentDTE();
                try
                {
                    //File is opened within VisualStudio 
                    dte.Windows.Item(line.FileInfo.Name).Activate();
                }
                catch
                {
                    try
                    {
                        //File is not open
                        dte.ExecuteCommand("File.OpenFile", line.FileInfo.Name);
                        dte.Windows.Item(line.FileInfo.Name).Activate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                System.Threading.Thread.Sleep(500);
                dte.ExecuteCommand("Edit.GoTo", line.LineNo.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}