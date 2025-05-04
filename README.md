# 🛒 E-Commerce Microservices Architecture with .NET 8

This repository contains a **real-world, cloud-native e-commerce microservices application** built with modern technologies and architecture patterns using **.NET**.

The solution demonstrates scalable microservices, asynchronous messaging, distributed caching, and clean API design — ready for local development or cloud deployment.

---

## 🚀 Tech Stack

- **.NET 8 & ASP.NET Core WebAPI**
- **Entity Framework Core + SQL Server** – Relational data store
- **MongoDB** – NoSQL data store (e.g., for product catalog)
- **RabbitMQ** & **Azure Service Bus** – Switchable event bus infrastructure
- **Redis** – Distributed caching for performance
- **Docker & Docker Compose** – Containerized services
- **Ocelot API Gateway** – Unified API entry point
- **Domain-Driven Design**, **SOLID**, and **Clean Architecture**

---

## 📦 Microservices

- `Catalog Service` – Manages products and categories (MongoDB)
- `Ordering Service` – Handles orders and event publishing (SQL Server)
- `Basket Service` – Manages shopping cart with Redis
- `Payment Service` – Simulated payment flow
- `Notification Service` – Sends alerts via message bus
- `Gateway` – API Gateway via Ocelot

---

## ⚙️ Architecture Highlights

- Custom `IEventBus` abstraction with interchangeable backends (RabbitMQ / Azure Service Bus)
- Asynchronous communication using Pub/Sub model
- Background services for handling event consumers
- Services are fully decoupled and independently deployable
- Docker-based development environment with `docker-compose`

---

## 🎯 Key Features

- Event-driven communication with RabbitMQ or Azure Service Bus
- Redis caching for fast shopping cart access
- Modular, clean project structure
- Designed for extensibility and maintainability
- Cloud-ready and fully containerized


