# Introduction

This repository contains TvMaze scraper WebApi application written in the latest version of .Net, which is .Net 8.0 at the time of writing.
The TvMaze application uses a SQLite database, which is created on the fly at start of application if not exists.   

# Getting Started

## Prerequisites

- Visual Studio 2022 Version 64-bit Version 17.8.1 or higher 
- .NET SDK 8.0.100
- [Docker desktop](https://www.docker.com/products/docker-desktop/) (optional, only need when wanting to build and run Docker file )
- Access to [TvMaze api](http://api.tvmaze.com/)

Check installed dotnet sdks

```powershell
dotnet --list-sdks
```

# Build

Clone the GitHub repository
```powershell
git clone https://github.com/ictbrain/tvmaze.git
```

Go to subdirectory `.\tvmaze\src`
```powershell
cd .\tvmaze\src\
```

Build dotnet solution
```powershell
dotnet build --configuration Release
```

Note: 
You could also build the solution with Visual studio 2022.
Then you should open file `.\tvmaze\src\TvMaze.sln` with Visual studio 2022 

# Run

Go to subdirectory `TvMaze.WebApi`  relative to `.\tvmaze\src`

```powershell
cd  .\TvMaze.WebApi\
```

Run dotnet solution

```powershell
dotnet run
```

Note: 
You could also run the solution with Visual studio 2022.

While running the following files are created if not exists
- tvmaze.db ( This is an SQLite database containing all shows with cast scraped from [TvMaze api](http://api.tvmaze.com/) . )
- log.txt ( Besides the console, application output is also logged to file and rotated daily. )

# Test

The TvMaze application has an api and can be accessed on following link http://localhost:5039/api/shop?page=<number>

Where `<number>` is between 0 and max integer. The `page` parameter is optional. 


# Docker 

You can also build and run the TvMaze docker container. Make sure `Docker Desktop` is installed and running.

Go to subdirectory `.\tvmaze\src`
```powershell
cd .\tvmaze\src\
```

build TvMaze docker container
```powershell
docker build --tag tvmaze:latest .
```

run TvMaze docker container
```powershell
docker run -d --name tvmaze -p 5039:80 tvmaze:latest
```
