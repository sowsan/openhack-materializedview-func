using System;
using System.Collections.Generic;
using System.Text;

namespace MaterializedViewFunction
{
    public class Movie
    {
        public int ItemId { get; set; }
        public int VoteCount { get; set; }
        public string ProductName { get; set; }
        public int ImdbId { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public object ThumbnailPath { get; set; }
        public double UnitPrice { get; set; }
        public int CategoryId { get; set; }
        public object Category { get; set; }
        public double Popularity { get; set; }
        public string OriginalLanguage { get; set; }
        public DateTime ReleaseDate { get; set; }
        public double VoteAverage { get; set; }
      
    }
}
