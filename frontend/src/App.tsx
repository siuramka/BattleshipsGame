import React, { useEffect, useState } from "react";
import "./App.css";
import * as signalR from "@microsoft/signalr";
import { IGameBoard } from "./enities/IGameBoard";

const exampleGameBoardGrid: IGameBoard = {
  Grid: [
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 1],
    [0, 0, 0, 0, 1, 0, 0, 0, 0, 1],
    [0, 0, 0, 0, 1, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 1, 0, 0, 0, 0, 1],
    [0, 0, 0, 0, 1, 0, 0, 0, 0, 1],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 1],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 1],
    [0, 0, 0, 0, 1, 1, 1, 1, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
  ],
};

const App: React.FC = () => {
  const [clientMessage, setClientMessage] = useState<string | null>(null);

  useEffect(() => {
    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5220/hub")
      .configureLogging(signalR.LogLevel.Information)
      .build();

    hubConnection.start().then(() => {
      if (hubConnection.connectionId) {
        hubConnection.invoke("JoinGame");
      }
    });

    // Listen for the "WaitingForOpponent" event
    hubConnection.on("WaitingForOpponent", (playerName: string) => {
      setClientMessage(`Hello ${playerName}, waiting for opponent...`);
    });

    hubConnection.on("SetupShips", () => {
      console.log("sending ships")
      hubConnection.invoke("SetShipsOnBoard", exampleGameBoardGrid).catch((err) => {
        console.log("setships" + err)
      })
    });


    return () => {
      // Clean up the connection when the component unmounts
      hubConnection.stop();
    };
  }, []); // Empty dependency array to run this effect only once

  return (
    <div>
      {clientMessage && <p>{clientMessage}</p>}
    </div>
  );
};

export default App;
