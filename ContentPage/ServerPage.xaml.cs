﻿/******************************************************************************
 * Filename    = ServerPage.xaml.cs
 * 
 * Author      = Sreelakshmi
 *
 * Product     = Analyser
 * 
 * Project     = ContentPage
 *
 * Description = This file contains the code-behind for the ServerPage.xaml.
 *             
 *****************************************************************************/

using System.Windows.Controls;
using Content.ViewModel;
using Networking.Communicator;
using System.Windows.Forms;
using System.Collections.Generic;
using Analyzer;
using Content.Model;
using System.Windows.Media.Imaging;
using System.Windows;
using System;

namespace ContentPage
{
    /// <summary>
    /// Logic for Server main page
    /// </summary>
    public partial class ServerPage : Page
    {
        private readonly ContentServerViewModel _viewModel;

        /// <summary>
        /// Create a server page instance.
        /// Refer <see cref="SetSessionID"/> on how to change result to each client's
        /// </summary>
        /// <param name="server">Running networking server</param>
        public ServerPage(ICommunicator server, string sessionID)
        {
            InitializeComponent();
            _viewModel = new ContentServerViewModel(
                new ContentServer( server, AnalyzerFactory.GetAnalyzer(), sessionID )
                );
            DataContext = _viewModel;

            LoadResultPage(); // Load ResultPage initially
            LoadConfigurationPage(); // Optionally, load ConfigurationPage initially
        }

        /// <summary>
        /// Set the session/client ID that the server is currently viewing and update result tabs
        /// 
        /// Note that this function has to be called first for server to show any result
        /// </summary>
        /// <param name="sessionID">Session ID or Client ID</param>
        public void SetSessionID(string sessionID)
        {
            _viewModel.SetSessionID(sessionID);
        }

        /// <summary>
        /// Loads the ResultPage into the ResultFrame.
        /// </summary>
        private void LoadResultPage()
        {
            ResultPage resultPage = new (_viewModel);
            ResultFrame.NavigationService.Navigate(resultPage);
            
        }
        /// <summary>
        /// Loads the ConfigurationPage into the ConfigFrame.
        /// </summary>
        private void LoadConfigurationPage()
        {
            ConfigurationPage configPage = new (_viewModel);
            ConfigFrame.NavigationService.Navigate(configPage);
            
        }

        /// <summary>
        /// Event handler for the AnalyzerUploadButton click. Allows uploading DLL files for analysis.
        /// </summary>
        private void AnalyzerUploadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true , // Allow multiple file selection
                Filter = "DLL files (*.dll)|*.dll|All files (*.*)|*.*" // Filter for DLL files
            };

            // Show the dialog and get the result
            DialogResult result = openFileDialog.ShowDialog();

            // Process the selected files
            if (result == DialogResult.OK)
            {
                List<string> filePaths = new (openFileDialog.FileNames);
                _viewModel.LoadCustomDLLs(filePaths);

            }
        }

        /// <summary>
        /// Event handler for the SendToCloudButton click. Initiates sending data to the cloud.
        /// </summary>
        private void SendToCloudButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.SendToCloud();
        }

        /// <summary>
        /// Event handler for the "Show Class Diagram" button click.
        /// Displays the class diagram image in a new window if the image path is available in the ClientServerViewModel.
        /// If the path is not available, shows a message to the user indicating the unavailability of the class diagram.
        /// </summary>
        /// <param name="sender">The button that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ShowClassDiagramButton_Click( object sender , RoutedEventArgs e )
        {
            string classDiagramPath = _viewModel.ClassDiagramImagePath; // Access the path from the view model

            if (!string.IsNullOrEmpty( classDiagramPath ))
            {
                Window diagramWindow = new()
                {
                    Title = "Class Diagram" ,
                    Width = 600 ,
                    Height = 400 ,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                Image diagramImage = new();
                BitmapImage bitmapImage = new( new Uri( classDiagramPath , UriKind.RelativeOrAbsolute ) );
                diagramImage.Source = bitmapImage;

                diagramWindow.Content = diagramImage;

                diagramWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show( "Class diagram image path is not available." ); // Show a message if the path is not available
            }
        }

    }
}

