#!/bin/bash

if [ ! "$(ls -A /opt/sphinx/conf)" ]; then
  echo "No configuration data found. Please put your configuration to /opt/sphinx/conf mapped volume"
  exit 1
fi


if [ ! "$(ls -A /opt/sphinx/data)" ]; then
  indexer --config /opt/sphinx/conf/sphinx.conf --all 
fi

searchd --nodetach --config /opt/sphinx/conf/sphinx.conf
