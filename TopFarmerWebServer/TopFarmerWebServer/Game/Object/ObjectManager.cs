using static Define;

namespace TopFarmerWebServer.Game
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();

        object _lock = new object();
        Dictionary<int, PlayerInfo> _players = new Dictionary<int, PlayerInfo>();

        //[UNUSED(1)][TYPE(7)][ID(24)]
        int _counter = 0;

        public T Add<T>() where T : GameObject, new()
        {
            T gameObejct = new T();

            lock (_lock)
            {
                //gameObejct.Id = GenerateId(gameObejct.ObjectType);
                if (gameObejct.ObjectType == ObjectType.OBJECT_TYPE_PLAYER)
                {
                    //_players.Add(gameObejct.Id, gameObejct as Player);
                }
            }
            return gameObejct;

        }
        int GenerateId(ObjectType type)
        {
            lock (_lock)
            {
                return ((int)type << 24) | (_counter++);
            }
        }
        public static ObjectType GetObjectTypeById(int id)
        {
            int type = (id >> 24) & 0x7F;
            return (ObjectType)type;
        }

        public bool Remove(int objectId)
        {
            ObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == ObjectType.OBJECT_TYPE_PLAYER)
                    return _players.Remove(objectId);
            }
            return false;
        }
        public PlayerInfo Find(int objectId)
        {
            ObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == ObjectType.OBJECT_TYPE_PLAYER)
                {
                    PlayerInfo player = null;
                    if (_players.TryGetValue(objectId, out player))
                        return player;
                }

                return null;
            }
        }
    }
}

