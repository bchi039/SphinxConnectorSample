
FROM ubuntu:16.04

ENV SPHINX_VERSION_HASH 612d99f
ENV SPHINX_VERSION 3.1.1
ENV DEBIAN_FRONTEND=noninteractive
ENV DEBCONF_FRONTEND=noninteractive

# Add Microsoft repo for Microsoft ODBC Driver 17 for Linux
RUN apt-get update \
    && apt-get install --no-install-recommends -y apt-utils apt-transport-https debconf-utils ca-certificates openssl nano curl \
    && curl -s https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl -s https://packages.microsoft.com/config/ubuntu/16.04/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install --no-install-recommends -y msodbcsql17 unixodbc-dev mssql-tools \
    && rm -rf /var/lib/apt/lists/*

# set up directories
RUN mkdir -pv /opt/sphinx/log /opt/sphinx/data /opt/sphinx/conf /opt/sphinx/dict

# ADD copies, extracts, then deletes the archive from the destination
ADD sphinx-${SPHINX_VERSION}-${SPHINX_VERSION_HASH}-linux-amd64.tar.gz /opt/sphinx/

# redirect logs to stdout
RUN ln -sv /dev/stdout /opt/sphinx/log/query.log 
RUN ln -sv /dev/stdout /opt/sphinx/log/searchd.log

# point to sphinx binaries
ENV PATH "${PATH}:/opt/sphinx/sphinx-${SPHINX_VERSION}/bin"

# expose TCP ports
EXPOSE 9306
EXPOSE 9312

# expose volume(s)
VOLUME /opt/sphinx/data /opt/sphinx/conf

COPY run.sh /opt/sphinx/sphinx-${SPHINX_VERSION}/bin
RUN chmod +x /opt/sphinx/sphinx-${SPHINX_VERSION}/bin/run.sh

COPY rotate_indexes.sh /opt/sphinx/sphinx-${SPHINX_VERSION}/bin
RUN chmod +x /opt/sphinx/sphinx-${SPHINX_VERSION}/bin/rotate_indexes.sh

COPY healthcheck.sh /opt/sphinx
RUN chmod +x /opt/sphinx/healthcheck.sh
#HEALTHCHECK CMD ["healthcheck"]

COPY sphinx.conf /opt/sphinx/conf
COPY data.csv /opt/sphinx/conf

CMD run.sh
