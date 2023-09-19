import  { FC } from "react";

type Props = {
  clearTiles: () => void;
  confirmTiles: () => void;
};

const TileButtons: FC<Props> = ({ clearTiles, confirmTiles }) => {
  return (
    <div>
      <button onClick={confirmTiles}>Confirm</button>
      <button className="cancel" onClick={clearTiles}>
        Clear
      </button>
    </div>
  );
};

export default TileButtons;
