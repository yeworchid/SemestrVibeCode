#!/bin/bash

echo "Building server..."
dotnet build Server/Server.csproj

if [ $? -eq 0 ]; then
    echo "Server built successfully!"
    echo "To run the server, use: dotnet run --project Server/Server.csproj"
else
    echo "Server build failed!"
fi
