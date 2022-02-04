using BikeshareClient;
using BikeshareClient.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace OsloBysykkel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public async Task OnGetAsync()
        {
            // Find a GBFS feed url. This one belongs to Los Angeles Metro
            var oslobysykkelFeedUrl = "https://gbfs.urbansharing.com/oslobysykkel.no/gbfs.json";

            // Create the client from a GBFS API URL.
            IBikeshareClient client = new Client(oslobysykkelFeedUrl);

            // Get available stations, containing name, id, lat, long, address and capacity
            var stations = await client.GetStationsAsync();

            // Get stations status, containing number of bikes and docks available, is renting, is returning etc.
            var statuses = await client.GetStationsStatusAsync();

            ViewData["Stations"] = stations;
            ViewData["Statuses"] = statuses;


            var model = new FeatureCollection();
            foreach (Station station in stations)
            {
                var geom = new Point(new Position(station.Latitude, station.Longitude));

                var props = new Dictionary<string, object>
                {
                    { "title", station.Name },
                    { "Address", station.Address},
                    { "Capacity", station.Capacity},
                    { "DocksAvailable", "TO FILL"}
                };

                var feature = new Feature(geom, props);
                model.Features.Add(feature);
            }

            var serializedData = JsonConvert.SerializeObject(model);
            ViewData["serializedData"] = serializedData;
            /*
            // combine the station and status information on StationId
            statuses.Join(stations, s => s.Id, s => s.Id, (status, station) => new
            {
                Station = station,
                Status = status,
            })
                .ToList()
                // Write out some information about the stations
                .ForEach(x => 
                    Console.WriteLine($"Station at {x.Station.Name} has {x.Status.BikesAvailable} bikes available and {x.Status.DocksAvailable} docks available")
                );*/
            }
            
    }
}