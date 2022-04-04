FROM nginx:alpine

WORKDIR /WebGl
COPY WebGl/ .

WORKDIR /etc/nginx/conf.d
RUN rm default.conf
COPY nginx.conf nginx.conf
