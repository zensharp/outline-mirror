# Outline Snapshot

# Overview
Outline Snapshot uses the [Outline API](https://www.getoutline.com/developers) and [GitHub Actions](https://docs.github.com/en/actions) to create free, fully version controlled snapshots of your Outline wiki at scheduled intervals.

# Usage
1. [Fork this repository](https://github.com/zensharp/outline-snapshot/fork).
2. Add a repository secret. Use name `OUTLINE_API_KEY` and value obtained from your outline instance.
3. Add a repository variable. Use name `OUTLINE_INSTANCE_URL` and value URL of your outline instance.
4. In repository **Settings > Actions > General**, enable "Read and write permissions".
> [!IMPORTANT]  
> If this option is not available, you may need to enable it at the Organization level.

Now, the workflow will be triggered automatically (default is every 4 hours). You can also manually trigger the workflow from the **Actions** tab.

After the workflow runs, your snapshots will be saved to the `snapshots/json` and `snapshots/markdown` branches.
