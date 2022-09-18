.DEFAULT_GOAL := all

build:
	dotnet build

run:
	dotnet exec "bin/Debug/net6.0/White World.dll"

all: build run
