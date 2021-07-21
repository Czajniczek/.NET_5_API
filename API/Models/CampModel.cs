using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        //public int CampId { get; set; }

        //public string Name { get; set; }

        //public string Moniker { get; set; }

        //public Location Location { get; set; }

        //public DateTime EventDate { get; set; } = DateTime.MinValue;

        //public int Length { get; set; } = 1;

        //public ICollection<Talk> Talks { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Moniker { get; set; }

        public DateTime EventDate { get; set; } = DateTime.MinValue;

        [Range(1, 100)]
        public int Length { get; set; } = 1;



        //public int LocationId { get; set; }

        //public string VenueName { get; set; }

        //public string Address1 { get; set; }

        //public string Address2 { get; set; }

        //public string Address3 { get; set; }

        //public string CityTown { get; set; }

        //public string StateProvince { get; set; }

        //public string PostalCode { get; set; }

        //public string Country { get; set; }

        public string Venue { get; set; }

        public string LocationAddress1 { get; set; }

        public string LocationAddress2 { get; set; }

        public string LocationAddress3 { get; set; }

        public string LocationCityTown { get; set; }

        public string LocationStateProvince { get; set; }

        public string LocationPostalCode { get; set; }

        public string LocationCountry { get; set; }



        public ICollection<TalkModel> Talks { get; set; }
    }
}
