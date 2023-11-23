/******************************************************************************
 * Filename    = FileUpload.xaml.cs
 * 
 * Author      = Sreelakshmi
 *
 * Product     = Analyzer
 * 
 * Project     = ContentPage
 *
 * Description = Page that lets user upload files for analysis
 *****************************************************************************/

using Content.Client;
using Networking.Communicator;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ContentPage
{
    /// <summary>
    /// Interaction logic for FileUpload.xaml
    /// </summary>
    public partial class FileUpload : Page
    {
        private readonly ContentClient _uploadClient;

        /// <summary>
        /// Initialses the page
        /// </summary>
        /// <param name="client">The input ICommuncator that can send messages to server</param>
        /// <param name="sessionID">Some unique identifier for this user</param>
        public FileUpload(ICommunicator client, string sessionID)
        {
            _uploadClient = new ContentClient(client, sessionID);
            InitializeComponent();
        }

        /// <summary>
        /// Handles a directory being provided to be uploaded
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event args</param>
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.CheckFileExists = false;
            string defaultFilename = "This folder";
            ofd.FileName = defaultFilename;
            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                string folderPath = ofd.FileName;
                Trace.WriteLine(folderPath);
                // Pass folder path to Content Client
                _uploadClient.HandleUpload(folderPath.Substring(0, folderPath.Length - 12));
            }
            
        }
    }
}
