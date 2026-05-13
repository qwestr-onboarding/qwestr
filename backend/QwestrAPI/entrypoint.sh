#!/bin/sh
mkdir -p /app/wwwroot/uploads
chown -R app:app /app/wwwroot/uploads
exec su -s /bin/sh -c "exec dotnet QwestrAPI.dll" app
