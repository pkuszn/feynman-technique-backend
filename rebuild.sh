#!/bin/bash

docker rm -f feynman-technique-backend && docker rmi feynman-technique-backend-feynman-technique-backend:latest && git reset --hard && git.exe pull && dos2unix build.sh && chmod +x build.sh && ./build.sh