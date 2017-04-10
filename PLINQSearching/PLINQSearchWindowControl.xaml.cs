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
using Microsoft.VisualStudio.Shell.Interop;

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
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            InitializeComponent();
            //solutionContents = FileSearch.GetAllFilesInFolder(FileSearch.GetSolutionDirectory(FileSearch.GetCurrentDTE()));

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
            btnSearch.Background = new SolidColorBrush(backgroundColor);
            btnRegEx.Background = new SolidColorBrush(backgroundColor);
            txtSearchTerm.Background = new SolidColorBrush(backgroundColor);

            var foreground = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
            var foreGroundColor = System.Windows.Media.Color.FromArgb(foreground.A, foreground.R, foreground.G, foreground.B);
            btnSearch.Foreground = new SolidColorBrush(foreGroundColor);
            btnRegEx.Foreground = new SolidColorBrush(foreGroundColor);

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
         
                    List<LineDetails> matches = new List<LineDetails>();

                    //9 is the number in which boyer-moore out performs IndexOf
                    if (txtSearchTerm.Text.Length > 9)
                    {
                        matches = FileSearch.BoyerMooreSearch2(txtSearchTerm.Text, solutionContents);
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

                    Window window = new Window
                    {
                        Title = "Search Results",
                        Content = new ResultsControl()

                    };

                    window.Show();
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
                    var matches = FileSearch.RegExStringSearch(txtSearchTerm.Text, solutionContents);

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
                    Window window = new Window
                    {
                        Title = "Search Results",
                        Content = new ResultsControl()

                    };

                    window.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Reloads the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            solutionContents = FileSearch.GetAllFilesInFolder(FileSearch.GetSolutionDirectory(FileSearch.GetCurrentDTE()));
        }
    }
}