# Recathon

**Recathon** is a music recommendation project. It analyzes the musical characteristic of the playlists with using machine learning algorithms and generates a new playlist with recommended tracks.

## Technologies

Used technologies are:

* .NET 5
    * Dapper
    * Hangfire
    * Swagger
    * Polly
* Python
    * Flask
    * Pandas
    * Scikit-learn
* React
    * Axios
    * Redux
    * Material-UI
* PostgreSQL
* MongoDB
* Redis

## Setup

To run the project locally, follow the instructions.

### 1. Prerequisite

* .NET 5
* Python 3
* node.js
* PostgreSQL
* MongoDB
* Redis

### 2. Spotify Api Configuration

Recathon depends on the Spotify Web Api for music recommendation. You should have a valid access token to able to run the project.

Visit [Spotify's official documentation](https://developer.spotify.com/documentation/web-api/) for more detailed information.

### 3. Database Configuration

All necessary scripts for the database configuration are provided under `/db` folder.

Run the sql scripts inside `/tables`, `/procedures` and `/scripts` folders respectively to have the correct database structure.

NOTE: Don't forget to update the `client_id, client_secret, access_token` fields inside `/scripts/crendetial_inserts.sql` with the values that you've provided from Spotify.

### 4. Prediction Service Configuration

To install the necessary packages, follow this scripts.

```bash
$ cd recathon\src\Prediction\

$ python -m venv venv

$ venv\Scripts\activate.bat

$ python -m pip install -U pip

$ pip install -r requirements.txt
```

After package installation is done, open the `settings.development.json` file or one of the `settings.*.json` files depending on your environment. Edit the "connectionString" variable with your
database connection string that you've created on the previous step.

Example:

* username: rec-user
* password: rec-password
* host: 127.0.0.1
* port: 5432
* database: recathondb

```json
{
  "connectionString": "postgresql://rec-user:rec-password@127.0.0.1:5432/recathondb"
}
```

### 5. Spotify Gateway Service Configuration

This service uses Telegram for crucial alerts. You can create a Telegram bot and a chat with that bot to receive the alert messages immediately or just change the `TelegramSettings.LogLevel` to `None`
to not use this feature at all.

If you want to use this feature, update the following settings in `Configuration` table.

Visit [Telegram's official documentation](https://core.telegram.org/bots/api) for detailed information.

```text
TelegramSettings.Token
TelegramSettings.ErrorLogChatId
TelegramSettings.InformationChatId
```

Open the `appsettings.Development.json` file or one of the `appsettings.*.json` files depending on your environment.

Edit the database option settings with your local database connection's information.

Example:

PostreSQL Settings

* username: rec-user
* password: rec-password
* host: 127.0.0.1
* port: 5432
* database: recathondb

MongoDB Settings

* host: 127.0.0.1
* port: 27017

Redis Settings

* host: 127.0.0.1
* port: 6379

```json
{
  "DatabaseOptions": {
    "DefaultConnection": "Server=127.0.0.1;Port=5432;Database=recathondb;User Id=rec-user;Password=rec-password;",
    "MongoDbConnection": "mongodb://127.0.0.1:27017"
  },
  "RedisOptions": {
    "Connection": "127.0.0.1:6379"
  }
}
```