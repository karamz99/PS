using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PS.Data
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        //Data Usage
        [Display(Name = "Control data usage")]
        public bool DataUsageControl { get; set; }

        [Display(Name = "Data Usage")]
        [Range(0, 1024)]
        public long DataUsage { get; set; }
        public DataUsageUnits DataUsageUnit { get; set; }
        [NotMapped]
        public long DataUsageInBytes => DataUsage * (long)Math.Pow(1024, (int)DataUsageUnit);

        //Time Usage
        [Display(Name = "Control time usage")]
        public bool TimeUsageControl { get; set; }

        [Display(Name = "Start time")]
        [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TimeUsageStart { get; set; }

        [Display(Name = "Stop time")]
        [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TimeUsageEnd { get; set; }

        public List<Customer>? Customers { get; set; }

        [NotMapped]
        public bool Saving { get; set; }

        [NotMapped]
        public bool InValidTime => TimeUsageControl && (ConvertTime(DateTime.Now) < ConvertTime(TimeUsageStart) || ConvertTime(DateTime.Now) > ConvertTime(TimeUsageEnd));

        private int ConvertTime(DateTime date) => date.Hour * 60 + date.Minute;
    }

    public enum DataUsageUnits { Byte, KB, MB, GB, }
}
