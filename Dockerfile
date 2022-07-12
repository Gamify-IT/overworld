FROM nginx:alpine

WORKDIR /etc/nginx/conf.d
COPY docker/nginx/webgl.conf default.conf

WORKDIR /webgl
COPY builds/WebGL/WebGL .
COPY docker/style.css ./TemplateData/style.css

EXPOSE 80/tcp