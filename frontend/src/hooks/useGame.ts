import { useGameContext } from "../contexts/gameContext";

const useGame = () => {
  const { hubConnection } = useGameContext();

  const handleClickOnGrid = async (row: number, column: number) => {
    await hubConnection?.invoke("MakeMove", row, column);
  };

  return { handleClickOnGrid };
};

export default useGame;
