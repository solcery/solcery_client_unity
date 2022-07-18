@ECHO off
for %%I IN (%*) DO set CI_COMPOSE=%%I
docker-compose -f %CI_COMPOSE% build
docker-compose -f %CI_COMPOSE% up -d
explorer "http://localhost:8080/"