import * as signalR from "@microsoft/signalr";

import {
  FC,
  ReactNode,
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";

type CShip = {
  Coordinates: { X: number; Y: number }[];
  Name: string;
};

const ship: CShip = {
  Coordinates: [
    { X: 3, Y: 5 },
    { X: 3, Y: 6 },
    { X: 3, Y: 7 },
    { X: 3, Y: 8 },
  ],
  Name: "test",
};

const ship1: CShip = {
  Coordinates: [
    { X: 5, Y: 5 },
    { X: 5, Y: 6 },
    { X: 4, Y: 7 },
  ],
  Name: "test",
};

const ship2: CShip = {
  Coordinates: [
    { X: 2, Y: 5 },
    { X: 1, Y: 6 },
  ],
  Name: "test",
};

export const GameContext = createContext({
  // eslint-disable-next-line no-empty-pattern
  ownShips: [] as CShip[],
  opponentShips: [] as CShip[],
  clientMessage: "",
  isGameStarted: false,
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  setClientMessage: (_: string) => {
    return;
  },
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  setIsGameStarted: (_: boolean) => {
    return;
  },
  hubConnection: {} as signalR.HubConnection,
});

type Props = {
  children?: ReactNode;
};

export const GameProvider: FC<Props> = ({ children }) => {
  const [ownShips] = useState<CShip[]>([ship, ship1, ship2]);
  const [opponentShips, setOpponentShips] = useState<CShip[]>([]);
  const [clientMessage, setClientMessage] = useState<string>("");
  const [isGameStarted, setIsGameStarted] = useState<boolean>(false);
  const hubConnection = useRef<signalR.HubConnection>();

  useEffect(() => {
    hubConnection.current = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5220/hub")
      .configureLogging(signalR.LogLevel.Information)
      .build();

    hubConnection.current.start().then(() => {
      if (hubConnection.current?.connectionId) {
        hubConnection.current?.invoke("JoinGame");
      }
    });

    // Listen for the "WaitingForOpponent" event
    hubConnection.current.on("GameStarted", () => {
      setClientMessage("");
      setIsGameStarted(true);
    });

    hubConnection.current.on("ReturnMove", (data) => {
      const { isHit, row, column } = data;
      if (!isHit) {
        return;
      }

      console.log(data);
      setOpponentShips([
        ...opponentShips,
        // TODO: wrong coordinate arangement
        { Coordinates: [{ Y: column, X: row }], Name: "enemy" },
      ]);
    });

    hubConnection.current.on("WaitingForOpponent", (playerName: string) => {
      setClientMessage(`Hello ${playerName}, waiting for opponent...`);
    });

    hubConnection.current.on("SetupShips", () => {
      console.log("sending ships");
      hubConnection.current
        ?.invoke("SetShipsOnBoard", ownShips)
        .catch((err) => {
          console.log("setships" + err);
        });
    });

    return () => {
      // Clean up the connection when the component unmounts
      hubConnection.current?.stop();
    };
  }, [
    hubConnection,
    opponentShips,
    ownShips,
    setClientMessage,
    setIsGameStarted,
  ]);

  return (
    <GameContext.Provider
      value={{
        ownShips,
        opponentShips,
        clientMessage,
        setClientMessage,
        setIsGameStarted,
        isGameStarted,
        hubConnection: hubConnection.current as signalR.HubConnection,
      }}
    >
      {children}
    </GameContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useGameContext = () => useContext(GameContext);
