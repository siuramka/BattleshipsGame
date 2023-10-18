using backend.Models.Entity;
using backend.Models.Entity.Bombs;
using backend.Models.Entity.Ships;
using backend.Strategies;
using Shared;

namespace backend.Service
{
    public class GameBoardService
    {
        private GameBoard _gameBoard;

        public GameBoardService(GameBoard gameBoard)
        {
            _gameBoard = gameBoard ?? throw new ArgumentNullException(nameof(gameBoard));
        }

        public List<ShipCoordinate> GetHitCoordinates(int x, int y, BombType attackBomb)
        {
            List<ShipCoordinate> hitableCoordinates = GetHitableCordinates(x, y, attackBomb);
            List<ShipCoordinate> hitShipCoordinates = new List<ShipCoordinate>();


            foreach (var ship in _gameBoard.GetShips())
            {
                foreach (var hitableCoord in hitableCoordinates)
                {
                    if (ship.CanHitCoordinate(hitableCoord.X, hitableCoord.Y))
                    {
                        ship.HitCoordinate(hitableCoord.X, hitableCoord.Y);
                        hitShipCoordinates.Add(hitableCoord);
                    }
                    else
                    {
                        _gameBoard.AddMissed(hitableCoord);
                    }
                }
            }

            foreach (var miss in _gameBoard.GetMissedCoordinates())
            {
                if (hitShipCoordinates.Contains(miss))
                {
                    _gameBoard.RemoveMissed(miss);
                }
            }


            return hitShipCoordinates;
        }
        private List<ShipCoordinate> GetHitableCordinates(int x, int y, BombType attackBomb)
        {
            var enemyAttackShip = _gameBoard.GetEnemyAttackShip();

            if (enemyAttackShip == null)
            {
                throw new InvalidOperationException("Enemy attack ship not set.");
            }

            BombFactory factory = enemyAttackShip.GetShipBombFactory();
            IAttackStrategy attackStrategy = enemyAttackShip.GetAttackStrategy();

            if (attackBomb == BombType.MissileBomb)
            {
                MissileBomb missileBomb = factory.CreateMissileBomb();
                return attackStrategy.TargetShip(x, y, missileBomb, _gameBoard.maxSizeX, _gameBoard.maxSizeY);
            }

            if (attackBomb == BombType.AtomicBomb)
            {
                AtomicBomb atomicBomb = factory.CreateAtomicBomb();
                return attackStrategy.TargetShip(x, y, atomicBomb, _gameBoard.maxSizeX, _gameBoard.maxSizeY);
            }

            throw new ArgumentNullException(nameof(BombType));
        }
    }
}
