.DEFAULT_GOAL := all

build:
	dotnet build

run:
	dotnet run

all: build run
