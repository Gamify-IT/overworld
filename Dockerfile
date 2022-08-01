FROM nginx:alpine

WORKDIR /etc/nginx/conf.d
COPY .nginx/webgl.conf default.conf

WORKDIR /webgl
COPY builds/WebGL/WebGL .
COPY .nginx/style.css ./TemplateData/style.css

EXPOSE 80/tcp