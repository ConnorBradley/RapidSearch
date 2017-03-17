//------------------------------------------------------------------------------
// <copyright file="PLINQSearchWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PLINQSearching
{
    /// <summary>
    ///     Interaction logic for PLINQSearchWindowControl.
    /// </summary>
    public partial class PLINQSearchWindowControl : UserControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PLINQSearchWindowControl" /> class.
        /// </summary>
        public PLINQSearchWindowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var matches = FileSearch.IndexOfSearch(textBox.Text);

                var dt = new DataTable();

                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("LineNo", typeof(int));
                dt.Columns.Add("Content", typeof(string));


                foreach (var match in matches)
                {
                    dt.Rows.Add(match.FileInfo.FullName, match.LineNo, match.LineContent);
                }


                ResultsStorage.ResultsDataTable = dt;

                sw.Stop();
                MessageBox.Show(sw.Elapsed.ToString("g"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            //d.populateDataGrid(dt);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var matches = FileSearch.SearchFiles(textBox.Text);

                var dt = new DataTable();

                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("LineNo", typeof(int));
                dt.Columns.Add("Content", typeof(string));

                foreach (var match in matches)
                {
                    dt.Rows.Add(match.FileInfo.FullName, match.LineNo, match.LineContent);
                }


                ResultsStorage.ResultsDataTable = dt;

                sw.Stop();
                MessageBox.Show(sw.Elapsed.ToString("g"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}