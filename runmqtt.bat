@echo off
REM Uruchamianie kontenera Mosquitto MQTT w Dockerze

docker run -d --name pressurelogger.mqtt ^
  -p 1883:1883 ^
  -p 9001:9001 ^
  -v "%cd%\.containers\mqtt\config:/mosquitto/config" ^
  -v "%cd%\.containers\mqtt\data:/mosquitto/data" ^
  -v "%cd%\.containers\mqtt\log:/mosquitto/log" ^
  eclipse-mosquitto

echo Mosquitto MQTT uruchomiony.
pause