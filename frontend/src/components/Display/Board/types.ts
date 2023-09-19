// TODO: refactor me
export type State = {
  myBoard?: any;
  placedShips: Ship[];
  overlaySettings?: any;
  title?: string;
  showConfirmCancelButtons?: any;
  clearTiles?: any;
  clickTile?: any;
  chosenTiles?: any;
  confirmTiles?: any;
  shot?: any;
  active?: any;
};

export type Ship = {
  coordinates: { row: number; column: number }[];
  name: string;
};
