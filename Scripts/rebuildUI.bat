@echo off

docker-compose up -d --force-recreate --no-deps --build pressurelogger.ui

docker-compose down

echo UI Rebuilded

pause