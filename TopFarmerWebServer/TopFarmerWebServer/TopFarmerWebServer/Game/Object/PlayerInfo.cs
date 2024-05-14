using static Define;

namespace TopFarmerWebServer.Game
{
    public class PlayerInfo
    {
        public int PlayerDbId { get; set; }
        public string PlayerName { get; set; }
        public int Coin { get; set; }
        public StatInfo Stat { get; set; } = new StatInfo();
        //public Inventory Inven { get; set; } = new Inventory();
    }
}
