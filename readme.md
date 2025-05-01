
# Accio CLI

  

![Accio CLI Logo](assets/accio-cli-logo.png)
<br />
Ever been tasked with manipulating a database—editing it, then needing to restore the data afterward? Accio CLI simplifies database management by providing easy-to-use commands to create, restore, and list databases. It also supports environment profiles (like dev, staging, and production), so you don’t have to retype connection details every time.
Check out the documentation for full details and examples.

  

## Features

-  **Create a PostgreSQL database backup**
-  **Restore a PostgreSQL database from a backup**
-  **List, create, check databases**
- **Create profiles to most used servers**


## Requirements

  

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)

- [PostgreSQL](https://www.postgresql.org/download/) (if using PostgreSQL features)

-  `psql` and `pg_dump` must be available in the system path (for PostgreSQL backups)

  

## Installation
  
The best option to use the Accio is to publish the cli, copy files to folder, and add it to the environmentvariable. This way you will be able to access it from any folder.

  

```sh

# Clone the repository
git  clone  https://github.com/bregunceluan/accio.cli.git

# Navigate to the project directory
cd  accio-cli

# Publish the folder (ex: to Windows 64bits)
#Windows
dotnet publish -r win-x64 -p:PublishSingleFile=true

# Move to the folder published
cd .\\bin\\Release\\net8.0\\win-x64\\publish\\

# Create a recommended folder for CLI tools
# This folder will store your CLI tool permanently
mkdir %USERPROFILE%\.tools\AccioCLI

# Copy the published files to that folder
copy * %USERPROFILE%\.tools\AccioCLI\

# Map this folder to the environment variables.

```

  
## Usage
Once the CLI is mapped to your system’s environment variables, you can run it using the following command:
```sh

# the format of the commands expected
accio.cli.exe [commands] [options]

```
You can append --help or -h to any command to view available options and usage instructions.




### Available Commands


#### Profiles Commands 
Profiles are preconfigured database settings stored in configs/config.json. They help avoid typing connection details repeatedly by allowing you to save commonly used configurations such as dev, staging, or production.


```sh
#List the profile existant
accio.cli.exe profile listprofiles
```

```json
// You will get response like this: 

📂 Available Profiles:

[dev]

{
  "Postgres": {
    "UserName": "postgres",
    "DataBaseName": "postgres",
    "Host": "localhost",
    "Port": 5432
  }
}
----------------------------------------
[prod]

{
  "Postgres": {
    "UserName": "prod",
    "DataBaseName": "users",
    "Host": "prod.enviroment.com",
    "Port": 5432
  }
}
----------------------------------------
```

-  **Create a new profile**
```sh
#Creating a new profile to staging
accio.cli.exe profile postgres --name staging --u stagin --d users --h staging.enviroment.com --p 5433
```

-  **Delete a profile**
```sh
#Deleting the staging profile
accio.cli.exe profile delete --profile staging
```


#### PostgreSQL Commands

To any of the postgres command, you can either use the typed postgres server configs or the profile.

-  **Verify if a database exists**

```sh
#Check if the database 'db_teste' exists to the default configuration (list those dafaults typing accio.cli.exe db exist --help)
accio.cli.exe db exist --d db_teste --pass <db_password>

#Check the same, but at this time to the dev enviroment
accio.cli.exe db exist --profile dev --pass <db_password>

```

-  **List all databases**

```sh

#List the database to the dev profile
accio.cli.exe db listdatabases --profile dev --pass <db_password>

#List the databases existent
accio.cli.exe db listdatabases --u <user_name> --d <database_name> --h <url> --p <port>

```

-  **Create a new database**

```sh

#Create a new database to with the dev profile configs
accio.cli.exe db create --pass <db_password> -d <new_db_name> --profile dev


```

-  **Backup a PostgreSQL database**

```sh
#Create a backup to the database 'ftc' with the dev profile. 
accio.cli.exe db backup --pass <db_password> --profile dev -d <db_name> --file <path_to_save>

#Create a backup to the server typed.
accio.cli.exe db backup --pass <db_password> --profile dev -u <user> -d <db_name> -h <port> --file <path_to_save>

```

-  **Restore a PostgreSQL database backup**

```sh
#Set the backup file
accio.cli.exe db set --profile dev --pass <db_pass> --file <path_dump_file> -d <db_name>

```


## Configuration

The `Postgres` model allows setting default values for:

-  `UserName` (default: `postgres`)

-  `DataBaseName` (default: `postgres`)

-  `Host` (default: `localhost`)

-  `Port` (default: `5432`)

  

These can be overridden via command-line options.

  

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.

2. Create a new branch.

3. Commit your changes.

4. Open a pull request.