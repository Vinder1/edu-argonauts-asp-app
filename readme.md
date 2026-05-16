# Постигайте 'Аргонавтов'!

'Аргонавты' - ММО в космическом сеттинге, написанная на .Net

# Руководство по разворачиванию

```
docker compose up -d
dotnet ef database update -p .\Argonauts.Infrastructure\Argonauts.Infrastructure.csproj -s .\Argonauts.Web\Argonauts.Web.csproj
```

## Запуск

При разработке:

```
cd ./Argonauts.Web
dotnet run

cd ../Frontend
npm run dev
```
Подключаться к адресу, указанному в Frontend


При разворачивании на сервере:

```
cd Frontend
npm run build      # создаёт файлы в ../Argonauts.Web/wwwroot/

cd ../Argonauts.Web

# через publish (лучше):
dotnet publish -c Release -o ./publish
dotnet ./publish/Argonauts.Web.dll

# либо по старинке:
dotnet run
```

## Порты

3000: Grafana
80: Seq 