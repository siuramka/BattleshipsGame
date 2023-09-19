import { FC } from "react";

type Props = {
  settings: boolean;
};

const Overlay: FC<Props> = ({ settings }) => {
  const style = settings ? { display: "flex" } : {};
  return (
    <div className="overlay" style={style}>
      <h2>{settings && settings}</h2>
    </div>
  );
};

export default Overlay;
