# Using the .NET SDK base image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Setting the working directory inside the container
WORKDIR /app

# Copying the project files to the container's working directory
COPY . .

# Restoring the project's dependencies
RUN dotnet restore RO.DevTest.WebApi/RO.DevTest.WebApi.csproj

# Building the application
RUN dotnet build RO.DevTest.WebApi/RO.DevTest.WebApi.csproj -c Release -o /app/build

# Publishing the application (preparing the version for production)
RUN dotnet publish RO.DevTest.WebApi/RO.DevTest.WebApi.csproj -c Release -o /app/publish

# Using the base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Setting the working directory inside the container
WORKDIR /app

# Copying the published application into the container
COPY --from=build /app/publish .

# Exposing the port where the application will run
EXPOSE 80

# Defining the command to run the application
ENTRYPOINT ["dotnet", "RO.DevTest.WebApi.dll"]
