import { useEffect, useState } from "react";
import "./App.css";

function App() {
  const [notifications, setNotifications] = useState([]);

  const loadNotifications = async () => {
    const response = await fetch(`${import.meta.env.VITE_API_URL}/api/notifications`);
    const data = await response.json();

    // En yeni bildirim üstte olsun
    setNotifications(data.reverse());
  };

  useEffect(() => {
    loadNotifications();

    const interval = setInterval(loadNotifications, 2000);

    return () => clearInterval(interval);
  }, []);

  const sourceColor = (source) => {
    switch (source) {
      case "RabbitMQ":
        return "#27ae60";

      case "Redis":
        return "#8e44ad";

      case "WebSocket":
        return "#3498db";

      case "Webhook":
        return "#e67e22";

      default:
        return "#7f8c8d";
    }
  };

  return (
    <div className="container">
      <h1>🔔 Notification Dashboard</h1>

      {notifications.map((notification) => (
        <div className="card" key={notification.id}>
          <div
            className="source"
            style={{ color: sourceColor(notification.source) }}
          >
            ● {notification.source}
          </div>

          <h2>{notification.title}</h2>

          <p>{notification.message}</p>

          <span>
            {new Date(notification.timestamp).toLocaleString()}
          </span>
        </div>
      ))}
    </div>
  );
}

export default App;