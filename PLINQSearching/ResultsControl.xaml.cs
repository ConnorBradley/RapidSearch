//------------------------------------------------------------------------------
// <copyright file="ResultsControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.PlatformUI;
using Color = System.Windows.Media.Color;

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
            
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            this.InitializeComponent();

            if (ResultsStorage.ResultsDataTable != null)
            {
                dataGrid.DataContext = ResultsStorage.ResultsDataTable.DefaultView;
                dataGrid.UpdateLayout();
            }

            ChangeColours();
            //System.Threading.Tasks.Task.Run(() => RefreshGrid());

        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            ChangeColours();
        }


        private void ChangeColours()
        {
            try
            {
                var background = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundBrushKey);
                var backgroundColor = System.Windows.Media.Color.FromArgb(background.A, background.R, background.G, background.B);
                //button1.Background = new SolidColorBrush(backgroundColor);

                var foreground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
                var foreGroundColor = System.Windows.Media.Color.FromArgb(foreground.A, foreground.R, foreground.G, foreground.B);
                //button1.Foreground = new SolidColorBrush(foreGroundColor);

                dataGrid.Foreground = new SolidColorBrush(foreGroundColor);
                dataGrid.RowBackground = new SolidColorBrush(backgroundColor);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void RefreshDataGrid_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.DataContext = ResultsStorage.ResultsDataTable.DefaultView;
            dataGrid.UpdateLayout();
        }


        public void RefreshGrid()
        {
            if (ResultsStorage.ResultsDataTable != null)
            {
                dataGrid.DataContext = ResultsStorage.ResultsDataTable.DefaultView;
                dataGrid.UpdateLayout();
            }
            //try
            //{
            //    do
            //    {
            //        Thread.Sleep(100);
            //        if (!ResultsStorage.SearchResultsChanged) continue;
            //        //if (Application.Current.Dispatcher.CheckAccess())
            //        //{
            //        Thread.Sleep(1000); //wait for thread controlling datatable to stop
            //        dataGrid.DataContext = null;
            //        dataGrid.DataContext = ResultsStorage.ResultsDataTable.DefaultView;
            //        dataGrid.UpdateLayout();
            //        ResultsStorage.SearchResultsChanged = false;
            //        //}
            //        //else
            //        //{
            //        //    Application.Current.Dispatcher.Invoke(new System.Action(RefreshGrid));
            //        //}

            //    } while (true);
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }
 

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try catch methods are expensive and having 3 in a row isn't good, TODO Refactor
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