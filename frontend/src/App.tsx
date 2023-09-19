import React from "react";
import Display from "./components/Display";
import Heading from "./components/Heading";
import "./App.css";
import { Box } from "@mui/material";

import { useGameContext } from "./contexts/gameContext";

const App: React.FC = () => {
  const { isGameStarted, clientMessage, ownShips, opponentShips } =
    useGameContext();

  if (!isGameStarted) {
    return <Box sx={{ color: "white" }}>{clientMessage || ""}</Box>;
  }

  console.log(opponentShips);

  return (
    <>
      <Heading />
      <Display
        myState={{
          placedShips: ownShips.map(({ Coordinates, Name }) => {
            return {
              coordinates: Coordinates.map(
                ({ X, Y }: { X: number; Y: number }) => ({ column: X, row: Y })
              ),
              name: Name,
            };
          }),
          myBoard: true,
        }}
        opponentState={{
          myBoard: true,
          placedShips: opponentShips.map(({ Coordinates, Name }) => {
            return {
              coordinates: Coordinates.map(
                ({ X, Y }: { X: number; Y: number }) => ({ column: X, row: Y })
              ),
              name: Name,
            };
          }),
        }}
      />
    </>
  );
};

export default App;
