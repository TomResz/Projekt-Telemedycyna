@echo off

echo Rebuilding API
docker-compose up -d --force-recreate --no-deps --build pressurelogger.api

echo  Rebuilding UI
docker-compose up -d --force-recreate --no-deps --build pressurelogger.ui

docker-compose down

docker-compose --project-name pressurelogger up -d
