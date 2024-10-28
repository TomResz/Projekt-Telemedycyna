@echo off

docker-compose up -d --force-recreate --no-deps --build pressurelogger.api

docker-compose down

echo API Rebuilded

pause
