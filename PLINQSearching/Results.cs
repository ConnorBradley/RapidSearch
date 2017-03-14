//------------------------------------------------------------------------------
// <copyright file="Results.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace PLINQSearching
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("75d1b6e7-a58e-4afc-b4ee-0ee66733871d")]
    public class Results : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Results"/> class.
        /// </summary>
        public Results() : base(null)
        {
            this.Caption = "Results";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ResultsControl();
        }
    }
}
