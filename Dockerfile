FROM mono:5.4.0
WORKDIR /app
COPY CDP4WebServer/bin/Release/net452 /app
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