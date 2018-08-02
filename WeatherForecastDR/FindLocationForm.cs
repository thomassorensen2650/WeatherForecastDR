using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace WeatherForecastDR
{
    public partial class FindLocationForm : Form
    {
        const string googleApiKey = "AIzaSyA5y79x4M1agAxXkwB21skuFonN1hBFSr0";
        const string googleGeoBaseUrl = @"https://maps.googleapis.com/maps/api/geocode/json?";

        WeatherDR _dr;
        public GeoSearchItem SelectedItem { get; set; }
        WebClient webClient = new WebClient();

        public FindLocationForm(WeatherDR dr, bool isReadyOnly)
        {
            _dr = dr;
            
            InitializeComponent();
            webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
            searchTextbox.Text = dr.SearchString;
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            searchButton.Enabled = true;

            if (e.Cancelled)
            {
                return;
            }

            if (e.Error != null)
            {
                resultDataGridView.DataSource = null;
                MessageBox.Show(e.Error.ToString());
            }
            else
            {
                var j = JObject.Parse(e.Result);
                if (j["status"].ToString() != "OK")
                {
                    MessageBox.Show(j["status"].ToString());
                    return;
                }

                var results = j["results"].Select(sf => (JObject)sf).Select(sf =>
                    new GeoSearchItem()
                    {
                        Address = sf["formatted_address"].ToString(),
                        LocationLat = sf["geometry"]["location"]["lat"].Value<float>(),
                        LocationLng = sf["geometry"]["location"]["lng"].Value<float>()
                    });
                resultDataGridView.DataSource = results.ToArray();
            }
        }

        private void searchTextbox_TextChanged(object sender, EventArgs e)
        {
            // Hide button when empty search string
            searchButton.Enabled = !string.IsNullOrWhiteSpace(((TextBox)sender).Text);
        }

        private void resultDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            okButton.Enabled = resultDataGridView.SelectedRows.Count > 0;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // Build URL.\ Encoding will be done by webclient
            var url = new Uri($"{googleGeoBaseUrl}address={searchTextbox.Text}&Key={googleApiKey}");

            // Dowload Data
            webClient.DownloadStringAsync(url);
            searchButton.Enabled = false; // Not the best design, but OK for a demo
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SelectedItem = resultDataGridView.SelectedRows[0].DataBoundItem as GeoSearchItem;
            _dr.Lng = SelectedItem.LocationLng;
            _dr.Lat = SelectedItem.LocationLat;
            _dr.SearchString = searchTextbox.Text;
            _dr.ConfigString = _dr.ConfigString; // Force ui to update
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
