# Protocol Independent Notification Connector

Dockerized end-to-end notification system that receives messages from different protocols, normalizes them through a protocol-independent connector, and displays them on a React dashboard.

## Architecture

Simulator → RabbitMQ / WebSocket / Redis / Webhook → Connector → Backend API → React Frontend

## Technologies

- .NET 8
- ASP.NET Core Minimal API
- .NET Worker Service
- React + Vite
- RabbitMQ
- Redis
- Docker Compose

## Supported Adapters

- Webhook
- WebSocket
- RabbitMQ
- Redis

Adapters can be enabled or disabled through configuration without rebuilding the project.

## Run

```bash
git clone https://github.com/bredkeklol/NotificationConnector.git
cd NotificationConnector
docker compose up -d --build
```

## Ports

- Frontend: http://localhost:3000
- Backend API: http://localhost:8080
- WebSocket Server: http://localhost:5050
- RabbitMQ Management: http://localhost:15672

## Test Redis

```bash
docker exec -it redis redis-cli PUBLISH notifications "hello redis"
```

## Test Webhook

```bash
curl -X POST http://localhost:8080/webhook ^
  -H "Content-Type: application/json" ^
  -d "{\"title\":\"Webhook\",\"message\":\"Hello\"}"
```

## View Logs

```bash
docker compose logs -f connector
```

## Features

- Protocol-independent connector core
- Runtime adapter selection
- Message normalization
- Error-tolerant processing
- Dockerized deployment
- Live dashboard

## Project Structure

- backend/
- connector/
- frontend/
- simulator/
- websocketserver/

## Shutdown

```bash
docker compose down
```