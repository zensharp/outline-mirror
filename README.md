# Outline Snapshot

> This repository contains code for generating snapshots from Outline. If you're looking for Outline Snapshot CI/CD, see [Outline Mirror](https://github.com/outline-snapshot).

# Overview
Outline Snapshot uses the [Outline API](https://www.getoutline.com/developers) and [GitHub Actions](https://docs.github.com/en/actions) to create free, fully version controlled snapshots of your Outline wiki at scheduled intervals.

# Building the image locally
```
docker build -t outline-mirror
docker run -it outline-mirror
```
