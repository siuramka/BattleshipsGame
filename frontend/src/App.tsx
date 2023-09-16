import React, { useEffect, useState } from "react";
import "./App.css";
import * as signalR from "@microsoft/signalr";
import { Ship } from "./enities/Ship";

const ship: Ship = {
  Coordinates: [
      { X: 3, Y: 5 },
      { X: 3, Y: 6 },
      { X: 3, Y: 7 },
      { X: 3, Y: 8 },
  ],
};
const ship1: Ship = {
  Coordinates: [
      { X: 5, Y: 5 },
      { X: 5, Y: 6 },
      { X: 5, Y: 7 },
  ],
};
const ship2: Ship = {
  Coordinates: [
      { X: 1, Y: 5 },
      { X: 1, Y: 6 },
  ],
};

const ships: Array<Ship> = [ship, ship1, ship2];

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
    hubConnection.on("GameStarted", () => {
      setClientMessage(`Game Started!`);
    });

    hubConnection.on("WaitingForOpponent", (playerName: string) => {
      setClientMessage(`Hello ${playerName}, waiting for opponent...`);
    });

    hubConnection.on("SetupShips", () => {
      console.log("sending ships")
      hubConnection.invoke("SetShipsOnBoard", ships).catch((err) => {
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
