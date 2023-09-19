import * as signalR from "@microsoft/signalr";
import { API_URL } from "./constants";

class Connector {
  private connection: signalR.HubConnection;
  public events: (
    onMessageReceived: (username: string, message: string) => void,
    onSomeOtherServerEventReceived: (payload: string) => void
  ) => void;
  static instance: Connector;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/hub`)
      .withAutomaticReconnect()
      .build();
    this.connection.start().catch((err) => document.write(err));

    this.events = (onMessageReceived, onSomeOtherServerEventReceived) => {
      this.connection.on("messageReceived", (username, message) => {
        onMessageReceived(username, message);
      });
      this.connection.on("somethingDefinedOnServer", (payload) => {
        onSomeOtherServerEventReceived(payload);
      });
    };
  }

  public newMessage = (messages: string) => {
    this.connection
      .send("newMessage", "foo", messages)
      .then(() => console.log("sent"));
  };

  public static getInstance(): Connector {
    if (!Connector.instance) Connector.instance = new Connector();
    return Connector.instance;
  }
}

export default Connector.getInstance;
