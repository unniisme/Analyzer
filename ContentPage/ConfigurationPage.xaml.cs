/******************************************************************************
 * Filename    = ConfigurationPage.xaml.cs
 * 
 * Author      = Sreelakshmi
 *
 * Product     = Analyzer
 * 
 * Project     = ContentPage
 *
 * Description = This file contains the code-behind for the ConfigurationPage.xaml. 
 *               The ConfigurationPage displays configuration options for analyzers
 *               within the ContentPage project.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Analyzer;
using Content.Server;
using Networking.Communicator;

namespace ContentPage
{
    /// <summary>
    /// Interaction logic for ConfigurationPage.xaml
    /// </summary>
    public partial class ConfigurationPage : Page
    {
        private ContentServerViewModel viewModel;
        private IDictionary<int, bool> accumulatedOptions = new Dictionary<int, bool>();

        /// <summary>
        /// Initializes content Server ViewModel
        /// </summary>
        /// <param name="server">Running server</param>
        public ConfigurationPage(ContentServerViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        /// <summary>
        /// Handles the event when a checkbox associated with an analyzer configuration option is checked.
        /// Updates the accumulated options dictionary with the checked analyzer ID and triggers the view model
        /// to configure the analyzer with the updated options.
        /// </summary>
        /// <param name="sender">The checkbox element that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            
            var checkBox = sender as CheckBox;
            var analyzerItem = checkBox?.DataContext as AnalyzerConfigOption;

            if (analyzerItem != null && checkBox?.IsChecked == true)
            {
                
                int analyzerId = Convert.ToInt32(analyzerItem.AnalyzerId);
                accumulatedOptions[analyzerId] = true;
                viewModel.ConfigureAnalyzer(accumulatedOptions, true);
               
            }
        }

        /// <summary>
        /// Handles the event when a checkbox associated with an analyzer configuration option is unchecked.
        /// Removes the unchecked analyzer ID from the accumulated options dictionary and triggers the view model
        /// to configure the analyzer with the updated options.
        /// </summary>
        /// <param name="sender">The checkbox element that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var analyzerItem = checkBox?.DataContext as AnalyzerConfigOption;

            if (analyzerItem != null)
            {
                int analyzerId = Convert.ToInt32(analyzerItem.AnalyzerId);
                accumulatedOptions.Remove(analyzerId);

                viewModel.ConfigureAnalyzer(accumulatedOptions, true);
            }
        }


    }

}

