FROM mono:5.20.1.19

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq nano netcat

WORKDIR /app
COPY CDP4WebServer/bin/Release/net472 /app
COPY LICENSE /app

RUN mkdir /app/wait-for
COPY wait-for /app/wait-for
COPY entrypoint.sh /app

RUN mkdir /app/logs; exit 0
VOLUME /app/logs

RUN mkdir /app/storage; exit 0
VOLUME /app/storage

RUN mkdir /app/upload; exit 0
VOLUME /app/upload

#expose ports
EXPOSE 5000

CMD ["/bin/bash", "/app/entrypoint.sh"]