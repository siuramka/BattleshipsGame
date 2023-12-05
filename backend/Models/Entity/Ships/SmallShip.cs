using backend.Models.Entity.Bombs;
using backend.Models.Entity.Bombs.SmallBomb;
using backend.Strategies;
using backend.Strategies.Attacks;
using backend.Strategies.Ships;
using backend.Visitor;
using Shared;

namespace backend.Models.Entity.Ships;

public class SmallShip : Ship
{
    public SmallShip()
    {
        Stats = new Statistics(1000);
    }

    public SmallShip(SmallShip ship) {
        Size = ship.Size;
        IsVertical = ship.IsVertical;
        ShipType = ship.ShipType;
        foreach(ShipCoordinate coordinate in ship.GetCoordinates())
        {
            AddCoordinate(coordinate);
        }
    }

    public override Ship DeepCopy()
    {
        SmallShip ship = new SmallShip(this);
        List<ShipCoordinate> coords = GetCoordinates();
        RemoveAllCoordinates();
        foreach (ShipCoordinate coordinate in coords)
        {
            AddCoordinate(coordinate);
        }
        return ship;
    }

    public override AttackTemplate GetAttackTemplate()
    {
        return new SmallShipAttack();
    }

    public override Ship ShallowCopy()
    {
        return new SmallShip(this);
    }

    public override Ship GetShip(ShipCoordinate c)
    {
        if (Coordinates.Contains(c))
        {
            return this;
        }
        return null;
    }

    public override int Accept(ShipInspector shipInspector)
    {
        return shipInspector.visit(this);
    }
}