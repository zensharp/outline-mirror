# Outline Snapshot

> This repository contains code for generating snapshots from Outline. For an automated Outline snapshot solution, see [Outline Mirror](https://github.com/zensharp/outline-mirror).

# Overview
Outline Snapshot uses the [Outline API](https://www.getoutline.com/developers) and [GitHub Actions](https://docs.github.com/en/actions) to generate local snapshots of your Outline wiki.

# Building the image locally
```
docker build -t outline-mirror
docker run -it outline-mirror
```
