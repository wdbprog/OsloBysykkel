using BikeshareClient;
using BikeshareClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

            // combine the station and status information on StationId
            statuses.Join(stations, s => s.Id, s => s.Id, (status, station) => new
            {
                Station = station,
                Status = status,
            })
                .ToList()
                // Write out some information about the stations
                .ForEach(x => Console.WriteLine($"Station at {x.Station.Name} has {x.Status.BikesAvailable} bikes available and {x.Status.DocksAvailable} docks available"));
        }
    }
    class Combinedview
    {
        Station Station;
        StationStatus Status;
    }
}