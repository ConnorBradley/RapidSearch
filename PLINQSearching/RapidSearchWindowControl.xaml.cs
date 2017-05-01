//------------------------------------------------------------------------------
// <copyright file="RapidSearchWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Windows.Forms.Timer;
using UserControl = System.Windows.Controls.UserControl;

namespace RapidSearching
{
    /// <summary>
    ///     Interaction logic for RapidSearchWindowControl.
    /// </summary>
    public partial class RapidSearchWindowControl : UserControl
    {

        public List<LineDetails> solutionContents = new List<LineDetails>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="RapidSearchWindowControl" /> class.
        /// </summary>T
        public RapidSearchWindowControl()
        {
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            InitializeComponent();

            ChangeColours();

            Thread t = new System.Threading.Thread(LoopLoadSolutionContents);
            t.Start();

            this.dataGrid.CanUserAddRows = false;
        }

        /// <summary>
        /// Raised whenever the visual studio theme changes.
        /// Matches the colour theme of VS
        /// </summary>
        /// <param name="e"></param>
        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            ChangeColours();
        }

        private void ChangeColours()
        {
            var background = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundBrushKey);
            var backgroundColor = Color.FromArgb(background.A, background.R, background.G, background.B);
            btnSearch.Background = new SolidColorBrush(backgroundColor);
            btnRegEx.Background = new SolidColorBrush(backgroundColor);
            txtSearchTerm.Background = new SolidColorBrush(backgroundColor);

            var foreground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
            var foreGroundColor = System.Windows.Media.Color.FromArgb(foreground.A, foreground.R, foreground.G,
                foreground.B);
            btnSearch.Foreground = new SolidColorBrush(foreGroundColor);
            btnRegEx.Foreground = new SolidColorBrush(foreGroundColor);

            var textSearchForeground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextBrushKey);
            var textForeGroundColor = Color.FromArgb(textSearchForeground.A, textSearchForeground.R,
                textSearchForeground.G,
                textSearchForeground.B);
            txtSearchTerm.Foreground = new SolidColorBrush(textForeGroundColor);

            var backgroundDataGrid = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundBrushKey);
            var backgroundColorDataGrid = System.Windows.Media.Color.FromArgb(backgroundDataGrid.A, backgroundDataGrid.R,
                backgroundDataGrid.G, backgroundDataGrid.B);


            var foregroundDataGrid = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
            var foreGroundColorDataGrid = System.Windows.Media.Color.FromArgb(foregroundDataGrid.A, foregroundDataGrid.R,
                foregroundDataGrid.G, foregroundDataGrid.B);

            dataGrid.Foreground = new SolidColorBrush(foreGroundColorDataGrid);
            dataGrid.RowBackground = new SolidColorBrush(backgroundColorDataGrid);

        }

        /// <summary>
        ///     Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Default event handler naming pattern")]
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtSearchTerm.Text == string.Empty)
                {
                    MessageBox.Show("Please enter a search term.");
                }
                else
                {
                    //if (solutionContents.Count <= 0)
                    //{
                    //    LoadSolutionContents();
                    //}

                    List<LineDetails> matches = new List<LineDetails>();
                    var sw = new Stopwatch();
                    sw.Start();
                    //9 is the number in which boyer-moore out performs IndexOf
                    if (txtSearchTerm.Text.Length > 9)
                    {
                        matches = FileSearch.BoyerMooreSearch(txtSearchTerm.Text,
                            solutionContents);
                    }
                    else
                    {
                        matches = FileSearch.IndexOfSearch(txtSearchTerm.Text, solutionContents);
                    }
                    

                    if (matches.Count <= 0)
                    {
                        MessageBox.Show("No matches were found.");
                        return;
                    }
                    var dt = new DataTable();

                    dt.Columns.Add("FileName", typeof(string));
                    dt.Columns.Add("LineNo", typeof(int));
                    dt.Columns.Add("Content", typeof(string));


                    foreach (var match in matches)
                    {
                        dt.Rows.Add(match.FileInfo.Name, match.LineNo, match.LineContent);
                    }


                    ResultsStorage.ResultsDataTable = dt;

                    if (ResultsStorage.ResultsDataTable != null)
                    {
                        dataGrid.DataContext = ResultsStorage.ResultsDataTable.DefaultView;
                        dataGrid.UpdateLayout();
                    }

                    sw.Stop();
                    lblTimeTaken.Text = "Time Taken: " + sw.Elapsed.ToString("g");
                    lblTimeTaken.UpdateLayout();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private void RegExbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (txtSearchTerm.Text == string.Empty)
                {
                    MessageBox.Show("Please enter a search term");
                }
                else
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var matches = FileSearch.RegExStringSearch(txtSearchTerm.Text,
                        solutionContents);
                    

                    if (matches.Count <= 0)
                    {
                        MessageBox.Show("No matches were found.");
                        return;
                    }

                    var dt = new DataTable();

                    dt.Columns.Add("FileName", typeof(string));
                    dt.Columns.Add("LineNo", typeof(int));
                    dt.Columns.Add("Content", typeof(string));

                    foreach (var match in matches)
                    {
                        dt.Rows.Add(match.FileInfo.Name, match.LineNo, match.LineContent);
                    }


                    ResultsStorage.ResultsDataTable = dt;

                    if (ResultsStorage.ResultsDataTable != null)
                    {
                        dataGrid.DataContext = ResultsStorage.ResultsDataTable.DefaultView;
                        //dataGrid.UpdateLayout();
                        dataGrid.Items.Refresh();
                    }

                    sw.Stop();
                    lblTimeTaken.Text = "Time Taken: " + sw.Elapsed.ToString("g");
                    lblTimeTaken.UpdateLayout();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (dataGrid.SelectedItems.Count <= 0) return;
                DataRowView row = (DataRowView) dataGrid.SelectedItems[0];
                LineDetails line = new LineDetails(new FileInfo(row[0].ToString()), (int) row[1], (string) row[2]);


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

        /// <summary>
        /// Timer to prevent multiple loading of the solution contents in quick succession
        /// </summary>

        private void LoopLoadSolutionContents()
        {
            while (true)
            {
                Thread.Sleep(5000);
                try
                {

                    solutionContents =
                        FileSearch.GetAllFilesInFolder(FileSearch.GetSolutionDirectory(FileSearch.GetCurrentDTE()));
                    //if it gets to this part, there was no errors meaning the solution was loaded properly.

                }
                catch (DirectoryNotFoundException ex)
                {
                    //this just means that the solution isn't loaded properly yet, its OK.
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Oops, theres been an error \r\n\r\n" + ex.Message);
                }
            }
        }
    }
}