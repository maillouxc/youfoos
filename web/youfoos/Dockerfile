##### STAGE 1: Build #####

FROM node:10-alpine as builder
COPY package.json package-lock.json ./

## Storing node modules in their own layer will prevent repeat npm installing during rebuilds

RUN npm ci && mkdir /ng-app && mv ./node_modules ./ng-app

WORKDIR /ng-app

COPY . .

## Build the Angular app in production mode and store the artifacts in dist folder

RUN npm run ng build -- --prod --output-path=dist

##### STAGE 2: Setup #####

FROM nginx:1.14.1-alpine

## Copy our default nginx config file
COPY nginx/default.conf /etc/nginx/conf.d/

## Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

## From the 'Builder' stage copy over the artifacts in the dist folder to default nginx public folder
COPY --from=builder /ng-app/dist /usr/share/nginx/html

CMD ["nginx", "-g", "daemon off;"]
