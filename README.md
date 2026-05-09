## Planning

View [Planned Features](https://github.com/users/robchendev/projects/1/views/2)

# Initial Setup

## Front End (Angular)

[Install Node.js](https://nodejs.org/en)
In the `/Frontend` directory

Initial install

```
npm install -g @angular/cli
npm install
```

Run development build

```
ng serve
```

## Database (PostgreSQL)

Install PostgreSQL:

- [MacOS](https://postgresapp.com/)
- [Windows/Linux](https://www.postgresql.org/download/)

During the setup, make sure to also install PostGIS. This is done through Application Stack Builder.

The backend expects to connect using the following credentials, also defined in `appsettings.Development.json`:

- Database: guitardb
- Username: postgres
- Password: 1234

You don't need to create a PostgreSQL Database yourself, as long as you have PostgreSQL installed with the PostGIS extension, running `dotnet run` or `dotnet watch` in the "Back End" section below will create a development database with the above credentials.

## Back End (.NET)

[Install .NET 9](https://dotnet.microsoft.com/en-us/download)
In the `/Backend` directory,

`appsettings.json` is intentionally ignored in git, so it won't appear in your cloned repo. You will need to create the file in the `/Backend` folder with the following temporary contents:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "TokenKey": "anything works here, as long as it is a string of length >= 64 characters",
  "AllowedHosts": "*"
}
```

Initial install

```
dotnet restore
```

Run development build

```
dotnet run
```

## Testing (Jest, xUnit)

Before every PR, make sure the tests work.

- **Frontend (Jest):** in the `/Frontend` directory, run `npm run tests` for a single pass-through of tests, or `npm run tests:watch` to keep it running as you work. In watch mode, Jest will automatically detect changed files through git and run tests for them if they exist.
- **Backend (xUnit):** in the `/Backend.Tests` directory, run `dotnet test`. There's no watch mode so you must run the command again each time to test.

## Environment

For now, non-secret information will go inside `appsettings.Development.json`. Eventually we'll use appsettings.json for deployment, but no secret information should go in there.

# Contributing

## Commits

Please structure your commits like `[feat] some feature`, it doesnt matter what goes in `[]` as long as it makes sense.

## Branching & Pull Requests

Branches and Pull Requests should be named similar to the issue ticket, for example, for an issue numbered **#123** and titled **[Feat] Brewers Revision System**:

- branch name: `123-brewer-revision-system`
- pull request title: [Feat] Revision System for Brewers

## Dev QoL

If you are using VSCode, install the workspace extensions in `/.vscode/extensions`.

View postman collection and current system design files at
https://drive.google.com/drive/folders/1DwAAinVfh1acoeLnDJgPP51gNhoecVis?usp=sharing

## MacOS Quirks

**If you are on MacOS, there are some issues you may run into, documented here.**

Disable AirPlay Receiver, otherwise you will randomly get 403 Forbidden errors on every request. It was a massive pain to figure out what was wrong. https://stackoverflow.com/a/70562478

If you encounter issues running `npm run test:watch`, you can fix it by installing watchman and tweaking file descriptor sizes:

```
brew install watchman
sudo launchctl limit maxfiles 16384 16384 && ulimit -n 16384
```
