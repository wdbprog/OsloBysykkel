using BikeshareClient;
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
            //GBFS feed url, move to appsettings in bigger project
            var osloBysykkelFeedUrl = "https://gbfs.urbansharing.com/oslobysykkel.no/gbfs.json";

            // Create the client from a GBFS API URL.
            IBikeshareClient client = new Client(osloBysykkelFeedUrl);

            // Get available stations
            var stations = await client.GetStationsAsync();

            // Get statuses of stations
            var statuses = await client.GetStationsStatusAsync();

            //Combine statuses with stations
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
            var geoJson = JsonConvert.SerializeObject(model);
            ViewData["GeoJson"] = geoJson;
        }
    }
}