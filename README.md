# Notification Connector

A protocol-independent notification connector developed as an internship project.

## Technologies

- .NET 8
- React (Vite)
- Docker
- Docker Compose

## Project Structure

```
NotificationConnector
│
├── backend
├── frontend
├── simulator
└── docker-compose.yml
```

## Run with Docker

```bash
docker compose up --build
```

Frontend

```
http://localhost:3000
```

Backend

```
http://localhost:8080/api/notifications
```

## Current Status

- ✅ Backend API
- ✅ React Dashboard
- ✅ Notification Simulator
- ✅ Docker Support