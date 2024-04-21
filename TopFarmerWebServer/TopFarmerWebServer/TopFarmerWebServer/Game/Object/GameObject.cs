using TopFarmerWebServer.Game.Object;
using static Define;

namespace TopFarmerWebServer.Game
{

    public class GameObject
    {
        public ObjectType ObjectType { get; protected set; } = ObjectType.OBJECT_TYPE_NONE;
        public ObjectInfo Info { get; private set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();

        public GameObject()
        {
            Info.posInfo = PosInfo;
        }

        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.posX,PosInfo.posY);
            }
            set
            {
                PosInfo.posX = value.x;
                PosInfo.posY = value.y;
            }
        }

        public virtual GameObject GetOwner()
        {
            return this;
        }
    }

   
}
