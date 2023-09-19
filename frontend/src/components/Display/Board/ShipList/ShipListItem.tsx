import { FC } from "react";
import "./ShipList.css";

type Props = {
  ship: {
    name: string;
  };
};

const ShipListItem: FC<Props> = ({ ship: { name } }) => {
  return (
    <div className="ship-name">
      <div className="label"></div>
      {name}
    </div>
  );
};

export default ShipListItem;
