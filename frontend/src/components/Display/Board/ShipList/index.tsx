import { FC } from "react";
import ShipListItem from "./ShipListItem";
import TileButtons from "./TileButtons";

import "./ShipList.css";
import { Ship } from "../types";

type Props = {
  placedShips: Ship[];
  showConfirmCancelButtons: boolean;
  clearTiles: () => void;
  confirmTiles: () => void;
  active: boolean;
};

const ShipList: FC<Props> = ({
  placedShips,
  showConfirmCancelButtons,
  clearTiles,
  confirmTiles,
  active,
}) => {
  const className = active ? "ship-list active" : "ship-list";
  const lst = [];
  for (const index in placedShips) {
    const ship = placedShips[index];
    lst.push(<ShipListItem ship={ship} />);
  }
  return (
    <div className={className}>
      <div>{lst}</div>
      {showConfirmCancelButtons && (
        <TileButtons {...{ clearTiles, confirmTiles }} />
      )}
    </div>
  );
};

export default ShipList;
