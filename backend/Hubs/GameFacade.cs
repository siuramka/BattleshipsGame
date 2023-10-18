using backend.Manager;
using backend.Models.Entity.Ships.Factory;
using backend.Models.Entity.Ships;
using Shared.Transfer;
using backend.Service;
using backend.Models.Entity;

namespace backend.Hubs
{
    public class GameFacade
    {
        private GameManager _gameManager;
        private ShipFactory _shipFactory;
        private string CurrentPlayerConnectionId;
        
        public GameFacade(string connectionId)
        {
            CurrentPlayerConnectionId = connectionId;
            _shipFactory = new ConcreteShipFactory();
            _gameManager = GameManager.Instance;
        }

        public Player GetCurrentPlayer()
        {
            return _gameManager.GetPlayer(CurrentPlayerConnectionId);
        }
        public Game GetCurrentGame()
        {
            return _gameManager.GetPlayerGame(CurrentPlayerConnectionId);
        }
        public Player GetEnemyPlayer()
        {
            var currentPlayer = GetCurrentPlayer();
            var currentGame = GetCurrentGame();

            return currentGame.GetEnemyPlayer(currentPlayer);
        }

        public List<ShipCoordinate> MakeMove(MakeMove move)
        {
            Ship ship = _shipFactory.GetShip(move.TypeOfShip);
            if (move.IsVertical)
            {
                ship.SetVertical();
            }

            var enemyPlayer = GetEnemyPlayer();
            var enemyBoard = enemyPlayer.OwnBoard;

            enemyBoard.SetEnemyAttackShip(ship);
            enemyBoard.ClearMissedCoordinates();//clear missed coordinates so they dont duplicate

            GameBoardService gameBoardService = new GameBoardService(enemyBoard);

            return gameBoardService.GetHitCoordinates(move.X, move.Y, move.AttackBomb);
        }

        public bool HasGameEnded()
        {
            var enemyPlayer = GetEnemyPlayer();
            var enemyBoard = enemyPlayer.OwnBoard;

            return enemyBoard.HaveAllShipsSunk;
        }

        public List<ShipCoordinate> GetMissedCoordinates()
        {

            var enemyPlayer = GetEnemyPlayer();
            var enemyBoard = enemyPlayer.OwnBoard;

            return enemyBoard.GetMissedCoordinates();
        }
        public List<ShipCoordinate> GetEnemyShipCoordinates()
        {
            var enemyPlayer = GetEnemyPlayer();
            var enemyBoard = enemyPlayer.OwnBoard;
            var enemyShips = enemyBoard.GetShips();

            List<ShipCoordinate> shipCoordinates = new List<ShipCoordinate>();
            foreach (var ship in enemyShips)
            {
                foreach (var shipCoordinate in ship.GetCoordinates())
                {
                    shipCoordinates.Add(shipCoordinate);
                }
            }

            return shipCoordinates;
        }
    }
}
