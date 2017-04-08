//------------------------------------------------------------------------------
// <copyright file="PLINQSearchWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace PLINQSearching
{
    /// <summary>
    ///     Interaction logic for PLINQSearchWindowControl.
    /// </summary>
    public partial class PLINQSearchWindowControl : UserControl
    {

        public List<LineDetails> solutionContents = new List<LineDetails>();
        /// <summary>
        ///     Initializes a new instance of the <see cref="PLINQSearchWindowControl" /> class.
        /// </summary>
        public PLINQSearchWindowControl()
        {
            solutionContents = FileSearch.GetAllFilesInFolder(FileSearch.GetSolutionDirectory(FileSearch.GetCurrentDTE()));

            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            InitializeComponent();
             
            ChangeColours();
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
            btnIndexOf.Background = new SolidColorBrush(backgroundColor);
            btnContains.Background = new SolidColorBrush(backgroundColor);
            btnBoyerMoore.Background = new SolidColorBrush(backgroundColor);
            btnNaive.Background = new SolidColorBrush(backgroundColor);
            txtSearchTerm.Background = new SolidColorBrush(backgroundColor);

            var foreground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
            var foreGroundColor = System.Windows.Media.Color.FromArgb(foreground.A, foreground.R, foreground.G, foreground.B);
            btnIndexOf.Foreground = new SolidColorBrush(foreGroundColor);
            btnContains.Foreground = new SolidColorBrush(foreGroundColor);
            btnBoyerMoore.Foreground = new SolidColorBrush(foreGroundColor);
            btnNaive.Foreground = new SolidColorBrush(foreGroundColor);

            var textSearchForeground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextBrushKey);
            var textForeGroundColor = Color.FromArgb(textSearchForeground.A, textSearchForeground.R, textSearchForeground.G,
                textSearchForeground.B);
            txtSearchTerm.Foreground = new SolidColorBrush(textForeGroundColor);

        }

        /// <summary>
        ///     Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Default event handler naming pattern")]
        private void IndexOf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtSearchTerm.Text == string.Empty)
                {
                    MessageBox.Show("Please enter a search term.");
                }
                else
                {

                    var sw = new Stopwatch();

                    sw.Start();
                    var matches = FileSearch.IndexOfSearch(txtSearchTerm.Text, solutionContents);
                    sw.Stop();
                    var dt = new DataTable();

                    dt.Columns.Add("FileName", typeof(string));
                    dt.Columns.Add("LineNo", typeof(int));
                    dt.Columns.Add("Content", typeof(string));


                    foreach (var match in matches)
                    {
                        dt.Rows.Add(match.FileInfo.FullName, match.LineNo, match.LineContent);
                    }


                    ResultsStorage.ResultsDataTable = dt;


                    
                    MessageBox.Show(sw.Elapsed.ToString("g"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //d.populateDataGrid(dt);
        }

        private void ContainsSearch_Click(object sender, RoutedEventArgs e)
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
                    var matches = FileSearch.SearchFiles(txtSearchTerm.Text);
                    sw.Stop();

                    var dt = new DataTable();

                    dt.Columns.Add("FileName", typeof(string));
                    dt.Columns.Add("LineNo", typeof(int));
                    dt.Columns.Add("Content", typeof(string));

                    foreach (var match in matches)
                    {
                        dt.Rows.Add(match.FileInfo.FullName, match.LineNo, match.LineContent);
                    }


                    ResultsStorage.ResultsDataTable = dt;
                    Window window = new Window
                    {
                        Title = "Search Results",
                        Content = new ResultsControl()
                        
                    };
                  
                    window.Show();

                    
                    MessageBox.Show(sw.Elapsed.ToString("g"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBoyerMoore_Click(object sender, RoutedEventArgs e)
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

                    var matches = FileSearch.BoyerMooreSearch2(txtSearchTerm.Text, solutionContents);
                    sw.Stop();

                    var dt = new DataTable();

                    dt.Columns.Add("FileName", typeof(string));
                    dt.Columns.Add("LineNo", typeof(int));
                    dt.Columns.Add("Content", typeof(string));

                    foreach (var match in matches)
                    {
                        dt.Rows.Add(match.FileInfo.FullName, match.LineNo, match.LineContent);
                    }


                    ResultsStorage.ResultsDataTable = dt;
                    Window window = new Window
                    {
                        Title = "Search Results",
                        Content = new ResultsControl()

                    };

                    window.Show();
                    
                    MessageBox.Show(sw.Elapsed.ToString("g"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}