import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App.tsx";
import { GameProvider } from "./contexts/gameContext.tsx";
// import { CssBaseline, ThemeProvider } from "@mui/material";
// import theme from "./common/theme.ts";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    {/* <ThemeProvider theme={theme}> */}
    {/* <CssBaseline /> */}
    <GameProvider>
      <App />
    </GameProvider>
    {/* </ThemeProvider> */}
  </React.StrictMode>
);
