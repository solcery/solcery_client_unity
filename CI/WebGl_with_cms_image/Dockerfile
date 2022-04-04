FROM node:16-bullseye as build
RUN apt-get update
RUN apt-get install -y git
RUN git clone https://github.com/solcery/solcery_cms.git
WORKDIR solcery_cms
RUN git fetch
ARG BRANCH
RUN git checkout $BRANCH
COPY ./WebGl/StreamingAssets public/new_game/StreamingAssets
COPY ./WebGl/Build public/new_game
RUN yarn install
RUN yarn build