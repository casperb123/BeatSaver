using Newtonsoft.Json;

namespace BeatSaver.Entities
{
    public class OnlineBeatmaps
    {
        [JsonProperty("docs")]
        public OnlineBeatmap[] Maps { get; set; }
        [JsonProperty("totalDocs")]
        public int TotalMaps { get; set; }
        public int LastPage { get; set; }
        public int? PrevPage { get; set; }
        public int? NextPage { get; set; }
        public int CurrentPage
        {
            get
            {
                if (!NextPage.HasValue && !PrevPage.HasValue)
                    return 0;

                return NextPage.HasValue ? (NextPage.Value - 1) : (PrevPage.Value + 1);
            }
        }
        public int CurrentPageReal
        {
            get { return CurrentPage + 1; }
        }
        public int LastPageReal
        {
            get { return LastPage + 1; }
        }
    }
}