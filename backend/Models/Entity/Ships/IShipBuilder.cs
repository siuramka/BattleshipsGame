using Shared;

namespace backend.Models.Entity.Ships
{
    public interface IShipBuilder
    {
        IShipBuilder SetSize(int size);
        IShipBuilder SetIsVertical(bool isVertical);
        IShipBuilder SetType(ShipType type);
        Ship Build();
    }
}
