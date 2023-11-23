/******************************************************************************
 * Filename    = ResultPage.xaml.cs
 * 
 * Author      = Sreelakshmi
 *
 * Product     = Analyzer
 * 
 * Project     = ContentPage
 *
 * Description = This file contains the code-behind for the ResultPage.xaml, 
 *               which visualizes the result of an analysis within the ContentPage project.
 *****************************************************************************/



using System.Windows.Controls;
using Analyzer;
using Content.Server;
using Networking.Communicator;

using System;
using System.Collections.Generic;
using System.Windows;


namespace ContentPage
{
    /// <summary>
    /// Represents a page within the application responsible for loading and managing ResultPage and ConfigurationPage.
    /// </summary>
    public partial class ClientPage : Page
    {
        private ContentServerViewModel viewModel;

        /// <summary>
        /// Constructor for ClientPage that initializes the page and sets up necessary components.
        /// </summary>
        /// <param name="server">An ICommunicator instance used for communication with the server.</param>
        public ClientPage(ICommunicator server)
        {
            InitializeComponent();
            viewModel = new ContentServerViewModel(server);
            DataContext = viewModel;

            LoadResultPage(); // Load ResultPage initially
            LoadConfigurationPage(); // Optionally, load ConfigurationPage initially
        }


        /// <summary>
        /// Loads and navigates to the ResultPage within the ResultFrame.
        /// </summary>
        private void LoadResultPage()
        {
            ResultPage resultPage = new ResultPage(viewModel);
            ResultFrame.NavigationService.Navigate(resultPage);
            
        }

        /// <summary>
        /// Loads and navigates to the ConfigurationPage within the ConfigFrame.
        /// </summary>
        private void LoadConfigurationPage()
        {
            ConfigurationPage configPage = new ConfigurationPage(viewModel);
            ConfigFrame.NavigationService.Navigate(configPage);
            
        }
    }
}

