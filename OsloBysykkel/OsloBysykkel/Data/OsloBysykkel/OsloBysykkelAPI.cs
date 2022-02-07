using BikeshareClient;
using BikeshareClient.Models;
/*
namespace OsloBysykkel.Data.OsloBysykkel
{*/
    public class OsloBysykkelAPI
    {

        public async Task<Station> getStations()
        {
            // Find a GBFS feed url. This one belongs to Los Angeles Metro
            var oslobysykkelFeedUrl = "https://gbfs.urbansharing.com/oslobysykkel.no/gbfs.json";

            // Create the client from a GBFS API URL.
            IBikeshareClient client = new Client(oslobysykkelFeedUrl);

            // Get available stations, containing name, id, lat, long, address and capacity
            return (Station)await client.GetStationsAsync();
        }
    }
//}
