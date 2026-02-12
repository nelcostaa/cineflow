# Multi-stage build: Node builds React, Nginx serves it + proxies API
# Build context must be project root (set in docker-compose.yml)

# Build stage
FROM node:20-alpine AS build

WORKDIR /app

ARG VITE_API_URL=http://localhost/api
ENV VITE_API_URL=$VITE_API_URL

COPY CineflowFront/package*.json ./

RUN npm ci

COPY CineflowFront/ .

RUN npm run build

# Production stage - single nginx: serves React build + proxies API
FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html

COPY nginx/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
