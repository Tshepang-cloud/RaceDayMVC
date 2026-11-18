using System;

namespace RacedayMvc.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string BannerUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
    }
}