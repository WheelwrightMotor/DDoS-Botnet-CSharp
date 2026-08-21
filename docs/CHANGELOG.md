# Changelog

All notable changes to this project will be documented in this file.

## [2.0.0] - 2026-07-10

### Added
- DNS amplification attack module
- Slow Loris connection exhaustion
- Proxy pool rotation with dead-proxy removal
- Process guard with analysis tool detection
- Geolocation for bot tracking

### Changed
- Migrated to .NET 9
- Redesigned C2 protocol for lower latency

## [1.5.0] - 2026-05-20

### Added
- UDP amplification module
- Auto-start persistence mechanism
- Raw socket wrapper for custom packet crafting

### Fixed
- Memory leak in HTTP flood worker
- Connection timeout in slow networks

## [1.0.0] - 2026-04-01

### Added
- Initial release
- HTTP flood with proxy support
- SYN flood with raw sockets
- Basic C2 server with bot management
- Attack scheduler with queue system
