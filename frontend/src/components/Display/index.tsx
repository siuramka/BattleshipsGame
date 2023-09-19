import { FC } from "react";
import Board from "./Board";
import "./Display.css";
import { State } from "./Board/types";

type Props = {
  myState: State;
  opponentState: State;
};

const Display: FC<Props> = ({ myState, opponentState }) => {
  return (
    <div className="display">
      <Board state={myState} />
      <Board state={opponentState} />
    </div>
  );
};

export default Display;
