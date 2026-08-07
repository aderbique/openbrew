FROM mono:latest AS build

ENV DEBIAN_FRONTEND=noninteractive

RUN printf 'deb http://archive.debian.org/debian buster main\n\
deb http://archive.debian.org/debian-security buster/updates main\n' > /etc/apt/sources.list \
    && apt-get -o Acquire::Check-Valid-Until=false update \
    && apt-get -o Acquire::Check-Valid-Until=false install -y --no-install-recommends \
        mono-xsp4 \
        nuget \
        unzip \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY . /src

RUN nuget restore Openbrew.Web.sln -PackagesDirectory /packages
RUN MONO_GAC_PREFIX=/usr xbuild Openbrew.Web.sln /p:Configuration=Release /verbosity:minimal
RUN MONO_GAC_PREFIX=/usr xbuild Openbrew.DbInit/Openbrew.DbInit.csproj /p:Configuration=Release /verbosity:minimal

FROM mono:latest

ENV DEBIAN_FRONTEND=noninteractive

RUN printf 'deb http://archive.debian.org/debian buster main\n\
deb http://archive.debian.org/debian-security buster/updates main\n' > /etc/apt/sources.list \
    && apt-get -o Acquire::Check-Valid-Until=false update \
    && apt-get -o Acquire::Check-Valid-Until=false install -y --no-install-recommends \
        mono-xsp4 \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /src /app
COPY scripts/docker-image-entrypoint.sh /usr/local/bin/openbrew-entrypoint.sh
RUN chmod +x /usr/local/bin/openbrew-entrypoint.sh

ENTRYPOINT ["/usr/local/bin/openbrew-entrypoint.sh"]
