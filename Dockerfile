FROM mono:5.12.0.226
WORKDIR /app
COPY CDP4WebServer/bin/Release/net472 /app
COPY LICENSE /app

RUN mkdir /app/logs
VOLUME /app/logs

RUN mkdir /app/storage
VOLUME /app/storage

RUN mkdir /app/upload
VOLUME /app/upload

#expose ports
EXPOSE 5000

CMD ["mono", "./CDP4WebServer.exe"]