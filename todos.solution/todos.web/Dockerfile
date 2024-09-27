# Stage 1: Build the React app
FROM node:18 as build
WORKDIR /app

# Copy the package.json and package-lock.json for dependency installation
COPY ./todos.web/package.json ./todos.web/package-lock.json ./

# Install the dependencies
RUN npm install

# Copy the rest of the React app source code
COPY ./todos.web ./

# Build the React app
RUN npm run build

# Stage 2: Serve the React app with Nginx
FROM nginx:alpine

# Copy the built React app from Stage 1
COPY --from=build /app/build /usr/share/nginx/html

# Copy SSL certificates (make sure they exist locally in todos.web/certs)
COPY certs/aspnetapp.crt /etc/nginx/certs/aspnetapp.crt
COPY certs/aspnetapp.key /etc/nginx/certs/aspnetapp.key

# Copy Nginx configuration for HTTPS
COPY ./todos.web/nginx.conf /etc/nginx/conf.d/default.conf

# Set permissions
RUN chmod -R 755 /usr/share/nginx/html

# Expose HTTP and HTTPS ports
EXPOSE 80
EXPOSE 443

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
