using JukeBox.Core;
using Kentico.Kontent.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jukebox.Domain
{
    public static class JukeboxHelpers
    {
        public static async Task<string> GetStringifiedSongList(string kontentProjectId)
        {
            IDeliveryClient client = DeliveryClientBuilder.WithProjectId(kontentProjectId).Build();

            DeliveryItemListingResponse<Song> listingResponse = await client.GetItemsAsync<Song>();

            var songs = listingResponse.Items.OrderBy(x => x.TrackNumber).Select(x => $"{x.TrackNumber} - {x.Title}").ToArray();

            return ConvertToNumberedList(songs);

            string ConvertToNumberedList(IEnumerable<string> songList)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var s in songList)
                {
                    sb.AppendLine(s);
                }
                return sb.ToString();
            }
        }
        public static async Task<Song> GetSong(string kontentProjectId, int trackNumber)
        {
            IDeliveryClient client = DeliveryClientBuilder.WithProjectId(kontentProjectId).Build();
                        
            DeliveryItemListingResponse<Song> song = await client.GetItemsAsync<Song>(new EqualsFilter("elements.track_number", trackNumber.ToString()));
            return song.Items[0];
        }
    }
}
