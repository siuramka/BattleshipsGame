import CoordinateList from "./CoordinateList";
import CoordinateLabelList from "./CoordinateLabelList";

import "./Coordinate.css";
import { FC } from "react";
import { Ship } from "../types";
import useGame from "../../../../hooks/useGame";

type Props = {
  placedShips: Ship[];
  myBoard: boolean;
};

const Coordinate: FC<Props> = ({ placedShips, myBoard }) => {
  const { handleClickOnGrid } = useGame();

  const lst = [];
  for (let i = 0; i < 10; i++) {
    lst.push(
      <CoordinateList
        {...{
          key: i,
          row: i,
          placedShips,
          myBoard,
          handleClickTile: async (row, column) => {
            await handleClickOnGrid(row, column);
          },
        }}
      />
    );
  }
  return (
    <>
      <div className="row-column">
        <div className="coordinate-space"></div>
        <CoordinateLabelList />
      </div>
      <div className="row-column">
        <CoordinateLabelList isRow={true} />
        <div className="coordinate">{lst}</div>
      </div>
    </>
  );
};

export default Coordinate;
