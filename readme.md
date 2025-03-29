# Accio CLI

![Accio CLI Logo](assets/accio-cli-logo.png)
Accio CLI started as a personal project to assist me with my daily development tasks, particularly in managing backups and automating routine operations. Over time, I realized its potential to help other developers as well, so I decided to make it open-source for anyone who wants to use it.

## Features

- **Check if PostgreSQL is installed**
- **Verify database existence**
- **List all databases**
- **Create a new database**
- **Backup a PostgreSQL database**
- **Restore a PostgreSQL database from a backup**


## Requirements

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [PostgreSQL](https://www.postgresql.org/download/) (if using PostgreSQL features)
- `psql` and `pg_dump` must be available in the system path (for PostgreSQL backups)

## Installation

To use Accio CLI, clone the repository and build the project:

```sh
# Clone the repository
git clone https://github.com/bregunceluan/accio.cli.git

# Navigate to the project directory
cd accio-cli

# Build the project
dotnet build
```

## Usage

Run the CLI application using the following command:

```sh
dotnet run -- [category] [command] [options]
```

### Available Commands

#### PostgreSQL Commands

- **Check if PostgreSQL is installed**
  ```sh
  dotnet run -- db installed
  ```

- **Verify if a database exists**
  ```sh
  dotnet run -- db exist --pass <db_password>
  ```

- **List all databases**
  ```sh
  dotnet run -- db listdatabases --pass <db_password>
  ```

- **Create a new database**
  ```sh
  dotnet run -- db create --pass <db_password>
  ```

- **Backup a PostgreSQL database**
  ```sh
  dotnet run -- db backup --pass <db_password> --file <backup_path>
  ```

- **Restore a PostgreSQL database backup**
  ```sh
  dotnet run -- db set --pass <db_password> --file <backup_path> --force (if needed)
  ```

#### MongoDB Commands

- **Backup a MongoDB database**
  ```sh
  dotnet run -- mongo backup --db <database_name> --file <backup_path>
  ```

- **Restore a MongoDB backup**
  ```sh
  dotnet run -- mongo restore --db <database_name> --file <backup_path>
  ```

#### Docker Commands

- **Backup a Docker container volume**
  ```sh
  dotnet run -- docker backup --volume <volume_name> --file <backup_path>
  ```

- **Restore a Docker container volume**
  ```sh
  dotnet run -- docker restore --volume <volume_name> --file <backup_path>
  ```

## Configuration

The `Postgres` model allows setting default values for:
- `UserName` (default: `postgres`)
- `DataBaseName` (default: `postgres`)
- `Host` (default: `localhost`)
- `Port` (default: `5432`)

These can be overridden via command-line options.

## Contributing

Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a new branch.
3. Commit your changes.
4. Open a pull request.

