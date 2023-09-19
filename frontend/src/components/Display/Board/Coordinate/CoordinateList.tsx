import CoordinateListItem from "./CoordinateListItem";
import { CONFIRMED } from "../../../../constants";
import { FC } from "react";
import { whichShipCoordinateIsBelong } from "../../../../helpers";
import { Ship } from "../types";

type Props = {
  placedShips: Ship[];
  myBoard: boolean;
  row: number;
  handleClickTile: (row: number, column: number) => void;
};

const CoordinateList: FC<Props> = ({
  placedShips,
  row,
  myBoard,
  handleClickTile,
}) => {
  const lst = [];

  for (let i = 0; i < 10; i++) {
    const coordinate = { row, column: i };

    const state = () => {
      if (myBoard) {
        const shipName = whichShipCoordinateIsBelong(placedShips, coordinate);
        if (shipName) return { type: CONFIRMED, shipName };
      }

      return { type: null };
    };

    lst.push(
      <CoordinateListItem
        {...{
          state: state(),
          key: i,
          clickHandler: () => handleClickTile(row, i),
        }}
      />
    );
  }

  return <div className="row">{lst}</div>;
};

export default CoordinateList;
