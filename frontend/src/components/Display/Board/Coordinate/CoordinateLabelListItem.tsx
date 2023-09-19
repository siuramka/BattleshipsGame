import { FC } from "react";
import { columnLabel } from "../../../../constants";

type Props = {
  isRow: boolean;
  index: number;
};

const CoordinateLabelListItem: FC<Props> = ({ isRow, index }) => {
  const coordinateClassName = isRow
    ? "coordinate-label-row"
    : "coordinate-label-column";

  const label = isRow ? index + 1 : columnLabel[index];

  return <div className={coordinateClassName}>{label}</div>;
};

export default CoordinateLabelListItem;
