import { FC } from "react";
import Coordinate from "./Coordinate";
import ShipList from "./ShipList";
import Overlay from "./Overlay";
import "./Board.css";
import { State } from "./types";

type Props = {
  state: State;
};

const Board: FC<Props> = ({ state }) => {
  const {
    myBoard,
    placedShips,
    overlaySettings,
    title,
    showConfirmCancelButtons,
    clearTiles,
    confirmTiles,
    shot,
    active,
  } = state;

  const boardClass = active ? "board active" : "board";

  const coordinate = (
    <div className={boardClass}>
      <h3>{title}</h3>
      <Coordinate myBoard={true} placedShips={placedShips} />
    </div>
  );

  const shipList = (
    <ShipList
      {...{
        active,
        placedShips,
        showConfirmCancelButtons,
        clearTiles,
        confirmTiles,
        shot,
      }}
    />
  );

  const board = myBoard ? (
    <>
      {shipList}
      {coordinate}
    </>
  ) : (
    <>
      {coordinate}
      {shipList}
    </>
  );

  return (
    <div className="whole-board">
      {board}
      <Overlay settings={overlaySettings} />
    </div>
  );
};

export default Board;
