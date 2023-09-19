import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faAnchor,
  faShip,
  faXmark,
  faBomb,
} from "@fortawesome/free-solid-svg-icons";
import { MISSED, SELECTED, CONFIRMED, HIT } from "../../../../constants";
import { FC } from "react";

type Props = {
  clickHandler: () => void;
  // TODO: refactor me
  state: {
    type: unknown;
    shipName?: string;
  };
};

const CoordinateListItem: FC<Props> = ({ clickHandler, state }) => {
  const { type, shipName } = state;
  const className = shipName ? `square ${shipName}` : "square";
  return (
    <div className={className} onClick={clickHandler}>
      {type === SELECTED && <FontAwesomeIcon icon={faAnchor} />}
      {type === CONFIRMED && <FontAwesomeIcon icon={faShip} />}
      {type === MISSED && <FontAwesomeIcon icon={faXmark} />}
      {type === HIT && <FontAwesomeIcon icon={faBomb} />}
    </div>
  );
};

export default CoordinateListItem;
