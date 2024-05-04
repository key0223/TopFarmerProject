using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using TopFarmerWebServer.DB;
using TopFarmerWebServer.Game;

namespace TopFarmerWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {

        AppDbContext _context;
        IConfiguration _configuration;
        IDatabase _redis;
        public RedisController(AppDbContext context, IConfiguration configuration, IDatabase redis)
        {
            _context = context;
            _configuration = configuration;
            _redis = redis;
        }

        public void SetPlayer(PlayerInfo player)
        {
            string json = JsonConvert.SerializeObject(player);
            _redis.HashSet("players", player.PlayerDbId, json);
        }

        public PlayerInfo GetPlayer(int playerDbId)
        {
            string playerJson = _redis.HashGet("Players", playerDbId);
            return JsonConvert.DeserializeObject<PlayerInfo>(playerJson);

        }

        /*
        public void AddItem(int playerDbId, Item item)
        {
            Player player = GetPlayer(playerDbId);
            if (player != null)
            {
                //player.Inven.Add(item);
            }
        }

        */
    }
    }
