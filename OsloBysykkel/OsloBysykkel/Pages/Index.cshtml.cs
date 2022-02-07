using BikeshareClient;
using BikeshareClient.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Extensions.Configuration:

namespace OsloBysykkel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public async Task OnGetAsync()
        {

            //get GBFS feed url from settings, otherwise use default
            var SettingsUrl = _configuration.GetSection("OsloBysykkelSettings").GetSection("URL").Value;
            var oslobysykkelFeedUrl = SettingsUrl != null ? SettingsUrl : "https://gbfs.urbansharing.com/oslobysykkel.no/gbfs.json";

            // Create the client from a GBFS API URL.
            IBikeshareClient client = new Client(oslobysykkelFeedUrl);

            // Get available stations
            var stations = await client.GetStationsAsync();

            // Get statusses of stations
            var statuses = await client.GetStationsStatusAsync();

            //ViewData["Stations"] = stations;
            //ViewData["Statuses"] = statuses;

            //Combine statusestatuses with stations
            var stationStatuses = statuses.Join(stations, s => s.Id, s => s.Id, (status, station) => new
            {
                Station = station,
                Status = status,
            }).ToList();

            //create model to generate GeoJSON
            var model = new FeatureCollection();
            foreach (var station in stationStatuses)
            {
                var geom = new Point(new Position(station.Station.Latitude, station.Station.Longitude));
                var props = new Dictionary<string, object>
                {
                    { "Title", station.Station.Name },
                    { "Address", station.Station.Address},
                    { "Capacity", station.Station.Capacity},
                    { "DocksAvailable", station.Status.DocksAvailable},
                    { "BikesAvailable", station.Status.BikesAvailable},
                };

                var feature = new Feature(geom, props);
                model.Features.Add(feature);
            }

            //Serialize GeoJSON
            var serializedData = JsonConvert.SerializeObject(model);
            ViewData["serializedData"] = serializedData;
        }
    }
}