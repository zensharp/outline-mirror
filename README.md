# Outline Mirror

> This repository contains reusable workflows, and code for generating snapshots from Outline. For a ready-to-use Outline snapshot solution, see the [Outline Mirror Runner](https://github.com/zensharp/outline-mirror-runner) template.

# Overview
Outline Snapshot uses the [Outline API](https://www.getoutline.com/developers) to generate local snapshots of your Outline wiki.

# Development
## Building the image locally
```
docker build -t outline-mirror .
docker run -it outline-mirror
```
