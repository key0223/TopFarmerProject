using Microsoft.Identity.Client.Extensions.Msal;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopFarmerWebServer.DB
{
    public class DataModel
    {
        [Table("Account")]
        public class AccountDb
        {
            public int AccountDbId { get; set; }
            public string AccountName { get; set; }
            public string Password { get; set; }

            public ICollection<PlayerDb> Players { get; set; }

        }

        [Table("Player")]
        public class PlayerDb
        {
            public int PlayerDbId { get; set; }
            public string PlayerName { get; set; }
            public int Coin { get; set; }
            [ForeignKey("Account")]
            public int AccountDbId { get; set; }
            public AccountDb Account { get; set; }
            public ICollection<ItemDb> Items { get; set; }

        }


        [Table("Item")]
        public class ItemDb
        {
            public int ItemDbId { get; set; }
            public int TemplatedId { get; set; }
            public int Count { get; set; }
            public int Slot { get; set; } 
            public bool Equipped { get; set; } = false; 

            [ForeignKey("Owner")]
            public int? OwnerDbId { get; set; }
            public PlayerDb? Owner { get; set; }
        }
    }
}
