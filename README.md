# Outline Backup
> Automated backups for [Outline](https://www.getoutline.com/) through [GitHub Actions](https://docs.github.com/en/actions)

# Usage
1. [Fork this repository](https://github.com/zensharp/outline-backup/fork).
1. Add a repository secret. Use name `OUTLINE_API_KEY` and value obtained from your outline instance.
1. Add a repository variable. Use name `OUTLINE_INSTANCE_URL` and value URL of your outline instance.
1. In repository [Settings > Actions > General](https://github.com/zensharp/outline-backup/settings/actions), enable "Read and write permissions".

> If this option is not available, you may need to enable it at the Organization level.
