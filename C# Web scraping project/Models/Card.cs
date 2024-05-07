using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CSharp_Web_scraping_project.Models
{
    public class Card
    {
        [Key]
        public int CardDetailsID { get; set; }
        public string Title { get; set; }
        public string Rarity { get; set; }
        public int QuantityOfListings { get; set; }
        public decimal MarketPrice { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdateTime { get; set; }
        public int UserID { get; set; }

        public Card() { }

        public Card(string title, string rarity, int quantityOfListings, decimal marketPrice, DateTime updateTime)
        {
            Title = title;
            Rarity = rarity;
            QuantityOfListings = quantityOfListings;
            MarketPrice = marketPrice;
            UpdateTime = updateTime;
        }
    }
}
